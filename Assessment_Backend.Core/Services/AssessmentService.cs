using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Assessment_Backend.DataLayer.Entities.duty;
using System.Reflection;

namespace Assessment_Backend.Core.Services
{
    public class AssessmentService : IAssessmentService
    {
        #region Constructor
        private const string BUCKET_NAME = "assessment";
        private static IAmazonS3 _s3Client;
        private readonly AssessmentDbContext _context;
        private readonly ILogger<AssessmentService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AssessmentService(AssessmentDbContext context, ILogger<AssessmentService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion


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
                var awsCredentials = new Amazon.Runtime.BasicAWSCredentials("7115d73c-b7dc-421d-ab5d-19114c5a7057", "735e892a72adfea1666d7fd644d0eeab84f0b4e12e4ef0d4d488be06d72c90c3");
                var config = new AmazonS3Config { ServiceURL = "https://s3.ir-thr-at1.arvanstorage.ir" };
                _s3Client = new AmazonS3Client(awsCredentials, config);

                if (assessmentSubmissionDTO.File != null)
                {
                    var fileName = Path.Combine("Submission/", NameGenerator.GenerateName()
                        + Path.GetExtension(assessmentSubmissionDTO.File.FileName));

                    bool bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, BUCKET_NAME);
                    if (bucketExists)
                    {
                        var result = await UploadObjectFromFileAsync(_s3Client, BUCKET_NAME, fileName, assessmentSubmissionDTO.File);
                        if (result)
                        {
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
                                Message = "تکلیف با موفقیت ثبت شد.",
                                Result = await GetAssignmentByIdAsync(assessmentSubmissionDTO.AssignmentId),
                            };

                        }
                        else
                        {
                            return new OutPutModel<AssessmentDTO>
                            {
                                StatusCode = 500,
                                Message = "بارگزاری ناموفق بود. مجدداً تلاش کنید."
                            };
                        }
                    }
                    else
                    {
                        _logger.LogCritical("Bucket in ArvanStorage doesn't exist.");
                        return new OutPutModel<AssessmentDTO>
                        {
                            StatusCode = 500,
                            Message = "بارگزاری ناموفق بود. مجدداً تلاش کنید."
                        };
                    }
                }
                else
                {
                    var newAssessmebt = new AssignmentSubmission
                    {
                        CreateDate = DateTime.Now,
                        AssignmentId = assessmentSubmissionDTO.AssignmentId,
                        StudentId = studentId,
                        Text = assessmentSubmissionDTO.Text,
                        FileName = "default",

                    };
                    await _context.AssignmentSubmissions.AddAsync(newAssessmebt);
                    await _context.SaveChangesAsync();
                }


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
                    Message = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید",

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

                var awsCredentials = new Amazon.Runtime.BasicAWSCredentials("7115d73c-b7dc-421d-ab5d-19114c5a7057", "735e892a72adfea1666d7fd644d0eeab84f0b4e12e4ef0d4d488be06d72c90c3");
                var config = new AmazonS3Config { ServiceURL = "https://s3.ir-thr-at1.arvanstorage.ir" };
                _s3Client = new AmazonS3Client(awsCredentials, config);


