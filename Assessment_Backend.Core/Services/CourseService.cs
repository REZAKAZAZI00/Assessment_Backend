namespace Assessment_Backend.Core.Servies
{
    public class CourseService : ICourseService
    {
        private readonly AssessmentDbContext _context;
        private readonly ILogger<CourseService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CourseService(AssessmentDbContext context, ILogger<CourseService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<OutPutModel<List<CourseDTO>>> CreateCourseAsync(CreateCourseDTO model)
        {
            try
            {
                if (!ValidateModel.Validate(model, out var validationResult))
                {
                    _logger.LogError(validationResult);

                    return new OutPutModel<List<CourseDTO>>
                    {
                        Message = validationResult,
                        Result = null,
                        StatusCode = 400
                    };
                }

                int teacherId = _httpContextAccessor.GetTeacherId();

                if (teacherId is 0)
                {
                    return new OutPutModel<List<CourseDTO>>
                    {

                        Result = null,
                        StatusCode = 401,
                        Message = ""
                    };
                }

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
                    Message = "",
                    Result = await GetCourseAsync()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,ex);
                return new OutPutModel<List<CourseDTO>>
                {
                    Message = ex.Message,
                    Result = null,
                    StatusCode = 500
                };

            }
        }

        public async Task<OutPutModel<List<CourseDTO>>> DeleteCourseAsync(DeleteCourseDTO model)
        {
            try
            {
                if (!ValidateModel.Validate(model, out var validationResult))
                {
                    _logger.LogError(validationResult);

                    return new OutPutModel<List<CourseDTO>>
                    {
                        Message = validationResult,
                        Result = null,
                        StatusCode = 400
                    };
                }


                int teacherId = _httpContextAccessor.GetTeacherId();

                if (teacherId is 0)
                {
                    return new OutPutModel<List<CourseDTO>>
                    {
                         Result= null,
                         StatusCode = 401,
                         Message=""
                    };
                }

                var existingCourse = await _context.Courses
                    .SingleOrDefaultAsync(c => c.CourseId == model.CourseId&&c.TeacherId==teacherId);
                if (existingCourse is null)
                {
                    return new OutPutModel<List<CourseDTO>>
                    {
                        StatusCode = 404,
                        Result = await GetCourseAsync(),
                        Message = ""

                    };
                }
                _context.Courses.Remove(existingCourse);
                await _context.SaveChangesAsync();

                return new OutPutModel<List<CourseDTO>>
                {
                    StatusCode = 200,
                    Message = "",
                    Result = await GetCourseAsync(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<List<CourseDTO>>
                {
                    Message = ex.Message,
                    Result = null,
                    StatusCode = 500
                };
            }
        }

        public async Task<OutPutModel<List<CourseDTO>>> GetAllCourseAsync()
        {
            try
            {
                var course = await GetCourseAsync();
                return new OutPutModel<List<CourseDTO>>
                {
                    Result = null,
                    Message = "",
                    StatusCode = 200,
                };

            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.Message, ex);
                throw;
            }
            throw new NotImplementedException();

        }

        public async Task<OutPutModel<List<TermDTO>>> GetAllTermAsync()
        {
            try
            {
                var term = await _context.Terms
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
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, ex);
                return new OutPutModel<List<TermDTO>>
                {
                    Message = ex.Message,
                    StatusCode = 500,

                };
            }
        }

        public async Task<List<CourseDTO>> GetCourseAsync()
        {
            try
            {
                int teacherId = _httpContextAccessor.GetTeacherId();

                if (teacherId is 0)
                {
                    return null;
                }
                var course = await _context.Courses
                   .Include(t => t.Term)
                   .Include(t => t.Teacher)
                   .Where(c => c.TeacherId == teacherId)
                   .Select(c => new CourseDTO
                   {
                       CountMembers = c.CountMembers,
                       Description = c.Description,
                       Link = c.Link,
                       Title = c.Title,
                       CourseId = c.CourseId,
                       Term = c.Term.Title,
                       TeacherName = c.Teacher.Name + " " + c.Teacher.family

                   })
                   .ToListAsync();
                return course;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }

        public Task<OutPutModel<List<CourseDTO>>> UpdateCourseAsync(UpdateCourseDTO model)
        {
            throw new NotImplementedException();
        }
    }
}
