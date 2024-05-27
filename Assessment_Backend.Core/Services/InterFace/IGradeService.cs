namespace Assessment_Backend.Core.Servies.InterFace
{
    public interface IGradeService
    {
        Task<OutPutModel<List<GradeDTO>>> GetAllGradesAsync();


    }
}
