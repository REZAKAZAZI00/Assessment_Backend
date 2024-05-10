namespace Assessment_Backend.Core.Servies
{
    public class GradeService : IGradeService
    {
        private readonly AssessmentDbContext _context;
        private readonly ILogger<GradeService> _logger;
        public GradeService(AssessmentDbContext context,ILogger<GradeService> logger)
        {
           _context = context;
            _logger = logger;
        }
        public async Task<OutPutModel<List<GradeDTO>>> GetAllGradesAsync()
        {
			try
			{
				var grades=await _context.Grades
                    .Select(g=> new GradeDTO
                    {
                         GradeId=g.GradeId,
                         Title=g.Title,

                    }).ToListAsync();

                return new OutPutModel<List<GradeDTO>>
                {
                     Result = grades,
                     StatusCode = 200,
                     Message=""  
                };
			}
			catch (Exception ex)
			{
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<List<GradeDTO>> 
                {
                    Result=null,
                    StatusCode = 500,
                    Message=ex.Message
                };
			}
        }
    }
}
