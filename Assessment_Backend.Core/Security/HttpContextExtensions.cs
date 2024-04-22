

public static class HttpContextExtensions
{
    public static int GetUserId(this IHttpContextAccessor httpContextAccessor)
    {
        try
        {
            int userId;
            //if (!int.TryParse(httpContextAccessor.HttpContext.User.FindFirstValue("userId"), out userId))
            //{
            //    //_logger.LogError("Failed to parse userId. User is not authenticated or userId is missing.");
            //    return 0;
            //}
            return 1;
        }
        catch (Exception)
        {
            return 0;
        }

    }
}