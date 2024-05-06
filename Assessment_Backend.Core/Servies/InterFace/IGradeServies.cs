namespace Assessment_Backend.Core.Servies.InterFace
{
    public interface IGradeServies
    {
        Task<OutPutModel<List<GradeDTO>>> GetAllGradesAsync();


    }
}
