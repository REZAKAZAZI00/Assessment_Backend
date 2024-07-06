namespace Assessment_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentController : ControllerBase
    {
        #region Constructor
        private readonly IAssessmentService _assessmentService;
        public AssessmentController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }
        #endregion


        [HttpPost("CreateAssessment")]

        public async Task<ActionResult<OutPutModel<CourseDTO>>> CreateAssessment(CreateAssessmentDTO assessmentDTO)
        {
            var result = await _assessmentService.CreateAssessmentAsync(assessmentDTO);
            return result;
        }

     

       

        [HttpPut("UpdateAssessment")]

        public async Task<ActionResult<OutPutModel<CourseDTO>>> UpdateAssessment(UpdateAssessmentDTO assessmentDTO)
        {
            var result = await _assessmentService.UpdateAssessmentAsync(assessmentDTO);
            return result;
        }

        [HttpDelete("DeleteAssessment")]
        public async Task<ActionResult<OutPutModel<CourseDTO>>> DeleteAssessment(DeleteAssessmentDTO assessmentDTO)
        {
            var result = await _assessmentService.DeleteAssessmentAsync(assessmentDTO);

            return result;
        }
        /// <summary>
        /// برای ارسال تمرین 
        /// </summary>
        /// <param name="submissionDTO"></param>
        /// <returns></returns>
        [HttpPost("AssignmentSubmission")]
        public async Task<ActionResult<OutPutModel<AssessmentDTO>>> AssignmentSubmission(AssignmentSubmissionDTO submissionDTO)
        {
            var result = await _assessmentService.AssignmentSubmissionAsync(submissionDTO);
            return result;
        }

        /// <summary>
        /// برای ثبت نمره
        /// </summary>
        /// <param name="scoreRegistrationDTO"></param>
        /// <returns></returns>
        [HttpPost("ScoreRegistration")]
        public async Task<ActionResult<OutPutModel<AssessmentDTO>>> ScoreRegistration(ScoreRegistrationDTO scoreRegistrationDTO)
        {
            var result = await _assessmentService.ScoreRegistrationAsync(scoreRegistrationDTO);
            return result;
        }
        /// <summary>
        /// برای دریافت تمرین های ارسال شده 
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <returns></returns>

        [HttpGet("AssignmentSubmissions/{assignmentId}")]
        public async Task<ActionResult<OutPutModel<List<SubmittedAssignmentDTO>>>> AssignmentSubmissions(int assignmentId)
        {
            var result = await _assessmentService.GetAssignmentSubmissionsByIdAsync(assignmentId);
            return result;
        }

        /// <summary>
        /// گرفتن همه تکلیف  ها
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllAssignments")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<List<AssessmentDTO>>>> GetAssignments()
        {
            var result = await _assessmentService.GetAllAssignmentAsync();
            return result;
        }

    }
}
