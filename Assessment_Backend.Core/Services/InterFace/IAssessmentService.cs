namespace Assessment_Backend.Core.Services.InterFace
{
    public interface IAssessmentService
    {

        Task<OutPutModel<CourseDTO>> CreateAssessmentAsync(CreateAssessmentDTO assessmentDTO);

        Task<OutPutModel<CourseDTO>> UpdateAssessmentAsync(UpdateAssessmentDTO assessmentDTO);

        Task<OutPutModel<CourseDTO>> DeleteAssessmentAsync(DeleteAssessmentDTO assessmentDTO);


        Task<CourseDTO> GetCourseByIdAsync(int courseId);

        Task<AssessmentDTO> GetAssignmentByIdAsync(int assessmentId);

        Task<OutPutModel<List<SubmittedAssignmentDTO>>> GetAssignmentSubmissionsByIdAsync(int assignmentId);

        Task<OutPutModel<AssessmentDTO>> AssignmentSubmissionAsync(AssignmentSubmissionDTO assessmentSubmissionDTO);

        Task<OutPutModel<AssessmentDTO>> ScoreRegistrationAsync(ScoreRegistrationDTO scoreRegistrationDTO);


    }
}
