using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System.Globalization;

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
    private readonly IAmazonS3 _s3Client;
    private readonly AssessmentDbContext _context;
    private readonly ILogger<AssessmentService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AssessmentService(AssessmentDbContext context, ILogger<AssessmentService> logger,
        IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IAmazonS3 s3Client)
    {
        _context = context;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _s3Client = s3Client;

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

    public async Task<OutPutModel<AssessmentDTO>> AssignmentSubmissionAsync(AssignmentSubmissionDTO assessmentSubmissionDTO)
    {
        ValidateModel.ValidateOrThrow(assessmentSubmissionDTO);

        var studentId = _httpContextAccessor.GetStudentId();
        if (studentId is 0)
            throw new UnauthorizedAppException();

        // دانشجو فقط می‌تواند برای تکلیفی که عضو کلاس آن است ارسال کند
        bool hasAccess = await _context.Assessments
            .AsNoTracking()
            .AnyAsync(a => a.AssessmentId == assessmentSubmissionDTO.AssignmentId
                && a.Course.CourseEnrollments.Any(e => e.StudentId == studentId));

        if (!hasAccess)
            throw new NotFoundAppException("تکلیف پیدا نشد.");

        var assessment = await _context.Assessments
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.AssessmentId == assessmentSubmissionDTO.AssignmentId)
            ?? throw new NotFoundAppException("تکلیف پیدا نشد.");

        bool alreadySubmitted = await _context.AssignmentSubmissions
            .AsNoTracking()
            .AnyAsync(a => a.StudentId == studentId && a.AssignmentId == assessmentSubmissionDTO.AssignmentId);

        if (alreadySubmitted)
            throw new ConflictAppException("دانشجوی گرامی شما قبلا تکلیف خود را ارسال کردید.");

        string fileName = "default";
        if (assessmentSubmissionDTO.File is not null)
        {
            fileName = "Submission/" + NameGenerator.GenerateName()
                + Path.GetExtension(assessmentSubmissionDTO.File.FileName);

            bool uploaded = await UploadWithBucketCheckAsync(fileName, assessmentSubmissionDTO.File);
            if (!uploaded)
                throw new BusinessException("بارگزاری ناموفق بود. مجدداً تلاش کنید.", 500);
        }

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

        int teacherId = _httpContextAccessor.GetTeacherId();
        if (teacherId is 0)
            throw new UnauthorizedAppException();

        // استاد فقط می‌تواند برای کلاس خودش تکلیف بسازد
        bool ownsCourse = await _context.Courses
            .AsNoTracking()
            .AnyAsync(c => c.CourseId == assessmentDTO.CourseId && c.TeacherId == teacherId);

        if (!ownsCourse)
            throw new NotFoundAppException("کلاس مورد نظر پیدا نشد.");

        var fileName = "default";
        if (assessmentDTO.File is not null)
        {
            fileName = "assessment/" + NameGenerator.GenerateName()
                + Path.GetExtension(assessmentDTO.File.FileName);

            bool uploaded = await UploadWithBucketCheckAsync(fileName, assessmentDTO.File);
            if (!uploaded)
                throw new BusinessException("بارگزاری ناموفق بود. مجدداً تلاش کنید.", 500);
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
            FileName = fileName,
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

    public async Task<OutPutModel<CourseDTO>> UpdateAssessmentAsync(UpdateAssessmentDTO assessmentDTO)
    {
        ValidateModel.ValidateOrThrow(assessmentDTO);

        int teacherId = _httpContextAccessor.GetTeacherId();
        if (teacherId is 0)
            throw new UnauthorizedAppException();

        // استاد فقط تکلیف کلاس خودش را می‌تواند ویرایش کند
        var assessment = await _context.Assessments
            .SingleOrDefaultAsync(a => a.AssessmentId == assessmentDTO.AssessmentId
                && a.Course.TeacherId == teacherId);

        if (assessment is null)
            throw new NotFoundAppException("تکلیف پیدا نشد.");

        if (assessmentDTO.File != null)
        {
            var fileName = "assessment/" + NameGenerator.GenerateName()
                + Path.GetExtension(assessmentDTO.File.FileName);

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
        int teacherId = _httpContextAccessor.GetTeacherId();
        if (teacherId is 0)
            throw new UnauthorizedAppException();

        // استاد فقط تکلیف کلاس خودش را می‌تواند حذف کند
        var existing = await _context.Assessments
            .SingleOrDefaultAsync(a => a.AssessmentId == assessmentDTO.AssessmentId
                && a.Course.TeacherId == teacherId);

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
            .AsNoTracking()
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
             .AsNoTracking()
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
            .AsNoTracking()
            .Where(c => c.CourseId == courseId)
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
            }).SingleOrDefaultAsync();

        if (course is null)
            throw new NotFoundAppException("درس پیدا نشد.");

        return course;
    }

    public async Task<OutPutModel<AssessmentDTO>> ScoreRegistrationAsync(ScoreRegistrationDTO scoreRegistrationDTO)
    {
        int teacherId = _httpContextAccessor.GetTeacherId();
        if (teacherId is 0)
            throw new UnauthorizedAppException();

        // استاد فقط نمره ارسال‌های تکلیف کلاس خودش را می‌تواند ثبت کند
        var existingSubmissions = await _context.AssignmentSubmissions
            .SingleOrDefaultAsync(a => a.AS_Id == scoreRegistrationDTO.AS_Id
                && a.Assessment.Course.TeacherId == teacherId);

        if (existingSubmissions is null)
            throw new NotFoundAppException("در ثبت نمره مشکل به وجود اومد مجدد تلاش کنید.");

        var timeSent = existingSubmissions.CreateDate;

        var assessment = await _context.Assessments
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.AssessmentId == existingSubmissions.AssignmentId)
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
                    throw new BusinessException("قاعده‌ی اعمال جریمه دارای مشکل است", 400);

                // تعداد روزهای تأخیر (بخش اعشاری = ساعات تأخیر)
                var delay = (timeSent - expirationDate).TotalDays;

                // بزرگ‌ترین پله‌ای که هنوز به آن نرسیده‌ایم؛ اگر از همه پله‌ها بگذرد، بیشترین جریمه اعمال می‌شود
                var applicable = penaltyRule.FirstOrDefault(p => delay < p.days);
                if (applicable == default)
                    applicable = penaltyRule.Last();

                var penaltyPercentage = applicable.score / 100.0;
                var scoreWithPenalty = scoreRegistrationDTO.Score * penaltyPercentage;
                existingSubmissions.RawScore = scoreRegistrationDTO.Score;
                existingSubmissions.LateScore = (int)Math.Round(scoreWithPenalty);
            }
            else
            {
                existingSubmissions.LateScore = 0;
                existingSubmissions.RawScore = scoreRegistrationDTO.Score;
            }
        }

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

        var assessments = await _context.Courses
            .AsNoTracking()
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
                submitted = studentId > 0 ? a.AssignmentSubmissions
                    .Where(s => s.StudentId == studentId)
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
            .ToListAsync();

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
            .AsNoTracking()
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
            .AsNoTracking()
            .Where(a => a.StudentId == studentId)
            .Select(s => new ScoreDTO
            {
                LastScore = s.LateScore,
                TermId = s.Assessment.Course.Term.Title,
                CourseTitle = s.Assessment.Course.Title
            }).ToListAsync();

        if (scores.Count == 0)
            throw new BusinessException("هنوز نمره‌ای برای شما ثبت نشده است.", 404);

        int average = (int)Math.Round(scores.Average(s => (double)s.LastScore));

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
        bool bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _s3Options.BucketName);
        if (!bucketExists)
        {
            _logger.LogCritical("Bucket {Bucket} in ArvanStorage doesn't exist.", _s3Options.BucketName);
            return false;
        }

        return await UploadObjectFromFileAsync(_s3Client, _s3Options.BucketName, fileName, formFile);
    }

    private async Task<bool> UploadObjectFromFileAsync(IAmazonS3 client, string bucketName, string keyName, IFormFile formFile)
    {
        try
        {
            // استریم مستقیم از IFormFile به S3 - بدون بافر کردن کل فایل در حافظه
            await using Stream inputStream = formFile.OpenReadStream();

            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
                InputStream = inputStream,
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

    /// <summary>
    /// پارس قاعده جریمه به شکل "1d 90n2d 50n3d 0" یعنی بعد از ۱ روز نمره ۹۰٪، بعد از ۲ روز ۵۰٪ و بعد از ۳ روز صفر.
    /// </summary>
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
                    if (double.TryParse(timePart.TrimEnd('d'), NumberStyles.Float, CultureInfo.InvariantCulture, out var days) &&
                        double.TryParse(scorePart, NumberStyles.Float, CultureInfo.InvariantCulture, out var score))
                    {
                        penalties.Add((days, score));
                    }
                }
            }
        }

        return penalties.OrderBy(x => x.days).ToList();
    }

    #endregion
}
