using Assessment_Backend.DataLayer.Entities.duty;
using Assessment_Backend.DataLayer.Entities.User;
using AutoMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Assessment_Backend.Core.Services
{
    public class AssessmentService : IAssessmentService
    {
        private readonly AssessmentDbContext _context;
        private readonly ILogger<AssessmentService> _logger;
        public AssessmentService(AssessmentDbContext context, ILogger<AssessmentService> logger)
        {
            _context = context;
            _logger = logger;

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
                     Result=await GetCourseByIdAsync(assessmentDTO.CourseId),   
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

        public async Task<CourseDTO> GetCourseByIdAsync(int courseId)
        {
            try
            {
                var course=await _context.Courses
                    .Include(t=> t.Term)
                    .Include(t=> t.Teacher)
                    .Include(a=> a.Assessments)
                    .Where(a=> a.CourseId==courseId)
                    .Select(c=> new CourseDTO
                    {
                         Title = c.Title,
                         CountMembers = c.CountMembers,
                         CourseId=c.CourseId,
                         Description = c.Description,
                         Term=c.Term.Title,
                          Link=c.Link,
                           TeacherName=c.Teacher.Name + " " + c.Teacher.family,
                            Assessments=c.Assessments
                            .Select(a=> new AssessmentDTO
                            {
                                 AssessmentId=a.AssessmentId,
                                 CourseId=a.CourseId,
                                 Description=a.Description, 
                                 EndDate=a.EndDate,
                                 Title = a.Title,
                                 PenaltyRule=a.PenaltyRule,
                                 StartDate=a.StartDate,
                                 
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

                var assessment=await _context.Assessments.FindAsync(assessmentDTO.AssessmentId);
                if (assessment is null)
                {
                    return new OutPutModel<CourseDTO>
                    {
                         Result = null,
                         StatusCode=404,
                         Message=""
                    };
                }

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<UpdateAssessmentDTO,Assessment>()
                        .ForMember(dest => dest.AssessmentId, opt => opt.Ignore()) // برای جلوگیری از بروزرسانی ایدی
                        .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
                });
                var mapper = config.CreateMapper();

                mapper.Map(assessmentDTO,assessment);

                _context.Assessments.Update(assessment);
                await _context.SaveChangesAsync();
                return new OutPutModel<CourseDTO>
                {
                    StatusCode=200,
                    Message="",
                     Result=await GetCourseByIdAsync(assessmentDTO.CourseId),
                };
            }
            catch (Exception  ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<CourseDTO>
                {

                     Message = ex.Message,
                      StatusCode=500,
                       Result=null,
                };
            }
        }
    }
}
