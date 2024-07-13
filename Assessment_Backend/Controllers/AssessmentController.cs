namespace Assessment_Backend.Controllers
{
    [Route("api/assessments")]
    [ApiController]
    public class AssessmentController : ControllerBase
    {
        #region Constructor
        private readonly IAssessmentService _assessmentService;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assessmentService"></param>
        public AssessmentController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }
        #endregion

        /// <summary>
        /// ایجاد تمرین توسط استاد
        /// </summary>
        /// <param name="assessmentDTO"></param>
        /// <returns></returns>
        [HttpPost("assessment")]

        public async Task<ActionResult<OutPutModel<CourseDTO>>> CreateAssessment(CreateAssessmentDTO assessmentDTO)
        {
            var result = await _assessmentService.CreateAssessmentAsync(assessmentDTO);

            if(result.StatusCode == 403){

                return Forbid();
            }
            return result;
        }

     

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assessmentDTO"></param>
        /// <returns></returns>
        [HttpPut("assessment")]

        public async Task<ActionResult<OutPutModel<CourseDTO>>> UpdateAssessment(UpdateAssessmentDTO assessmentDTO)
        {
            var result = await _assessmentService.UpdateAssessmentAsync(assessmentDTO);
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assessmentDTO"></param>
        /// <returns></returns>
        [HttpDelete("assessment")]
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
        [HttpPost("assignmentSubmission")]
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
        [HttpPost("scoreRegistration")]
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

        [HttpGet("assignmentSubmissions/{assignmentId}")]
        public async Task<ActionResult<OutPutModel<List<SubmittedAssignmentDTO>>>> AssignmentSubmissions(int assignmentId)
        {
            var result = await _assessmentService.GetAssignmentSubmissionsByIdAsync(assignmentId);
            return result;
        }

        /// <summary>
        /// گرفتن همه تکلیف  ها
        /// </summary>
        /// <returns></returns>
        [HttpGet("assignments")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<List<AssessmentDTO>>>> GetAssignments()
        {
            var result = await _assessmentService.GetAllAssignmentAsync();
            return result;
        }


        /// <summary>
        /// برای کارنامه  
        /// </summary>
        /// <returns></returns>
        [HttpGet("report")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<ReportDTO>>> GetReport()
        {
            var result = await _assessmentService.GetReportAsync();
            return result;
        }
    }
}
