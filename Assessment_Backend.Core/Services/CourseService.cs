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
                        Message = "لطفاً مجدداً وارد حساب کاربری خود شوید."
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
                    Message = "درس با موفقیت ثبت شد.",
                    Result = await GetCourseAsync()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<List<CourseDTO>>
                {
                    Message = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید.",
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
                        Result = null,
                        StatusCode = 401,
                        Message = "لطفاً مجدداً وارد حساب کاربری خود شوید."
                    };
                }

                var existingCourse = await _context.Courses
                    .SingleOrDefaultAsync(c => c.CourseId == model.CourseId && c.TeacherId == teacherId);
                if (existingCourse is null)
                {
                    return new OutPutModel<List<CourseDTO>>
                    {
                        StatusCode = 404,
                        Result = await GetCourseAsync(),
                        Message = "درس انتخاب شده حذف نشد مجدد تلاش کنید."

                    };
                }
                _context.Courses.Remove(existingCourse);
                await _context.SaveChangesAsync();

                return new OutPutModel<List<CourseDTO>>
                {
                    StatusCode = 200,
                    Message = " درس با موفقیت حذف شد. ",
                    Result = await GetCourseAsync(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<List<CourseDTO>>
                {
                    Message = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید.",
                    Result = null,
                    StatusCode = 500
                };
            }
        }

        public async Task<OutPutModel<List<CourseDTO>>> GetAllCourseAsync()
        {
            try
            {

                return new OutPutModel<List<CourseDTO>>
                {
                    Result = await GetCourseAsync(),
                    Message = "",
                    StatusCode = 200,
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<List<CourseDTO>>
                {
                    StatusCode = 500,
                    Message = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید",
                };
            }
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
                    Message = ".خطای غیرمنتظره ای رخ داد مجدد تلاش کنید",
                    StatusCode = 500,

                };
            }
        }


        public async Task<List<CourseDTO>> GetCourseAsync()
        {
            try
            {
                int teacherId = _httpContextAccessor.GetTeacherId();
                int studentId = _httpContextAccessor.GetStudentId();

                var query = _context.Courses
                    .Include(c => c.Term)
                    .Include(c => c.Teacher)
                    .Include(c => c.Assessments)
                    .Include(c => c.CourseEnrollments).ThenInclude(e => e.Student) // Include Student information
                    .Where(c => teacherId != 0 ? c.TeacherId == teacherId : c.CourseEnrollments.Any(e => e.StudentId == studentId))
                    .Select(c => new CourseDTO
                    {
                        CountMembers = c.CountMembers,
                        Description = c.Description,
                        Link = c.Link,
                        Title = c.Title,
                        CourseId = c.CourseId,
                        Term = c.Term.Title,
                        TeacherName = c.Teacher.Name + " " + c.Teacher.family,
                         Student = teacherId > 0 ? c.CourseEnrollments.Select(e => new StudentDTO // Add StudentDTO to CourseDTO only if teacherId > 0
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }


        public async Task<OutPutModel<List<CourseDTO>>> JoinClassAsync(JoinClassDTO model)
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


                int studentId = _httpContextAccessor.GetStudentId();

                if (studentId is 0)
                {
                    return new OutPutModel<List<CourseDTO>>
                    {
                        Result = null,
                        StatusCode = 401,
                        Message = "لطفاً مجدداً وارد حساب کاربری خود شوید."
                    };
                }
                var course = await _context.Courses.SingleOrDefaultAsync(c => c.Link == model.ClassLink);
                if (course is null)
                {
                    return new OutPutModel<List<CourseDTO>>
                    {
                        Result = null,
                        StatusCode = 404,
                        Message = "درسی پیدا نشد."
                    };
                }
                var existingEnrollment = await _context.CourseEnrollments
                          .SingleOrDefaultAsync(ce =>
                          ce.CourseId == course.CourseId 
                          && ce.StudentId == studentId);

                if (existingEnrollment != null)
                {
                    return new OutPutModel<List<CourseDTO>>
                    {
                        Message = "شما قبلاً در این درس عضو شده‌اید.",
                        StatusCode = 409,
                        Result = await GetCourseAsync()
                    };
                }

                var count=_context.CourseEnrollments
                    .Where(ce=> ce.CourseId==course.CourseId).Count();
                if (count >= course.CountMembers)
                {
                    return new OutPutModel<List<CourseDTO>>
                    {
                        Message = "ظرفیت کلاس پر شده است.",
                        StatusCode = 409,
                        Result = await GetCourseAsync()
                    };
                }
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<List<CourseDTO>>
                {
                    StatusCode = 500,
                    Message = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید.",
                };
            }
        }

        public async Task<OutPutModel<List<CourseDTO>>> LeavingClassAsync(LeavingClassDTO model)
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


                int studentId = _httpContextAccessor.GetStudentId();

                if (studentId is 0)
                {
                    return new OutPutModel<List<CourseDTO>>
                    {
                        Result = null,
                        StatusCode = 401,
                        Message = "لطفاً مجدداً وارد حساب کاربری خود شوید."
                    };
                }

                var existingEnrollment = await _context.CourseEnrollments
                    .SingleOrDefaultAsync(c=> c.CourseId==model.CourseId&&c.StudentId==studentId);

                if (existingEnrollment is null)
                {
                    return new OutPutModel<List<CourseDTO>>
                    {
                         Result= await GetCourseAsync(),
                         Message="خطای در حذف کلاس به وجود امد است مجدد تلاش کنید.",
                         StatusCode=404
                    };
                }
                _context.CourseEnrollments.Remove(existingEnrollment);
                await _context.SaveChangesAsync();


                return new OutPutModel<List<CourseDTO>>
                {
                     StatusCode=200,
                     Message="ترک کلاس با موفقیت  انجام شد.",
                     Result = await GetCourseAsync()
                };
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, ex);
                return new OutPutModel<List<CourseDTO>>
                {
                    StatusCode = 500,
                    Message = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید",
                };
            }
        }

        public async Task<OutPutModel<List<CourseDTO>>> UpdateCourseAsync(UpdateCourseDTO model)
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

                var course = await _context.Courses.FindAsync(model.CourseId);

                if (course is null)
                {
                    return new OutPutModel<List<CourseDTO>>
                    {
                        Result = null,
                        StatusCode = 404,
                        Message = "درس پیدا نشد ."
                    };
                }
                course.Description = model.Description;
                course.Title = model.Title;
                course.TermId = model.TermId;
                course.CountMembers = model.CountMembers;   
                

                if (model.ChangeLink)
                {
                    course.Link = NameGenerator.GenerateShareLink(8);

                }


                _context.Courses.Update(course);
                await _context.SaveChangesAsync();


                return new OutPutModel<List<CourseDTO>>
                {
                    StatusCode = 200,
                    Message = "بروزرسانی  با موفقیت انجام شد.",
                    Result = await GetCourseAsync()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<List<CourseDTO>>
                {
                    StatusCode = 500,
                    Message = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید",
                };
            }
        }

    }
}
