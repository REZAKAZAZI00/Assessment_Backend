using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using Serilog.Sinks.MSSqlServer;

namespace Assessment_Backend.Middleware
{
    /// <summary>
    /// ثبت درخواست (Request) و پاسخ (Response) هر درخواست HTTP در دو جدول مجزا:
    /// RequestLogs و ResponseLogs.
    ///
    /// این Middleware فقط رخدادهای Serilog با پراپرتی LogType=Request/Response تولید می‌کند
    /// و خودش چیزی در دیتابیس نمی‌نویسد؛ نوشتن در جدول‌ها توسط دو Sink مخصوص
    /// Serilog.Sinks.MSSqlServer که در Program.cs تنظیم شده‌اند انجام می‌شود.
    ///
    /// باید «اولین» Middleware در Pipeline باشد تا خطاهای GlobalExceptionHandlingMiddleware
    /// هم به عنوان Response ثبت شوند.
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>حداکثر طول Body درخواستی که ذخیره می‌شود (بقیه بریده می‌شود).</summary>
        private const int MaxRequestBodyLogLength = 16 * 1024;

        /// <summary>حداکثر طول Body پاسخ که ذخیره می‌شود (بقیه بریده می‌شود).</summary>
        private const int MaxResponseBodyLogLength = 32 * 1024;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = Activity.Current?.Id ?? context.TraceIdentifier;
            var stopwatch = Stopwatch.StartNew();

            var requestBody = await ReadRequestBodyAsync(context.Request);

            // 1) ثبت Request در جدول RequestLogs
            Log.ForContext("LogType", "Request")
               .ForContext("RequestId", requestId)
               .ForContext("RequestMethod", context.Request.Method)
               .ForContext("RequestPath", context.Request.Path.ToString())
               .ForContext("RequestBody", requestBody)
               .ForContext("ClientIp", context.Connection.RemoteIpAddress?.ToString() ?? "")
               .ForContext("UserName", context.User?.Identity?.Name ?? "")
               .ForContext("TraceId", requestId)
               .Information("Incoming request {RequestMethod} {RequestPath}",
                   context.Request.Method, context.Request.Path);

            // 2) بافر کردن Response در حافظه تا بعد از اجرای Pipeline بتوانیم Body آن را بخوانیم
            var originalBodyStream = context.Response.Body;
            await using var bufferedBody = new MemoryStream();
            context.Response.Body = bufferedBody;

