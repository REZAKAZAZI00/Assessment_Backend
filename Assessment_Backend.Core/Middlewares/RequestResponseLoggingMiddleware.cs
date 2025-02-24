namespace Assessment_Backend.Core.Middlewares;
public class RequestResponseLoggingMiddleware
{

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // لاگ کردن اطلاعات Request
        var request = await FormatRequest(context.Request);
        _logger.LogInformation("Request: {Request}", request);

        // کپی کردن Response برای لاگ کردن
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        // اجرای Middleware بعدی
        await _next(context);

        // لاگ کردن اطلاعات Response
        var response = await FormatResponse(context.Response);
        _logger.LogInformation("Response: {Response}", response);

        // بازگرداندن Response به جریان اصلی
        await responseBody.CopyToAsync(originalBodyStream);
    }

    private async Task<string> FormatRequest(HttpRequest request)
    {
       // request.EnableBuffering();
        var body = request.Body;

        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        var bodyAsText = Encoding.UTF8.GetString(buffer);
        request.Body.Position = 0;

        return $"Method: {request.Method}, Path: {request.Path}, QueryString: {request.QueryString}, Body: {bodyAsText}";
    }

    private async Task<string> FormatResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return $"StatusCode: {response.StatusCode}, Body: {text}";
    }
}
