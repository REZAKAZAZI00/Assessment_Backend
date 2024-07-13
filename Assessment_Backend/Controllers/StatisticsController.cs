namespace Assessment_Backend.Controllers
{
    [Route("api/statistics")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        #region Constructor
        private readonly IStatisticsService _statisticsService;
        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }
        #endregion

        /// <summary>
        /// گرفتن امار استاد
        /// </summary>
        /// <returns></returns>
        [HttpGet("studentstatistics")]
        [Authorize]
        public async  Task<ActionResult<OutPutModel<StudentStatisticsDTO >>> GetStudentStatistics() 
        {
        
           var result=await _statisticsService.GetStudentStatisticsAsync();

            return result;
        }
        
        
        /// <summary>
        /// گرفتن امار استاد
        /// </summary>
        /// <returns></returns>
        [HttpGet("teacherstatistics")]
        [Authorize] 
        public async Task<ActionResult<OutPutModel<TeacherStatisticsDTO>>> GetTeacherStatistics()
        {

            var result = await _statisticsService.GetTeacherStatisticsAsync();

            return result;
        }
    }
}
