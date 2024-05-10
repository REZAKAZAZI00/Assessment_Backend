namespace Assessment_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseServies;
        public CourseController(ICourseService courseServies)
        {
            _courseServies = courseServies;
        }

        [HttpGet("Terms")]
        public async Task<ActionResult<OutPutModel<List<TermDTO>>>> GetAllTerm()
        {
            var result = await _courseServies.GetAllTermAsync();

            return result;
        }


        [HttpGet("Course")]
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

        [HttpDelete("DeteteCourse")]
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

    }
}
