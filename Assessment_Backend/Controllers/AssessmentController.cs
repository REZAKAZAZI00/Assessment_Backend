namespace Assessment_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentController : ControllerBase
    {
        private readonly IAssessmentService _assessmentService;
        public AssessmentController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }

        [HttpPost("CreateAssessment")]
        
        public async Task<ActionResult<OutPutModel<CourseDTO>>> CreateAssessment(CreateAssessmentDTO courseDTO)
        {
            var result = await _assessmentService.CreateAssessmentAsync(courseDTO);
            return result;
        }

        [HttpPut("UpdateAssessment")]

        public async Task<ActionResult<OutPutModel<CourseDTO>>> UpdateAssessment(UpdateAssessmentDTO courseDTO)
        {
            var result = await _assessmentService.UpdateAssessmentAsync(courseDTO);
            return result;
        }
    }
}
