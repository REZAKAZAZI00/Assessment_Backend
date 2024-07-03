namespace Assessment_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeController : ControllerBase
    {
        #region Constructor
        private readonly IGradeService _gradesServies;
        public GradeController(IGradeService gradeServies)
        {
            _gradesServies = gradeServies;
        }

        #endregion


        [HttpGet("GetAllGrade")]
        public async Task<ActionResult<OutPutModel<List<GradeDTO>>>> GetAllGrade()
        {
            var result=await _gradesServies.GetAllGradesAsync();

            return result;
        } 
    }
}
