namespace Assessment_Backend.Controllers
{
    [Route("api/courses")]
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

        /// <summary>
        /// گرفتن ترم
        /// </summary>
        /// <returns></returns>
        [HttpGet("terms")]
        public async Task<ActionResult<OutPutModel<List<TermDTO>>>> GetAllTerm()
        {
            var result = await _courseServies.GetAllTermAsync();

            return result;
        }

        /// <summary>
        /// گرفتن کلاس های من 
        /// </summary>
        /// <returns></returns>
        [HttpGet("course")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<List<CourseDTO>>>> GetAllCourse() 
        {
            return await _courseServies.GetAllCourseAsync();
        }

        /// <summary>
        /// گرفتن درس با استفاده از شناسه کلاس
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        [HttpGet("course/{courseId}")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<CourseDTO>>> GetCourseById(int courseId)
        {
            return await _courseServies.GetCourseByCourseIdAsync(courseId);
        }

        [HttpGet("course/{link}")]
        public async Task<ActionResult<OutPutModel<CourseDTO>>> GetCourseBy(string link)
        {
            return await _courseServies.GetCourseByCourseLinkAsync(link);
        }

        /// <summary>
        /// ایجاد کلاس توسط استاد
        /// </summary>
        /// <param name="courseDTO"></param>
        /// <returns></returns>
        [HttpPost("course")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<List<CourseDTO>>>> CreateCourse(CreateCourseDTO courseDTO)
        {
            var result = await _courseServies.CreateCourseAsync(courseDTO);
            return result;
        }

        /// <summary>
        /// حذف کلاس توسط استاد
        /// </summary>
        /// <param name="courseDTO"></param>
        /// <returns></returns>
        [HttpPost("deletecourse")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<List<CourseDTO>>>> DeleteCourse(DeleteCourseDTO courseDTO)
        {
            var result = await _courseServies.DeleteCourseAsync(courseDTO);
            return result;
        }


        /// <summary>
        /// ویرایش کردن اطلاعات کلاس توسط استاد
        /// </summary>
        /// <param name="courseDTO"></param>
        /// <returns></returns>
        [HttpPut("course")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<List<CourseDTO>>>> UpdateCourse(UpdateCourseDTO courseDTO)
        {
            var result = await _courseServies.UpdateCourseAsync(courseDTO);
            return result;
        }

        /// <summary>
        /// عضو شدن دانشجو به کلاس
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("joinclass")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<List<CourseDTO>>>> JoinClass(JoinClassDTO model)
        {
            var result = await _courseServies.JoinClassAsync(model);
               
            return result;
        }

        /// <summary>
        /// ترک کلاس توسط دانشجو
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete("leavingclass")]
        [Authorize]
        public async Task<ActionResult<OutPutModel<List<CourseDTO>>>> LeavingClass(LeavingClassDTO model)
        {
            var result = await _courseServies.LeavingClassAsync(model);

            return result;
        }


    }
}
