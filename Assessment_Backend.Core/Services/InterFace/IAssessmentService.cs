namespace Assessment_Backend.Core.Services.InterFace
{
    public interface IAssessmentService
    {

        Task<OutPutModel<CourseDTO>> CreateAssessmentAsync(CreateAssessmentDTO assessmentDTO);

        Task<OutPutModel<CourseDTO>> UpdateAssessmentAsync(UpdateAssessmentDTO assessmentDTO);

        Task<CourseDTO> GetCourseByIdAsync(int courseId);
    }
}
