using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace Assessment_Backend.Core.Services;

/// <summary>
/// تنظیمات S3 (آروان کلود) که از appsettings خوانده می‌شود.
/// کلیدهای قبلی به صورت Hardcoded در سورس بودند که حذف شدند.
/// </summary>
public class S3StorageOptions
{
    public const string SectionName = "S3Storage";

    public string ServiceUrl { get; set; } = "";
    public string AccessKey { get; set; } = "";
    public string SecretKey { get; set; } = "";
    public string BucketName { get; set; } = "assessment";
}

public class AssessmentService : IAssessmentService
{
    #region Constructor
    private readonly S3StorageOptions _s3Options;
    private IAmazonS3? _s3Client;
    private readonly AssessmentDbContext _context;
    private readonly ILogger<AssessmentService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AssessmentService(AssessmentDbContext context, ILogger<AssessmentService> logger,
        IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        var section = configuration.GetSection(S3StorageOptions.SectionName);
        _s3Options = new S3StorageOptions
        {
            ServiceUrl = section[nameof(S3StorageOptions.ServiceUrl)] ?? "",
            AccessKey = section[nameof(S3StorageOptions.AccessKey)] ?? "",
            SecretKey = section[nameof(S3StorageOptions.SecretKey)] ?? "",
            BucketName = section[nameof(S3StorageOptions.BucketName)] ?? "assessment"
        };
    }
    #endregion

    private IAmazonS3 S3Client => _s3Client ??= new AmazonS3Client(
        new Amazon.Runtime.BasicAWSCredentials(_s3Options.AccessKey, _s3Options.SecretKey),
        new AmazonS3Config { ServiceURL = _s3Options.ServiceUrl });

    public async Task<OutPutModel<AssessmentDTO>> AssignmentSubmissionAsync(AssignmentSubmissionDTO assessmentSubmissionDTO)
    {
        ValidateModel.ValidateOrThrow(assessmentSubmissionDTO);

        var studentId = _httpContextAccessor.GetStudentId();
        if (studentId is 0)
            throw new UnauthorizedAppException();

        var assessment = await _context.AssignmentSubmissions
            .SingleOrDefaultAsync(a => a.StudentId == studentId && a.AssignmentId == assessmentSubmissionDTO.AssignmentId);

        if (assessment is not null)
            throw new BusinessException("دانشجوی گرامی شما قبلا تکلیف خود را ارسال کردید.", 409);

        if (assessmentSubmissionDTO.File == null)
        {
            var newTextSubmission = new AssignmentSubmission
            {
                CreateDate = DateTime.Now,
                AssignmentId = assessmentSubmissionDTO.AssignmentId,
                StudentId = studentId,
                Text = assessmentSubmissionDTO.Text,
                FileName = "default",
            };
            await _context.AssignmentSubmissions.AddAsync(newTextSubmission);
            await _context.SaveChangesAsync();
        }
        else
        {
            var fileName = Path.Combine("Submission/", NameGenerator.GenerateName()
                + Path.GetExtension(assessmentSubmissionDTO.File.FileName));

            bool uploaded = await UploadWithBucketCheckAsync(fileName, assessmentSubmissionDTO.File);
            if (!uploaded)
                throw new BusinessException("بارگزاری ناموفق بود. مجدداً تلاش کنید.", 500);

            var newSubmission = new AssignmentSubmission
            {
                CreateDate = DateTime.Now,
                AssignmentId = assessmentSubmissionDTO.AssignmentId,
                StudentId = studentId,
                Text = assessmentSubmissionDTO.Text,
                FileName = fileName,
            };
            await _context.AssignmentSubmissions.AddAsync(newSubmission);
            await _context.SaveChangesAsync();
        }

        return new OutPutModel<AssessmentDTO>
        {
            StatusCode = 200,
            Message = "تکلیف با موفقیت ثبت شد.",
            Result = await GetAssignmentByIdAsync(assessmentSubmissionDTO.AssignmentId),
        };
    }

    public async Task<OutPutModel<CourseDTO>> CreateAssessmentAsync(CreateAssessmentDTO assessmentDTO)
    {
        ValidateModel.ValidateOrThrow(assessmentDTO);

        if (assessmentDTO.EndDate <= assessmentDTO.StartDate)
            throw new BusinessException("تاریخ پایان نمی‌تواند قبل از تاریخ شروع باشد.", 400);

        if (assessmentDTO.File == null)
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
        }
        else
        {
            var fileName = Path.Combine("assessment/", NameGenerator.GenerateName()
                + Path.GetExtension(assessmentDTO.File.FileName));

            bool uploaded = await UploadWithBucketCheckAsync(fileName, assessmentDTO.File);
            if (!uploaded)
                throw new BusinessException("بارگزاری ناموفق بود. مجدداً تلاش کنید.", 500);

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
        }

