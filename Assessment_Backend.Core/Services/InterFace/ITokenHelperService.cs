namespace Assessment_Backend.Core.Servies.InterFace
{
    public interface ITokenHelperService
    {    
        string GenerateToken<T>(User user, T entity) where T : class;

    }
}
