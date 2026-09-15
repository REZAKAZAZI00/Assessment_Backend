namespace Assessment_Backend.Core.Security
{
    /// <summary>
    /// نتیجه استاندارد لایه سرویس. قبلاً OutPutModel&lt;T&gt; بود؛
    /// الان لایه سرویس یا Result موفق برمی‌گرداند یا Exception دامنه پرتاب می‌کند
    /// (که توسط GlobalExceptionHandlingMiddleware به پاسخ HTTP تبدیل می‌شود).
    /// </summary>
    public class ServiceResult<T>
    {
        public T? Result { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public static ServiceResult<T> Success(T? result, int statusCode = 200, string message = "")
            => new() { Result = result, StatusCode = statusCode, Message = message };
    }
}
