namespace Assessment_Backend.Core.Exceptions
{
    /// <summary>
    /// کلاس پایه برای تمام Exception های دامنه برنامه.
    /// به جای try/catch تکراری در سرویس‌ها، سرویس‌ها این Exception ها را پرتاب می‌کنند
    /// و GlobalExceptionHandlingMiddleware آن‌ها را به Response مناسب تبدیل می‌کند.
    /// </summary>
    public abstract class AppException : Exception
    {
        /// <summary>کد HTTP که باید به کلاینت برگردد.</summary>
        public int StatusCode { get; }

        /// <summary>پیامی که به کلاینت نمایش داده می‌شود (بدون اطلاعات حساس).</summary>
        public string UserMessage { get; }

        /// <summary>سطح لاگ برای Middleware.</summary>
        public LogLevel LogLevel { get; }

        protected AppException(string userMessage, int statusCode,
            LogLevel logLevel = LogLevel.Warning,
            Exception? innerException = null)
            : base(userMessage, innerException)
        {
            UserMessage = userMessage;
            StatusCode = statusCode;
            LogLevel = logLevel;
        }
    }

    /// <summary>خطای اعتبارسنجی - 400 Bad Request.</summary>
    public class ValidationAppException : AppException
    {
        /// <summary>خطاهای هر فیلد به صورت دیکشنری field -> errors.</summary>
        public Dictionary<string, string[]> Errors { get; }

        public ValidationAppException(Dictionary<string, string[]>? errors = null, string? message = null)
            : base(message ?? "اطلاعات ارسال شده معتبر نمی باشد.", 400)
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }
    }

    /// <summary>عدم احراز هویت - 401 Unauthorized.</summary>
    public class UnauthorizedAppException : AppException
    {
        public UnauthorizedAppException(string message = "لطفاً مجدداً وارد حساب کاربری خود شوید.")
            : base(message, 401) { }
    }

    /// <summary>عدم دسترسی - 403 Forbidden.</summary>
    public class ForbiddenAppException : AppException
    {
        public ForbiddenAppException(string message = "شما اجازه دسترسی به این بخش را ندارید.")
            : base(message, 403) { }
    }

    /// <summary>منبع پیدا نشد - 404 Not Found.</summary>
    public class NotFoundAppException : AppException
    {
        public NotFoundAppException(string message)
            : base(message, 404) { }
    }

    /// <summary>تضاد در وضعیت منبع - 409 Conflict.</summary>
    public class ConflictAppException : AppException
    {
        public ConflictAppException(string message)
            : base(message, 409) { }
    }

    /// <summary>خطای منطق دامنه با کد HTTP دلخواه.</summary>
    public class BusinessException : AppException
    {
        public BusinessException(string message, int statusCode = 400)
            : base(message, statusCode) { }
    }
}
