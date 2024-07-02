
namespace Assessment_Backend.Core.Services.InterFace
{
    public interface IStatisticsService
    {

        Task<OutPutModel<TeacherStatisticsDTO>>   GetTeacherStatisticsAsync();

        Task<OutPutModel<StudentStatisticsDTO>> GetStudentStatisticsAsync();
    }
}
