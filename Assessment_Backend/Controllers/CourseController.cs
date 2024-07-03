namespace Assessment_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {

        #region Constructor
        private readonly ICourseService _courseServies;
        public CourseController(ICourseService courseServies)
        {
            _courseServies = courseServies;
        }
        #endregion


        [HttpGet("Terms")]
        public async Task<ActionResult<OutPutModel<List<TermDTO>>>> GetAllTerm()
        {
            var result = await _courseServies.GetAllTermAsync();

            return result;
        }


        [HttpGet("Course")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<List<CourseDTO>>>> GetAllCourse() 
        {
            return await _courseServies.GetAllCourseAsync();
        }

        [HttpPost("CreateCourse")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<List<CourseDTO>>>> CreateCourse(CreateCourseDTO courseDTO)
        {
            var result = await _courseServies.CreateCourseAsync(courseDTO);
            return result;
        }

        [HttpPost("DeteteCourse")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<List<CourseDTO>>>> DeleteCourse(DeleteCourseDTO courseDTO)
        {
            var result = await _courseServies.DeleteCourseAsync(courseDTO);
            return result;
        }

        [HttpPut("UpdateCourse")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<List<CourseDTO>>>> UpdateCourse(UpdateCourseDTO courseDTO)
        {
            var result = await _courseServies.UpdateCourseAsync(courseDTO);
            return result;
        }
        [HttpPost("JoinClass")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<List<CourseDTO>>>> JoinClass(JoinClassDTO model)
        {
            var result = await _courseServies.JoinClassAsync(model);
               
            return result;
        }


        [HttpDelete("LeavingClass")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<List<CourseDTO>>>> LeavingClass(LeavingClassDTO model)
        {
            var result = await _courseServies.LeavingClassAsync(model);

            return result;
        }


    }
}
