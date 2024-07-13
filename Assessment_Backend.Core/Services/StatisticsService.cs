
namespace Assessment_Backend.Core.Services
{
    public class StatisticsService: IStatisticsService
    {
        #region Constructor
        private readonly AssessmentDbContext _context;
        private readonly ILogger<StatisticsService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public StatisticsService(AssessmentDbContext context, ILogger<StatisticsService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion


        public async Task<OutPutModel<StudentStatisticsDTO>> GetStudentStatisticsAsync()
        {
            try
            {
                var studentId = _httpContextAccessor.GetStudentId();

                if (studentId is 0)
                {
                    return new OutPutModel<StudentStatisticsDTO>
                    {

                        Result = null,
                        StatusCode = 403,
                        Message = "لطفاً مجدداً وارد حساب کاربری خود شوید."
                    };
                }

                int countJoinClass=await _context.CourseEnrollments
                    .AsNoTracking()
                    .CountAsync(ce=> ce.StudentId==studentId);
                int countDutyDone = await _context.AssignmentSubmissions
                    .AsNoTracking()
                    .CountAsync(a=> a.StudentId==studentId);

                int totalAssignments = await _context.Assessments
                    .AsNoTracking()
                    .CountAsync(a => a.Course.CourseEnrollments
                    .Any(ce => ce.StudentId == studentId));

                int countDutyUncompleted = Math.Max(0, totalAssignments - countDutyDone);

                var StudentStatistics = new StudentStatisticsDTO()
                {
                     ClassCount = countJoinClass,
                      CountDutyDone=countDutyDone,
                       CountDutyUncompleted=countDutyUncompleted
                };



                return new OutPutModel<StudentStatisticsDTO>
                {
                     StatusCode= 200,
                     Message="",
                      Result=StudentStatistics,
                };

            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, ex);
                return new OutPutModel<StudentStatisticsDTO>
                {
                    Result = null,
                    StatusCode = 500,
                    Message = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید",
                };
            }
        }

        public async Task<OutPutModel<TeacherStatisticsDTO>> GetTeacherStatisticsAsync()
        {
            try
            {
                int teacherId = _httpContextAccessor.GetTeacherId();

                if (teacherId is 0)
                {
                    return new OutPutModel<TeacherStatisticsDTO>
                    {

                        Result = null,
                        StatusCode = 401,
                        Message = "لطفاً مجدداً وارد حساب کاربری خود شوید."
                    };
                }

                int classCount=_context.Courses
                    .AsNoTracking()
                    .Count(c=>c.TeacherId==teacherId);
                
                int assessmentCount = await _context.Assessments
                                             .AsNoTracking()
                                             .CountAsync(a => a.Course.TeacherId == teacherId);


                var TeacherStatistics = new TeacherStatisticsDTO
                {
                    ClassCount = classCount,
                    AssessmentCount = assessmentCount
                }; 

                return new OutPutModel<TeacherStatisticsDTO> 
                {
                    StatusCode = 200,
                    Message="",
                    Result = TeacherStatistics
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<TeacherStatisticsDTO>
                {
                    Result = null,
                    StatusCode = 500,
                    Message = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید",
                };
            }
        }
    }
}
