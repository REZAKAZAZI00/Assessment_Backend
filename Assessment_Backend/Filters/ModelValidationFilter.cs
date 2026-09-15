using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Assessment_Backend.Filters
{
    /// <summary>
    /// خطاهای ModelState را به جای رفتار پیش‌فرض ApiController به شکل استاندارد
    /// { result, statusCode, message, errors } برمی‌گرداند تا با خطاهای
    /// GlobalExceptionHandlingMiddleware هم‌شکل باشند.
    /// </summary>
    public class ModelValidationFilter : IAsyncActionFilter, IOrderedFilter
    {
        // بعد از ApiController.ModelStateInvalidFilter اجرا شود
        // (ModelStateInvalidFilter Order = -2000)
        public int Order => -1999;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // ApiController خطای 400 خودش تولید کرده؟ اجازه بده پیش برود اما بازنویسی می‌کنیم
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(kv => kv.Value is { Errors.Count: > 0 })
                    .ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value!.Errors
                            .Where(e => !string.IsNullOrEmpty(e.ErrorMessage))
                            .Select(e => e.ErrorMessage)
                            .ToArray());

                var status = context.Result is BadRequestObjectResult ? 400 : 422;
                var message = "اطلاعات ارسال شده معتبر نمی باشد.";

                var response = new Dictionary<string, object?>
                {
                    ["result"] = null,
                    ["statusCode"] = status,
                    ["message"] = message,
                    ["errors"] = errors
                };

                context.Result = new ObjectResult(response) { StatusCode = status };
                return; // action اجرا نمی‌شود
            }

            await next();
        }
    }
}
