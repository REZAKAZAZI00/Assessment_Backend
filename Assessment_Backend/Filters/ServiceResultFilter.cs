using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Assessment_Backend.Filters
{
    /// <summary>
    /// نتیجه سرویس‌ها (OutPutModel/ServiceResult) را به HTTP Status واقعی تبدیل می‌کند
    /// تا Controller فقط «return result;» بنویسد و کد Status در Body گم نشود.
    /// </summary>
    public class ServiceResultFilter : IAsyncAlwaysRunResultFilter
    {
        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult &&
                (objectResult.Value is OutPutModel<object> || objectResult.Value?.GetType() is
                {
                    IsGenericType: true,
                    Name: "OutPutModel`1"
                }))
            {
                int status = objectResult.Value switch
                {
                    OutPutModel<object> o => o.StatusCode,
                    _ => (int)(objectResult.Value!.GetType()
                        .GetProperty("StatusCode")!
                        .GetValue(objectResult.Value) ?? 200)
                };

                if (status >= 100 && status < 600)
                    objectResult.StatusCode = status;
            }

            return next();
        }
    }
}
