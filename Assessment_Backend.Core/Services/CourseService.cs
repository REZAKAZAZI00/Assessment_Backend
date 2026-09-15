namespace Assessment_Backend.Core.Servies
{
    public class CourseService : ICourseService
    {
        #region Constructor
        private readonly AssessmentDbContext _context;
        private readonly ILogger<CourseService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CourseService(AssessmentDbContext context, ILogger<CourseService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        public async Task<OutPutModel<List<CourseDTO>>> CreateCourseAsync(CreateCourseDTO model)
        {
            ValidateModel.ValidateOrThrow(model);

            int teacherId = _httpContextAccessor.GetTeacherId();
            if (teacherId is 0)
                throw new UnauthorizedAppException();

            var newCourse = new Course()
            {
                Title = model.Title,
                CountMembers = model.CountMembers,
                Description = model.Description,
                TeacherId = teacherId,
                Link = NameGenerator.GenerateShareLink(),
                TermId = model.TermId,
            };

            await _context.Courses.AddAsync(newCourse);
            await _context.SaveChangesAsync();

            return new OutPutModel<List<CourseDTO>>
            {
                StatusCode = 200,
                Message = "درس با موفقیت ثبت شد.",
                Result = await GetCourseAsync()
            };
        }

        public async Task<OutPutModel<List<CourseDTO>>> DeleteCourseAsync(DeleteCourseDTO model)
        {
            ValidateModel.ValidateOrThrow(model);

            int teacherId = _httpContextAccessor.GetTeacherId();
            if (teacherId is 0)
                throw new UnauthorizedAppException();

            var existingCourse = await _context.Courses
                .SingleOrDefaultAsync(c => c.CourseId == model.CourseId && c.TeacherId == teacherId);

            if (existingCourse is null)
                throw new NotFoundAppException("درس انتخاب شده حذف نشد مجدد تلاش کنید.");

            _context.Courses.Remove(existingCourse);
            await _context.SaveChangesAsync();

            return new OutPutModel<List<CourseDTO>>
            {
                StatusCode = 200,
                Message = " درس با موفقیت حذف شد. ",
                Result = await GetCourseAsync(),
            };
        }

        public async Task<OutPutModel<List<CourseDTO>>> GetAllCourseAsync()
        {
            return new OutPutModel<List<CourseDTO>>
            {
                Result = await GetCourseAsync(),
                Message = "",
                StatusCode = 200,
            };
        }

        public async Task<OutPutModel<List<TermDTO>>> GetAllTermAsync()
        {
            var term = await _context.Terms
                .AsNoTracking()
                .Select(t => new TermDTO
                {
                    TermId = t.TermId,
                    Title = t.Title,
                })
                .ToListAsync();

            return new OutPutModel<List<TermDTO>>
            {
                StatusCode = 200,
                Result = term,
                Message = ""
            };
        }

        public async Task<List<CourseDTO>> GetCourseAsync()
        {
            int teacherId = _httpContextAccessor.GetTeacherId();
            int studentId = _httpContextAccessor.GetStudentId();

            var query = _context.Courses
                .AsNoTracking()
                .Include(c => c.Term)
                .Include(c => c.Teacher)
                .Include(c => c.Assessments)
                .Include(c => c.CourseEnrollments)
                .ThenInclude(e => e.Student)
                .Where(c => teacherId != 0 ? c.TeacherId == teacherId : c.CourseEnrollments.Any(e => e.StudentId == studentId))
                .Select(c => new CourseDTO
                {
                    CountMembers = c.CountMembers,
                    Description = c.Description,
                    Link = c.Link,
                    extant = (c.CountMembers - _context.CourseEnrollments.Count(ce => ce.CourseId == c.CourseId)),
                    Title = c.Title,
                    CourseId = c.CourseId,
                    Term = c.Term.Title,
                    TermId = c.TermId,
                    TeacherName = c.Teacher.Name + " " + c.Teacher.family,
                    Student = teacherId > 0 ? c.CourseEnrollments.Select(e => new StudentDTO
                    {
                        StudentId = e.StudentId,
                        Name = e.Student.Name,
                        family = e.Student.family,
                        PhoneNumber = e.Student.PhoneNumber,
                        Email = e.Student.Email
                    }).ToList() : null,
                    Assessments = c.Assessments
                        .Select(a => new AssessmentDTO
                        {
                            AssessmentId = a.AssessmentId,
                            StartDate = a.StartDate,
                            CourseId = a.CourseId,
                            Description = a.Description,
                            EndDate = a.EndDate,
                            Title = a.Title,
                            PenaltyRule = a.PenaltyRule
                        })
                        .ToList()
                });

            return await query.ToListAsync();
        }

        public async Task<OutPutModel<CourseDTO>> GetCourseByCourseIdAsync(int courseId)
        {
            if (courseId == 0)
                throw new BusinessException("شناسه درس نمی‌تواند خالی باشد.", 400);

            int teacherId = _httpContextAccessor.GetTeacherId();
            int studentId = _httpContextAccessor.GetStudentId();

            var query = _context.Courses
                .Where(c => c.CourseId == courseId)
                .Include(c => c.Term)
                .Include(c => c.Teacher)
                .Include(c => c.Assessments)
                .Include(c => c.CourseEnrollments).ThenInclude(e => e.Student)
                .Where(c => teacherId != 0 ? c.TeacherId == teacherId : c.CourseEnrollments.Any(e => e.StudentId == studentId))
                .Select(c => new CourseDTO
                {
                    extant = (c.CountMembers - _context.CourseEnrollments.Count(ce => ce.CourseId == c.CourseId)),
                    CountMembers = c.CountMembers,
                    Description = c.Description,
                    Link = c.Link,
                    Title = c.Title,
                    CourseId = c.CourseId,
                    Term = c.Term.Title,
                    TermId = c.TermId,
                    TeacherName = c.Teacher.Name + " " + c.Teacher.family,
                    Student = teacherId > 0 ? c.CourseEnrollments.Select(e => new StudentDTO
                    {
                        StudentId = e.StudentId,
                        Name = e.Student.Name,
                        family = e.Student.family,
                        PhoneNumber = e.Student.PhoneNumber,
                        Email = e.Student.Email
                    }).ToList() : null,
                    Assessments = c.Assessments
                        .Select(a => new AssessmentDTO
                        {
                            AssessmentId = a.AssessmentId,
                            StartDate = a.StartDate,
                            CourseId = a.CourseId,
                            FileName = a.FileName,
                            Description = a.Description,
                            EndDate = a.EndDate,
                            Title = a.Title,
                            PenaltyRule = a.PenaltyRule
                        })
                        .ToList()
                });

            var course = await query.SingleOrDefaultAsync();

            if (course is null)
                throw new NotFoundAppException("درس پیدا نشد.");

            return new OutPutModel<CourseDTO>
            {
                Message = "",
                StatusCode = 200,
                Result = course
            };
        }

        public async Task<OutPutModel<CourseDTO>> GetCourseByCourseLinkAsync(string link)
        {
            if (link is null)
                throw new NotFoundAppException("کلاس مورد نظر پیدا نشد.");

            var course = await _context.Courses
                .Where(c => c.Link == link)
                .Include(t => t.Teacher)
                .Include(t => t.Term)
                .Select(c => new CourseDTO
                {
                    Link = c.Link,
                    CourseId = c.CourseId,
                    Term = c.Term.Title,
                    TeacherName = c.Teacher.Name + "" + c.Teacher.family,
                    Description = c.Description,
                    CountMembers = c.CountMembers,
                    Title = c.Title,
                    TermId = c.TermId,
                    extant = (c.CountMembers - _context.CourseEnrollments.Count(ce => ce.CourseId == c.CourseId))
                })
                .SingleOrDefaultAsync();

            return new OutPutModel<CourseDTO>
            {
                StatusCode = 200,
                Message = "",
                Result = course
            };
        }

        public async Task<OutPutModel<List<CourseDTO>>> JoinClassAsync(JoinClassDTO model)
        {
            ValidateModel.ValidateOrThrow(model);

            int studentId = _httpContextAccessor.GetStudentId();
            if (studentId is 0)
                throw new UnauthorizedAppException();

            var course = await _context.Courses.SingleOrDefaultAsync(c => c.Link == model.ClassLink);
            if (course is null)
                throw new NotFoundAppException("درسی پیدا نشد.");

            var existingEnrollment = await _context.CourseEnrollments
                      .SingleOrDefaultAsync(ce =>
                      ce.CourseId == course.CourseId
                      && ce.StudentId == studentId);

            if (existingEnrollment != null)
                throw new ConflictAppException("شما قبلاً در این درس عضو شده‌اید.");

            var count = _context.CourseEnrollments
                .Where(ce => ce.CourseId == course.CourseId).Count();

            if (count >= course.CountMembers)
                throw new ConflictAppException("ظرفیت کلاس پر شده است.");

            await _context.CourseEnrollments.AddAsync(new CourseEnrollment()
            {
                DateTime = DateTime.Now,
                CourseId = course.CourseId,
                StudentId = studentId,
            });
            await _context.SaveChangesAsync();

            return new OutPutModel<List<CourseDTO>>
            {
                Message = "عضویت به کلاس با موفقیت انجام شد.",
                StatusCode = 200,
                Result = await GetCourseAsync()
            };
        }

        public async Task<OutPutModel<List<CourseDTO>>> LeavingClassAsync(LeavingClassDTO model)
        {
            ValidateModel.ValidateOrThrow(model);

            int studentId = _httpContextAccessor.GetStudentId();
            if (studentId is 0)
                throw new UnauthorizedAppException();

            var existingEnrollment = await _context.CourseEnrollments
                .SingleOrDefaultAsync(c => c.CourseId == model.CourseId && c.StudentId == studentId);

            if (existingEnrollment is null)
                throw new NotFoundAppException("خطای در حذف کلاس به وجود امد است مجدد تلاش کنید.");

            _context.CourseEnrollments.Remove(existingEnrollment);
            await _context.SaveChangesAsync();

            return new OutPutModel<List<CourseDTO>>
            {
                StatusCode = 200,
                Message = "ترک کلاس با موفقیت  انجام شد.",
                Result = await GetCourseAsync()
            };
        }

        public async Task<OutPutModel<List<CourseDTO>>> UpdateCourseAsync(UpdateCourseDTO model)
        {
            ValidateModel.ValidateOrThrow(model);

            var course = await _context.Courses.FindAsync(model.CourseId);

            if (course is null)
                throw new NotFoundAppException("درس پیدا نشد .");

            course.Description = model.Description;
            course.Title = model.Title;
            course.TermId = model.TermId;
            course.CountMembers = model.CountMembers;

            if (model.ChangeLink)
                course.Link = NameGenerator.GenerateShareLink(8);

            _context.Courses.Update(course);
            await _context.SaveChangesAsync();

            return new OutPutModel<List<CourseDTO>>
            {
                StatusCode = 200,
                Message = "بروزرسانی  با موفقیت انجام شد.",
                Result = await GetCourseAsync()
            };
        }
    }
}
