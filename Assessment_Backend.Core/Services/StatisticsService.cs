namespace Assessment_Backend.Core.Services
{
    public class StatisticsService : IStatisticsService
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
            var studentId = _httpContextAccessor.GetStudentId();
            if (studentId is 0)
                throw new UnauthorizedAppException("لطفاً مجدداً وارد حساب کاربری خود شوید.");

            int countJoinClass = await _context.CourseEnrollments
                .AsNoTracking()
                .CountAsync(ce => ce.StudentId == studentId);

            int countDutyDone = await _context.AssignmentSubmissions
                .AsNoTracking()
                .CountAsync(a => a.StudentId == studentId);

            int totalAssignments = await _context.Assessments
                .AsNoTracking()
                .CountAsync(a => a.Course.CourseEnrollments
                .Any(ce => ce.StudentId == studentId));

            int countDutyUncompleted = Math.Max(0, totalAssignments - countDutyDone);

            var studentStatistics = new StudentStatisticsDTO()
            {
                ClassCount = countJoinClass,
                CountDutyDone = countDutyDone,
                CountDutyUncompleted = countDutyUncompleted
            };

            return new OutPutModel<StudentStatisticsDTO>
            {
                StatusCode = 200,
                Message = "",
                Result = studentStatistics,
            };
        }

        public async Task<OutPutModel<TeacherStatisticsDTO>> GetTeacherStatisticsAsync()
        {
            int teacherId = _httpContextAccessor.GetTeacherId();
            if (teacherId is 0)
                throw new UnauthorizedAppException("لطفاً مجدداً وارد حساب کاربری خود شوید.");

            int classCount = _context.Courses
                .AsNoTracking()
                .Count(c => c.TeacherId == teacherId);

            int assessmentCount = await _context.Assessments
                .AsNoTracking()
                .CountAsync(a => a.Course.TeacherId == teacherId);

            var teacherStatistics = new TeacherStatisticsDTO
            {
                ClassCount = classCount,
                AssessmentCount = assessmentCount
            };

            return new OutPutModel<TeacherStatisticsDTO>
            {
                StatusCode = 200,
                Message = "",
                Result = teacherStatistics
            };
        }
    }
}