            try
            {
                await _next(context);
            }
            finally
            {
                // حتی در صورت Exception (که GlobalExceptionHandlingMiddleware پاسخ 500 می‌سازد) ثبت می‌شود
                stopwatch.Stop();

                var responseBody = await ReadResponseBodyAsync(bufferedBody);
                var elapsedMs = stopwatch.ElapsedMilliseconds;
                var statusCode = context.Response.StatusCode;

                // ارسال محتوای بافر شده به کلاینت
                bufferedBody.Position = 0;
                await bufferedBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;

                var responseLog = Log.ForContext("LogType", "Response")
                    .ForContext("RequestId", requestId)
                    .ForContext("RequestMethod", context.Request.Method)
                    .ForContext("RequestPath", context.Request.Path.ToString())
                    .ForContext("StatusCode", statusCode)
                    .ForContext("ResponseBody", responseBody)
                    .ForContext("ElapsedMs", elapsedMs);

                if (statusCode >= 500)
                    responseLog.Warning("Outgoing response {StatusCode} for {RequestMethod} {RequestPath} in {ElapsedMs}ms",
                        statusCode, context.Request.Method, context.Request.Path, elapsedMs);
                else
                    responseLog.Information("Outgoing response {StatusCode} for {RequestMethod} {RequestPath} in {ElapsedMs}ms",
                        statusCode, context.Request.Method, context.Request.Path, elapsedMs);
            }
        }

        /// <summary>
        /// Body درخواست را می‌خواند بدون اینکه Pipeline بعدی نتواند آن را بخواند
        /// (EnableBuffering + برگرداندن Position به صفر). برای multipart (آپلود فایل) چیزی خوانده نمی‌شود.
        /// </summary>
        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            var contentType = request.ContentType ?? "";

            if (contentType.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase))
                return "[multipart body - not logged]";

            if (!contentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) &&
                !contentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
                return string.Empty;

            try
            {
                request.EnableBuffering();
                using var reader = new StreamReader(request.Body, Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;
                return Truncate(body, MaxRequestBodyLogLength);
            }
            catch
            {
                return "[failed to read request body]";
            }
        }

        /// <summary>
        /// Body پاسخ را از استریم بافر شده در حافظه می‌خواند و سپس آن را به استریم اصلی
        /// (سمت کلاینت) کپی می‌کند.
        /// </summary>
        private static async Task<string> ReadResponseBodyAsync(MemoryStream bufferedBody)
        {
            try
            {
                bufferedBody.Position = 0;
                using var reader = new StreamReader(bufferedBody, Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                return Truncate(body, MaxResponseBodyLogLength);
            }
            catch
            {
                return "[failed to read response body]";
            }
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value;

            return value[..maxLength] + $"...[truncated, total {value.Length} chars]";
        }
    }

    /// <summary>
    /// تعریف ستون‌های سفارشی دو جدول لاگ (ستون‌های استاندارد Serilog مثل
    /// Id, Message, Level, TimeStamp, Exception به صورت پیش‌فرض وجود دارند).
    /// </summary>
    public static class RequestResponseLogColumns
    {
        public static ColumnOptions RequestOptions() => new()
        {
            AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn { ColumnName = "RequestId",     PropertyName = "RequestId",     DataType = SqlDbType.NVarChar, DataLength = 64 },
                new SqlColumn { ColumnName = "RequestMethod", PropertyName = "RequestMethod", DataType = SqlDbType.NVarChar, DataLength = 10 },
                new SqlColumn { ColumnName = "RequestPath",   PropertyName = "RequestPath",   DataType = SqlDbType.NVarChar, DataLength = 2048 },
                new SqlColumn { ColumnName = "RequestBody",   PropertyName = "RequestBody",   DataType = SqlDbType.NVarChar },
                new SqlColumn { ColumnName = "ClientIp",      PropertyName = "ClientIp",      DataType = SqlDbType.NVarChar, DataLength = 45 },
                new SqlColumn { ColumnName = "UserName",      PropertyName = "UserName",      DataType = SqlDbType.NVarChar, DataLength = 256 },
                new SqlColumn { ColumnName = "TraceId",       PropertyName = "TraceId",       DataType = SqlDbType.NVarChar, DataLength = 64 },
            }
        };

        public static ColumnOptions ResponseOptions() => new()
        {
            AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn { ColumnName = "RequestId",     PropertyName = "RequestId",     DataType = SqlDbType.NVarChar, DataLength = 64 },
                new SqlColumn { ColumnName = "RequestMethod", PropertyName = "RequestMethod", DataType = SqlDbType.NVarChar, DataLength = 10 },
                new SqlColumn { ColumnName = "RequestPath",   PropertyName = "RequestPath",   DataType = SqlDbType.NVarChar, DataLength = 2048 },
                new SqlColumn { ColumnName = "StatusCode",    PropertyName = "StatusCode",    DataType = SqlDbType.Int },
                new SqlColumn { ColumnName = "ResponseBody",  PropertyName = "ResponseBody",  DataType = SqlDbType.NVarChar },
                new SqlColumn { ColumnName = "ElapsedMs",     PropertyName = "ElapsedMs",     DataType = SqlDbType.BigInt },
            }
        };
    }

    /// <summary>ثبت Middleware به صورت متد الحاقی.</summary>
    public static class RequestResponseLoggingExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
            => app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}
