namespace Assessment_Backend.Controllers
{
    [Route("api/[controller]")]
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


        [HttpGet("StudentStatistics")]
        [Authorize]
        public async  Task<ActionResult<OutPutModel<StudentStatisticsDTO >>> GetStudentStatistics() 
        {
        
           var result=await _statisticsService.GetStudentStatisticsAsync();

            return result;
        }

        [HttpGet("TeacherStatistics")]
        [Authorize] 
        public async Task<ActionResult<OutPutModel<TeacherStatisticsDTO>>> GetTeacherStatistics()
        {

            var result = await _statisticsService.GetTeacherStatisticsAsync();

            return result;
        }
    }
}
