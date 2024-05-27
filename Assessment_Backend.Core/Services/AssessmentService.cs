namespace Assessment_Backend.Core.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly AssessmentDbContext _context;
        private readonly ILogger<AssessmentService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AssessmentService(AssessmentDbContext context, ILogger<AssessmentService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<OutPutModel<AssessmentDTO>> AssignmentSubmissionAsync(AssignmentSubmissionDTO assessmentSubmissionDTO)
        {
            try
            {
                if (!ValidateModel.Validate(assessmentSubmissionDTO, out var validationResult))
                {
                    _logger.LogError(validationResult);

                    return new OutPutModel<AssessmentDTO>
                    {
                        Message = validationResult,
                        Result = null,
                        StatusCode = 400
                    };
                }

                var studentId = _httpContextAccessor.GetStudentId();

                if (studentId is 0)
                {
                    return new OutPutModel<AssessmentDTO>
                    {

                        Result = null,
                        StatusCode = 403,
                        Message = ""
                    };
                }

                string fileName = NameGenerator.GenerateName(15);
                // TODO Save File
                var newAssessmebt = new AssignmentSubmission
                {
                    CreateDate = DateTime.Now,
                    AssignmentId = assessmentSubmissionDTO.AssignmentId,
                    StudentId = studentId,
                    Text = assessmentSubmissionDTO.Text,
                    FileName = fileName,

                };
                await _context.AssignmentSubmissions.AddAsync(newAssessmebt);
                await _context.SaveChangesAsync();

                return new OutPutModel<AssessmentDTO>
                {
                    StatusCode = 200,
                    Message = "",
                    Result = await GetAssignmentByIdAsync(assessmentSubmissionDTO.AssignmentId),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<AssessmentDTO>
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }

        public async Task<OutPutModel<CourseDTO>> CreateAssessmentAsync(CreateAssessmentDTO assessmentDTO)
        {
            try
            {
                if (!ValidateModel.Validate(assessmentDTO, out var validationResult))
                {
                    _logger.LogError(validationResult);

                    return new OutPutModel<CourseDTO>
                    {
                        Message = validationResult,
                        Result = null,
                        StatusCode = 400
                    };
                }
                var newAssessment = new Assessment()
                {
                    Description = assessmentDTO.Description,
                    EndDate = assessmentDTO.EndDate,
                    Title = assessmentDTO.Title,
                    IsDelete = false,
                    StartDate = assessmentDTO.StartDate,
                    PenaltyRule = assessmentDTO.PenaltyRule,
                    CourseId = assessmentDTO.CourseId,
                    FileName = ""
                };
                await _context.Assessments.AddAsync(newAssessment);
                await _context.SaveChangesAsync();
                return new OutPutModel<CourseDTO>
                {
                    StatusCode = 200,
                    Result = await GetCourseByIdAsync(assessmentDTO.CourseId),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<CourseDTO>
                {
                    Message = ex.Message,
                    StatusCode = 500,

                };
            }
        }

        public async Task<OutPutModel<CourseDTO>> DeleteAssessmentAsync(DeleteAssessmentDTO assessmentDTO)
        {
            try
            {
                var existing = await _context.Assessments.SingleOrDefaultAsync();
                if (existing is null)
                {
                    return new OutPutModel<CourseDTO>
                    {
                        StatusCode = 404,
                        Message = "",

                    };
                }
                _context.Assessments.Remove(existing);
                await _context.SaveChangesAsync();

                return new OutPutModel<CourseDTO>
                {
                    StatusCode = 200,
                    Message = "",
                    Result = await GetCourseByIdAsync(existing.CourseId)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<CourseDTO>
                {
                    StatusCode = 500,
                    Message = ex.Message,
                };
            }
        }

        public async Task<AssessmentDTO> GetAssignmentByIdAsync(int assessmentId)
        {
            try
            {
                var assessment = await _context.Assessments
                    .Where(a => a.AssessmentId == assessmentId)
                    .Select(a => new AssessmentDTO

                    {
                        AssessmentId = a.AssessmentId,
                        CourseId = a.CourseId,
                        Description = a.Description,
                        EndDate = a.EndDate,
                        Title = a.Title,
                        StartDate = a.StartDate,
                        PenaltyRule = a.PenaltyRule,
                    }).SingleOrDefaultAsync();

                return assessment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }

        public async Task<OutPutModel<List<SubmittedAssignmentDTO>>> GetAssignmentSubmissionsByIdAsync(int assignmentId)
        {
            try
            {
               var assessments=await _context.AssignmentSubmissions
                    .Include(s=> s.Student)
                    .Where(a=> a.AssignmentId == assignmentId)
                    .Select(a=> new SubmittedAssignmentDTO
                    {
                         AssignmentId = a.AssignmentId,
                         CreateDate = a.CreateDate,
                         LateScore=a.LateScore,
                         AS_Id=a.AS_Id,
                         FileName=a.FileName,
                         RawScore=a.RawScore,
                         Text=a.Text,
                         ReviewedDate=a.ReviewedDate,
                         Student=new StudentDTO
                         {
                             Email=a.Student.Email,
                             Name=a.Student.Name,
                             PhoneNumber=a.Student.PhoneNumber,
                             StudentId=a.Student.StudentId,
                             family = a.Student.family   
                         }  
                    })
                    .ToListAsync();
                return new OutPutModel<List<SubmittedAssignmentDTO>>
                {
                     Result=assessments,
                      StatusCode=200,
                       Message=""
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,ex);
                return new OutPutModel<List<SubmittedAssignmentDTO>>
                {
                    StatusCode = 500,
                    Message = ex.Message,

                };
            }
        }

        public async Task<CourseDTO> GetCourseByIdAsync(int courseId)
        {
            try
            {
                var course = await _context.Courses
                    .Include(t => t.Term)
                    .Include(t => t.Teacher)
                    .Include(a => a.Assessments)
                    .Where(a => a.CourseId == courseId)
                    .Select(c => new CourseDTO
                    {
                        Title = c.Title,
                        CountMembers = c.CountMembers,
                        CourseId = c.CourseId,
                        Description = c.Description,
                        Term = c.Term.Title,
                        Link = c.Link,
                        TeacherName = c.Teacher.Name + " " + c.Teacher.family,
                        Assessments = c.Assessments
                            .Select(a => new AssessmentDTO
                            {
                                AssessmentId = a.AssessmentId,
                                CourseId = a.CourseId,
                                Description = a.Description,
                                EndDate = a.EndDate,
                                Title = a.Title,
                                PenaltyRule = a.PenaltyRule,
                                StartDate = a.StartDate,
                                 
                            })
                            .ToList()
                    }).SingleAsync();

                return course;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }

        public async Task<OutPutModel<AssessmentDTO>> ScoreRegistrationAsync(ScoreRegistrationDTO scoreRegistrationDTO)
        {
            try
            {
                    
                var existingAssignmentSubmissions = await _context.AssignmentSubmissions
                    .Where(a=> a.AS_Id==scoreRegistrationDTO.AS_Id)
                    .SingleOrDefaultAsync();
                if (existingAssignmentSubmissions is null)
                {
                    return new OutPutModel<AssessmentDTO>
                    {
                         Result=null,
                         Message="",
                         StatusCode=400,
                    };
                }
                existingAssignmentSubmissions.LateScore=scoreRegistrationDTO.Score;
                _context.AssignmentSubmissions.Update(existingAssignmentSubmissions);
                await _context.SaveChangesAsync();  
                return new OutPutModel<AssessmentDTO>
                {
                    StatusCode = 200,
                    Result = await GetAssignmentByIdAsync(existingAssignmentSubmissions.AssignmentId),
                    Message="",  
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,ex);
                return new OutPutModel<AssessmentDTO>
                {
                    Message = ex.Message,
                    Result = null,
                    StatusCode = 500
                };
            }
        }

        public async Task<OutPutModel<CourseDTO>> UpdateAssessmentAsync(UpdateAssessmentDTO assessmentDTO)
        {
            try
            {
                if (!ValidateModel.Validate(assessmentDTO, out var validationResult))
                {
                    _logger.LogError(validationResult);

                    return new OutPutModel<CourseDTO>
                    {
                        Message = validationResult,
                        Result = null,
                        StatusCode = 400
                    };
                }

                var assessment = await _context.Assessments.FindAsync(assessmentDTO.AssessmentId);
                if (assessment is null)
                {
                    return new OutPutModel<CourseDTO>
                    {
                        Result = null,
                        StatusCode = 404,
                        Message = ""
                    };
                }

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UpdateAssessmentDTO, Assessment>()
                        .ForMember(dest => dest.AssessmentId, opt => opt.Ignore()) // برای جلوگیری از بروزرسانی ایدی
                        .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
                });
                var mapper = config.CreateMapper();

                mapper.Map(assessmentDTO, assessment);

                _context.Assessments.Update(assessment);
                await _context.SaveChangesAsync();
                return new OutPutModel<CourseDTO>
                {
                    StatusCode = 200,
                    Message = "",
                    Result = await GetCourseByIdAsync(assessmentDTO.CourseId),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<CourseDTO>
                {

                    Message = ex.Message,
                    StatusCode = 500,
                    Result = null,
                };
            }
        }
    }
}