        return new OutPutModel<CourseDTO>
        {
            StatusCode = 200,
            Result = await GetCourseByIdAsync(assessmentDTO.CourseId),
            Message = "تکلیف با موفقیت ثبت شد."
        };
    }

    public async Task<OutPutModel<CourseDTO>> UpdateAssessmentAsync(UpdateAssessmentDTO assessmentDTO)
    {
        ValidateModel.ValidateOrThrow(assessmentDTO);

        var assessment = await _context.Assessments.FindAsync(assessmentDTO.AssessmentId);
        if (assessment is null)
            throw new NotFoundAppException("تکلیف پیدا نشد.");

        if (assessmentDTO.File != null)
        {
            var fileName = Path.Combine("assessment/", NameGenerator.GenerateName()
                + Path.GetExtension(assessmentDTO.File.FileName));

            bool uploaded = await UploadWithBucketCheckAsync(fileName, assessmentDTO.File);
            if (!uploaded)
                throw new BusinessException("بارگزاری ناموفق بود. مجدداً تلاش کنید.", 500);

            assessment.FileName = fileName;
        }
        else
        {
            assessment.FileName = assessmentDTO.FileName;
        }

        assessment.StartDate = assessmentDTO.StartDate;
        assessment.EndDate = assessmentDTO.EndDate;
        assessment.Description = assessmentDTO.Description;
        assessment.CourseId = assessmentDTO.CourseId;
        assessment.PenaltyRule = assessmentDTO.PenaltyRule;
        assessment.Title = assessmentDTO.Title;

        _context.Assessments.Update(assessment);
        await _context.SaveChangesAsync();

        return new OutPutModel<CourseDTO>
        {
            StatusCode = 200,
            Message = "تکلیف با موفقیت بروزرسانی شد.",
            Result = await GetCourseByIdAsync(assessmentDTO.CourseId),
        };
    }

    public async Task<OutPutModel<CourseDTO>> DeleteAssessmentAsync(DeleteAssessmentDTO assessmentDTO)
    {
        var existing = await _context.Assessments
            .SingleOrDefaultAsync(a => a.AssessmentId == assessmentDTO.AssessmentId);

        if (existing is null)
            throw new NotFoundAppException("تکلیف پیدا نشد.");

        _context.Assessments.Remove(existing);
        await _context.SaveChangesAsync();

        return new OutPutModel<CourseDTO>
        {
            StatusCode = 200,
            Message = "تکلیف با موفقیت حذف شد.",
            Result = await GetCourseByIdAsync(existing.CourseId)
        };
    }

    public async Task<AssessmentDTO> GetAssignmentByIdAsync(int assessmentId)
    {
        var assessment = await _context.Assessments
            .AsTracking()
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
                FileName = a.FileName
            }).SingleOrDefaultAsync();

        if (assessment is null)
            throw new NotFoundAppException("تکلیف پیدا نشد.");

        return assessment;
    }

    public async Task<OutPutModel<List<SubmittedAssignmentDTO>>> GetAssignmentSubmissionsByIdAsync(int assignmentId)
    {
        var submissions = await _context.AssignmentSubmissions
             .AsTracking()
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
            Result = submissions,
            StatusCode = 200,
            Message = ""
        };
    }

    public async Task<CourseDTO> GetCourseByIdAsync(int courseId)
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

    public async Task<OutPutModel<AssessmentDTO>> ScoreRegistrationAsync(ScoreRegistrationDTO scoreRegistrationDTO)
    {
        var existingSubmissions = await _context.AssignmentSubmissions
            .Where(a => a.AS_Id == scoreRegistrationDTO.AS_Id)
            .SingleOrDefaultAsync();

        if (existingSubmissions is null)
            throw new NotFoundAppException("در ثبت نمره مشکل به وجود اومد مجدد تلاش کنید.");

        var timeSent = existingSubmissions.CreateDate;

        var assessment = await _context.Assessments.FindAsync(existingSubmissions.AssignmentId)
            ?? throw new NotFoundAppException("تکلیف پیدا نشد.");

        var expirationDate = assessment.EndDate;
        if (timeSent <= expirationDate)
        {
            existingSubmissions.LateScore = scoreRegistrationDTO.Score;
            existingSubmissions.RawScore = scoreRegistrationDTO.Score;
        }
        else
        {
            if (!string.IsNullOrEmpty(assessment.PenaltyRule) && assessment.PenaltyRule != "0")
            {
                var penaltyRule = ParsePenaltyRule(assessment.PenaltyRule);

                if (penaltyRule.Count == 0)
                    throw new BusinessException("قاعده‌ی اعمال جریمه دارای مشکل است", 404);

                var delay = (int)(timeSent - expirationDate).TotalDays;

                foreach (var item in penaltyRule)
                {
                    if (item.days == delay)
                    {
                        var penaltyPercentage = item.score / 100.0;
                        var scoreWithPenalty = scoreRegistrationDTO.Score * penaltyPercentage;
                        existingSubmissions.RawScore = scoreRegistrationDTO.Score;
                        existingSubmissions.LateScore = (int)scoreWithPenalty;

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

    public async Task<OutPutModel<List<AssessmentDTO>>> GetAllAssignmentAsync()
    {
        int teacherId = _httpContextAccessor.GetTeacherId();
        int studentId = _httpContextAccessor.GetStudentId();

        if (teacherId is 0 && studentId is 0)
            throw new UnauthorizedAppException();

        var assessments = _context.Courses
            .AsTracking()
            .Include(c => c.Teacher)
            .Include(c => c.Assessments)
            .Include(c => c.CourseEnrollments).ThenInclude(e => e.Student)
            .Where(c => teacherId > 0 ? c.TeacherId == teacherId : c.CourseEnrollments.Any(e => e.StudentId == studentId))
            .SelectMany(c => c.Assessments)
            .Select(a => new AssessmentDTO
            {
                CourseTitle = a.Course.Title,
                AssessmentId = a.AssessmentId,
                StartDate = a.StartDate,
                CourseId = a.Course.CourseId,
                Description = a.Description,
                EndDate = a.EndDate,
                Title = a.Title,
                PenaltyRule = a.PenaltyRule,
                FileName = a.FileName,
                submitted = studentId > 0 ? _context.AssignmentSubmissions
                .Where(s => s.StudentId == studentId && s.AssignmentId == a.AssessmentId)
                .Select(s => new SubmittedAssignmentDTO
                {
                    AssignmentId = s.AssignmentId,
                    CreateDate = s.CreateDate,
                    LateScore = s.LateScore,
                    AS_Id = s.AS_Id,
                    FileName = s.FileName,
                    RawScore = s.RawScore,
                    ReviewedDate = s.ReviewedDate,
                    Text = s.Text,
                }).SingleOrDefault() : null
            })
            .OrderBy(a => a.CourseId)
            .ThenBy(a => a.AssessmentId)
            .ToList();

        return new OutPutModel<List<AssessmentDTO>>
        {
            StatusCode = 200,
            Message = "",
            Result = assessments,
        };
    }

    public async Task<OutPutModel<ReportDTO>> GetReportAsync()
    {
        var studentId = _httpContextAccessor.GetStudentId();
        if (studentId is 0)
            throw new UnauthorizedAppException();

        var student = await _context.Students
            .AsTracking()
            .Where(s => s.StudentId == studentId)
            .Select(s => new StudentDTO
            {
                StudentId = s.StudentId,
                Email = s.Email,
                Name = s.Name,
                PhoneNumber = s.PhoneNumber,
                family = s.family,
            })
            .SingleOrDefaultAsync()
            ?? throw new NotFoundAppException("دانشجو پیدا نشد.");

        var scores = await _context.AssignmentSubmissions
            .AsTracking()
            .Where(a => a.StudentId == studentId)
            .Include(a => a.Assessment)
            .ThenInclude(c => c.Course).ThenInclude(t => t.Term)
            .Select(s => new ScoreDTO
            {
                LastScore = s.LateScore,
                TermId = s.Assessment.Course.Term.Title,
                CourseTitle = s.Assessment.Course.Title
            }).ToListAsync();

        if (scores.Count == 0)
            throw new BusinessException("هنوز نمره‌ای برای شما ثبت نشده است.", 404);

        int average = (int)scores.Average(s => s.LastScore);

        var report = new ReportDTO
        {
            Student = student,
            scores = scores,
            ScoreAvrge = average
        };

        return new OutPutModel<ReportDTO>
        {
            Result = report,
            StatusCode = 200,
            Message = ""
        };
    }

    #region S3 Helpers

    /// <summary>
    /// آپلود فایل با بررسی وجود Bucket. در صورت عدم موفقیت false برمی‌گرداند
    /// و کالر با BusinessException پاسخ مناسب می‌سازد.
    /// </summary>
    private async Task<bool> UploadWithBucketCheckAsync(string fileName, IFormFile formFile)
    {
        bool bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(S3Client, _s3Options.BucketName);
        if (!bucketExists)
        {
            _logger.LogCritical("Bucket {Bucket} in ArvanStorage doesn't exist.", _s3Options.BucketName);
            return false;
        }

        return await UploadObjectFromFileAsync(S3Client, _s3Options.BucketName, fileName, formFile);
    }

    public async Task<bool> UploadObjectFromFileAsync(IAmazonS3 client, string bucketName, string keyName, IFormFile formFile)
    {
        try
        {
            using Stream inputStream = formFile.OpenReadStream();
            using MemoryStream memoryStream = new MemoryStream();
            await inputStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
                InputStream = memoryStream,
                ContentType = formFile.ContentType,
                CannedACL = S3CannedACL.PublicRead,
            };

            putRequest.Metadata.Add("x-amz-meta-title", "someTitle");

            await client.PutObjectAsync(putRequest);

            _logger.LogInformation("Object {KeyName} added to {BucketName} bucket", keyName, bucketName);
            return true;
        }
        catch (AmazonS3Exception s3Ex)
        {
            // خطای S3 را لاگ می‌کنیم و به صورت business error پاسخ می‌دهیم
            _logger.LogError(s3Ex, "S3 upload failed for {KeyName}", keyName);
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
            return new List<(double days, double score)>();

        penalties = penalties.OrderBy(x => x.days).ToList();
        return penalties;
    }

    #endregion
}
