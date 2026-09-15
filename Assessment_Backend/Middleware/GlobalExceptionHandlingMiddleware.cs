using Assessment_Backend.Core.Exceptions;
using System.Text.Json;
using System.Text.Encodings.Web;

namespace Assessment_Backend.Middleware
{
    /// <summary>
    /// مدیریت متمرکز Exception ها برای کل برنامه.
    /// به جای try/catch تکراری در Controller ها و Service ها، تمام Exception ها
    /// در همین نقطه Log شده و پاسخ مناسب به کلاینت برمی‌گردد.
    /// </summary>
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            string message;
            object? errors = null;

            switch (exception)
            {
                case ValidationAppException validationEx:
                    statusCode = validationEx.StatusCode;
                    message = validationEx.UserMessage;
                    errors = validationEx.Errors;
                    _logger.LogWarning("Validation failed on {Path}: {Message}",
                        context.Request.Path, exception.Message);
                    break;

                case AppException appEx:
                    statusCode = appEx.StatusCode;
                    message = appEx.UserMessage;
                    LogByLevel(appEx.LogLevel, appEx, context);
                    break;

                case OperationCanceledException:
                    // کلاینت درخواست را قطع کرده - خطای واقعی نیست
                    context.Abort();
                    return;

                case DbUpdateException dbEx:
                    statusCode = 500;
                    message = "خطایی در ذخیره‌سازی اطلاعات رخ داد. مجدداً تلاش کنید.";
                    _logger.LogError(dbEx, "Database update failed on {Path}", context.Request.Path);
                    break;

                default:
                    statusCode = 500;
                    message = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید";
                    _logger.LogError(exception, "Unhandled exception on {Method} {Path}",
                        context.Request.Method, context.Request.Path);
                    break;
            }

            if (context.Response.HasStarted)
            {
                // اگر Response شروع شده دیگر نمی‌توان status/code را تغییر داد
                _logger.LogError("Response already started; exception {ExceptionType} could not be returned to client",
                    exception.GetType().Name);
                throw exception;
            }

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json; charset=utf-8";

            var response = new Dictionary<string, object?>
            {
                ["result"] = null,
                ["statusCode"] = statusCode,
                ["message"] = message
            };
            if (errors is not null)
                response["errors"] = errors;

            // در محیط Development جزئیات فنی (Type و StackTrace) اضافه می‌شود
            if (_environment.IsDevelopment())
            {
                response["exceptionType"] = exception.GetType().Name;
                response["stackTrace"] = exception.StackTrace;
                if (exception.InnerException is not null)
                    response["innerException"] = exception.InnerException.Message;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }

        private void LogByLevel(LogLevel level, AppException exception, HttpContext context)
        {
            _logger.Log(level, exception, "{ExceptionType} on {Method} {Path}: {Message}",
                exception.GetType().Name, context.Request.Method, context.Request.Path,
                exception.Message);
        }
    }

    /// <summary>ثبت Middleware به صورت متد الحاقی.</summary>
    public static class GlobalExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