                if (assessmentDTO.File != null)
                {
                    var fileName = Path.Combine("assessment/", NameGenerator.GenerateName() + Path.GetExtension(assessmentDTO.File.FileName));

                    bool bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, BUCKET_NAME);
                    if (bucketExists)
                    {
                        var result = await UploadObjectFromFileAsync(_s3Client, BUCKET_NAME, fileName, assessmentDTO.File);
                        if (result)
                        {
                            var newAssessment = new Assessment()
                            {
                                Description = assessmentDTO.Description,
                                EndDate = assessmentDTO.EndDate,
                                Title = assessmentDTO.Title,
                                IsDelete = false,
                                StartDate = assessmentDTO.StartDate,
                                PenaltyRule = assessmentDTO.PenaltyRule,
                                CourseId = assessmentDTO.CourseId,
                                FileName = fileName,
                            };
                            await _context.Assessments.AddAsync(newAssessment);
                            await _context.SaveChangesAsync();
                            return new OutPutModel<CourseDTO>
                            {
                                StatusCode = 200,
                                Result = await GetCourseByIdAsync(assessmentDTO.CourseId),
                                Message = " .تکلیف با موفقیت ثبت شد"
                            };
                        }
                        else
                        {
                            return new OutPutModel<CourseDTO>
                            {
                                StatusCode = 500,
                                Message = "بارگزاری ناموفق بود. مجدداً تلاش کنید."
                            };
                        }
                    }
                    else
                    {
                        _logger.LogCritical("Bucket in ArvanStorage doesn't exist.");
                        return new OutPutModel<CourseDTO>
                        {
                            StatusCode = 500,
                            Message = "بارگزاری ناموفق بود. مجدداً تلاش کنید."
                        };
                    }
                }
                else
                {
                    var newAssessment = new Assessment()
                    {
                        Description = assessmentDTO.Description,
                        EndDate = assessmentDTO.EndDate,
                        Title = assessmentDTO.Title,
                        IsDelete = false,
                        StartDate = assessmentDTO.StartDate,
                        PenaltyRule = assessmentDTO.PenaltyRule,
                        CourseId = assessmentDTO.CourseId,
                        FileName = "default",
                    };
                    await _context.Assessments.AddAsync(newAssessment);
                    await _context.SaveChangesAsync();
                    return new OutPutModel<CourseDTO>
                    {
                        StatusCode = 200,
                        Result = await GetCourseByIdAsync(assessmentDTO.CourseId),
                        Message = "تکلیف با موفقیت ثبت شد."
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<CourseDTO>
                {
                    Message = "خطای غیرمنتظره‌ای رخ داد. مجدداً تلاش کنید.",
                    StatusCode = 500,
                };
            }
        }


        public async Task<OutPutModel<CourseDTO>> DeleteAssessmentAsync(DeleteAssessmentDTO assessmentDTO)
        {
            try
            {
                var existing = await _context.Assessments
                    .SingleOrDefaultAsync(a => a.AssessmentId == assessmentDTO.AssessmentId);
                if (existing is null)
                {
                    return new OutPutModel<CourseDTO>
                    {
                        StatusCode = 404,
                        Message = "تکلیف پیدا نشد.",

                    };
                }
                _context.Assessments.Remove(existing);
                await _context.SaveChangesAsync();

                return new OutPutModel<CourseDTO>
                {
                    StatusCode = 200,
                    Message = "تکلیف با موفقیت حذف شد.",
                    Result = await GetCourseByIdAsync(existing.CourseId)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<CourseDTO>
                {
                    StatusCode = 500,
                    Message = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید"
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
                var assessments = await _context.AssignmentSubmissions
                     .Include(s => s.Student)
                     .Where(a => a.AssignmentId == assignmentId)
                     .Select(a => new SubmittedAssignmentDTO
                     {
                         AssignmentId = a.AssignmentId,
                         CreateDate = a.CreateDate,
                         LateScore = a.LateScore,
                         AS_Id = a.AS_Id,
                         FileName = a.FileName,
                         RawScore = a.RawScore,
                         Text = a.Text,
                         ReviewedDate = a.ReviewedDate,
                         Student = new StudentDTO
                         {
                             Email = a.Student.Email,
                             Name = a.Student.Name,
                             PhoneNumber = a.Student.PhoneNumber,
                             StudentId = a.Student.StudentId,
                             family = a.Student.family
                         }
                     })
                     .ToListAsync();
                return new OutPutModel<List<SubmittedAssignmentDTO>>
                {
                    Result = assessments,
                    StatusCode = 200,
                    Message = ""
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<List<SubmittedAssignmentDTO>>
                {
                    StatusCode = 500,
                    Message = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید"

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
                                FileName = a.FileName,
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

                var existingSubmissions = await _context.AssignmentSubmissions
                    .Where(a => a.AS_Id == scoreRegistrationDTO.AS_Id)
                    .SingleOrDefaultAsync();
                if (existingSubmissions is null)
                {
                    return new OutPutModel<AssessmentDTO>
                    {
                        Result = null,
                        Message = "در ثبت نمره مشکل به وجود اومد مجدد تلاش کنید؟",
                        StatusCode = 400,
                    };
                }


                var timeSent = existingSubmissions.CreateDate;

                var assessment = await _context.Assessments.FindAsync(existingSubmissions.AssignmentId);
                var expirationDate = assessment.EndDate;
                if (timeSent <= expirationDate)
                {
                    existingSubmissions.LateScore = scoreRegistrationDTO.Score;
                    existingSubmissions.RawScore = scoreRegistrationDTO.Score;
                }
                else
                {
                    if (assessment.PenaltyRule != "" && assessment.PenaltyRule != "0")
                    {
                        var penaltyRule = ParsePenaltyRule(assessment.PenaltyRule);

                        if (penaltyRule is null)
                        {
                            return new OutPutModel<AssessmentDTO>
                            {
                                Message = "قاعده‌ی اعمال جریمه دارای مشکل است ",
                                StatusCode = 404,
                                Result = null,
                            };
                        }

                        var delay = (int)(timeSent - expirationDate).TotalDays;

                        foreach (var item in penaltyRule)
                        {
                            if (item.days == delay)
                            {
                                var penaltyPercentage = item.score / 100.0;
                                var a = scoreRegistrationDTO.Score * penaltyPercentage;
                                existingSubmissions.RawScore = scoreRegistrationDTO.Score;
                                existingSubmissions.LateScore = (int)a;

                                break;
                            }
                        }

                    }
                    else
                    {
                        existingSubmissions.LateScore = 0;
                        existingSubmissions.RawScore = scoreRegistrationDTO.Score;
                    }

                }
                _context.AssignmentSubmissions.Update(existingSubmissions);
                await _context.SaveChangesAsync();
                return new OutPutModel<AssessmentDTO>
                {
                    StatusCode = 200,
                    Result = await GetAssignmentByIdAsync(existingSubmissions.AssignmentId),
                    Message = "نمره با موقثیت ثبت شد.",
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<AssessmentDTO>
                {
                    Message = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید",
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
                        Message = "تکلیف پیدا نشد ."
                    };
                }
                var awsCredentials = new Amazon.Runtime.BasicAWSCredentials("7115d73c-b7dc-421d-ab5d-19114c5a7057", "735e892a72adfea1666d7fd644d0eeab84f0b4e12e4ef0d4d488be06d72c90c3");
                var config = new AmazonS3Config { ServiceURL = "https://s3.ir-thr-at1.arvanstorage.ir" };
                _s3Client = new AmazonS3Client(awsCredentials, config);

                if (assessmentDTO.File != null)
                {
                    var fileName = Path.Combine("assessment/", NameGenerator.GenerateName() + Path.GetExtension(assessmentDTO.File.FileName));

                    bool bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, BUCKET_NAME);
                    if (bucketExists)
                    {
                        var result = await UploadObjectFromFileAsync(_s3Client, BUCKET_NAME, fileName, assessmentDTO.File);
                        if (result)
                        {
                            assessment.StartDate = assessmentDTO.StartDate;
                            assessment.EndDate = assessmentDTO.EndDate;
                            assessment.Description = assessmentDTO.Description;
                            assessment.CourseId = assessmentDTO.CourseId;
                            assessment.FileName = fileName;
                            assessment.PenaltyRule = assessmentDTO.PenaltyRule;
                            assessment.Title = assessmentDTO.Title;

                            _context.Assessments.Update(assessment);
                            await _context.SaveChangesAsync();
                            return new OutPutModel<CourseDTO>
                            {
                                StatusCode = 200,
                                Result = await GetCourseByIdAsync(assessmentDTO.CourseId),
                                Message = " .تکلیف با موفقیت بروزرسانی شد"
                            };
                        }
                        else
                        {
                            return new OutPutModel<CourseDTO>
                            {
                                StatusCode = 500,
                                Message = "بارگزاری ناموفق بود. مجدداً تلاش کنید."
                            };
                        }
                    }
                    else
                    {
                        _logger.LogCritical("Bucket in ArvanStorage doesn't exist.");
                        return new OutPutModel<CourseDTO>
                        {
                            StatusCode = 500,
                            Message = "بارگزاری ناموفق بود. مجدداً تلاش کنید."
                        };
                    }
                }
                else
                {
                    assessment.StartDate = assessmentDTO.StartDate;
                    assessment.EndDate = assessmentDTO.EndDate;
                    assessment.Description = assessmentDTO.Description;
                    assessment.CourseId = assessmentDTO.CourseId;
                    assessment.FileName = assessmentDTO.FileName;
                    assessment.PenaltyRule = assessmentDTO.PenaltyRule;
                    assessment.Title = assessmentDTO.Title;

                    _context.Assessments.Update(assessment);
                    await _context.SaveChangesAsync();

                    return new OutPutModel<CourseDTO>
                    {
                        StatusCode = 200,
                        Message = "با موفقیت بروزرسانی شد.",
                        Result = await GetCourseByIdAsync(assessmentDTO.CourseId),
                    };
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new OutPutModel<CourseDTO>
                {

                    Message = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید",

                    StatusCode = 500,
                    Result = null,
                };
            }
        }




        public async Task<bool> UploadObjectFromFileAsync(IAmazonS3 client, string bucketName, string keyName, IFormFile formFile)
        {
            try
            {
                using Stream inputStream = formFile.OpenReadStream();
                using MemoryStream memoryStream = new MemoryStream();
                inputStream.CopyTo(memoryStream);

                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    InputStream = inputStream,
                    ContentType = formFile.ContentType,//contentType, //For using and jpg image
                    CannedACL = S3CannedACL.PublicRead, //it's public to show as a image on internet

                };

                putRequest.Metadata.Add("x-amz-meta-title", "someTitle");

                PutObjectResponse response = await client.PutObjectAsync(putRequest);

                foreach (PropertyInfo prop in response.GetType().GetProperties())
                {
                    Console.WriteLine($"{prop.Name}: {prop.GetValue(response, null)}");
                }

                Console.WriteLine($"Object {keyName} added to {bucketName} bucket");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}");
                return false;
            }
        }



        public List<(double days, double score)> ParsePenaltyRule(string penaltyRule)
        {
            var penalties = new List<(double days, double score)>();
            var rules = penaltyRule.Split('n');
            foreach (var rule in rules)
            {
                var parts = rule.Split(' ');
                if (parts.Length == 2)
                {
                    var timePart = parts[0];
                    var scorePart = parts[1];

                    if (timePart.EndsWith("d"))
                    {
                        var days = double.Parse(timePart.TrimEnd('d'));
                        var score = double.Parse(scorePart);
                        penalties.Add((days, score));
                    }
                }
            }

            if (penalties.Count == 0)
            {
                return null;
            }
            penalties = penalties.OrderBy(x => x.days).ToList();
            return penalties;
        }

        public async Task<OutPutModel<List<AssessmentDTO>>> GetAllAssignmentAsync()
        {
            try
            {
                int teacherId = _httpContextAccessor.GetTeacherId();
                int studentId = _httpContextAccessor.GetStudentId();


                var assessments = _context.Courses
                    .Include(c => c.Teacher)
                    .Include(c => c.Assessments)
                    .Include(c => c.CourseEnrollments).ThenInclude(e => e.Student)
                    .Where(c => teacherId > 0 ? c.TeacherId == teacherId : c.CourseEnrollments.Any(e => e.StudentId == studentId))
                    .SelectMany(c => c.Assessments)
                    .Select(a => new AssessmentDTO
                    {
                        AssessmentId = a.AssessmentId,
                        StartDate = a.StartDate,
                        CourseId = a.Course.CourseId,
                        Description = a.Description,
                        EndDate = a.EndDate,
                        Title = a.Title,
                        PenaltyRule = a.PenaltyRule,
                        FileName = a.FileName,
                    })
                    .OrderBy(a=> a.CourseId)
                    .ThenBy(a=> a.AssessmentId)
                    .ToList();



                return new OutPutModel<List<AssessmentDTO>>
                {
                     StatusCode = 200,
                     Message=""
                     , Result= assessments,
                };
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, ex);
                return new OutPutModel<List<AssessmentDTO>>
                {
                    Message = "خطای غیرمنتظره‌ای رخ داد. مجدداً تلاش کنید.",
                    StatusCode = 500,
                };
            }
        }
    }
}
