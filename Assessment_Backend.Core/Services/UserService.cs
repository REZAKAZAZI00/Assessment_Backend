namespace Assessment_Backend.Core.Servies
{
    public class UserService : IUserService
    {
        private readonly AssessmentDbContext _context;
        private readonly ITokenHelperService _tokenHelperService;
        private readonly ILogger<UserService> _logger;

        public UserService(AssessmentDbContext context, ILogger<UserService> logger, ITokenHelperService tokenHelperService)
        {
            _context = context;
            _logger = logger;
            _tokenHelperService = tokenHelperService;
        }

        public async Task<OutPutModel<List<RoleDTO>>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _context.Roles.
                    Select(r => new RoleDTO()
                    {
                        RoleId = r.RoleId,
                        Title = r.Title,
                    }).ToListAsync();

                return new OutPutModel<List<RoleDTO>>
                {
                    Message = "",
                    Result = roles,
                    StatusCode = 200


                };

            }
            catch (Exception ex)
            {
                var errorMessage = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید";
                _logger.LogError(errorMessage, ex);
                return new OutPutModel<List<RoleDTO>>
                {
                    Message = errorMessage,
                    Result = null,
                    StatusCode = 500

                };
            }
        }

        public async Task<bool> IsExistCodeMelliAsync(string code)
        {
            return await _context.Users.AnyAsync(u => u.CodeMelli == code);
        }

        public async Task<OutPutModel<UserProfileDTO>> LoginAsync(LoginDTO model)
        {
            if (!ValidateModel.Validate(model, out var validationResult))
            {
                _logger.LogError(validationResult);

                return new OutPutModel<UserProfileDTO>
                {
                    Message = validationResult,
                    Result = null,
                    StatusCode = 400
                };
            }

            try
            {
                string password = PasswordHelper.EncodePasswordSHA1(model.Password);

                var existingUser = await _context.Users
                   .SingleOrDefaultAsync(u => u.CodeMelli == model.CodeMelli && u.Password == password);

                if (existingUser is null)
                {
                    return new OutPutModel<UserProfileDTO>
                    {
                        Message = "اطلاعات وارد شده صیحیح نمی باشد. ",
                        Result = null,
                        StatusCode = 400
                    };
                }
                var userProfile = new UserProfileDTO
                {
                    UserId = existingUser.UserId,
                    CodeMelli = existingUser.CodeMelli,
                    RoleId = existingUser.RoleId,
                    Token=""
                };
                var student = await _context.Students
                    .Include(g=> g.Grade)
                    .SingleOrDefaultAsync(s => s.UserId == existingUser.UserId);
                if (student != null)
                {
                    userProfile.Name = student.Name;
                    userProfile.Email = student.Email;
                    userProfile.PhoneNumber = student.PhoneNumber;
                    userProfile.Grade = student.Grade.Title;
                    userProfile.family=student.family;
                    userProfile.StudentId = student.StudentId;
                    userProfile.Token = _tokenHelperService.GenerateToken<Student>(existingUser,student);
                }
                else
                {
                    var teacher = await _context.Teachers.SingleOrDefaultAsync(t => t.UserId == existingUser.UserId);
                    if (teacher != null)
                    {
                        userProfile.TeacherId=teacher.TeacherId;
                        userProfile.Name = teacher.Name;
                        userProfile.family=teacher.family;
                        userProfile.Email = teacher.Email;
                        userProfile.PhoneNumber = teacher.PhoneNumber;
                        userProfile.TeacherCode = teacher.TeacherCode;
                        userProfile.Token = _tokenHelperService.GenerateToken<Teacher>(existingUser,teacher);
                    }
                }

                return new OutPutModel<UserProfileDTO>
                {
                     Result = userProfile,
                     StatusCode=200,
                     Message=""

                };

            }
            catch (Exception ex)
            {

                var errorMessage = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید";
                _logger.LogError(errorMessage, ex);
                return new OutPutModel<UserProfileDTO>
                {
                    Message = errorMessage,
                    Result = null,
                    StatusCode = 500

                };
            }
        }

        public async Task<OutPutModel<bool>> RegisterStudentAsync(RegisterStudentDTO model)
        {
            if (!ValidateModel.Validate(model, out var validationResult))
            {
                _logger.LogError(validationResult);

                return new OutPutModel<bool>
                {
                    Message = validationResult,
                    Result = false,
                    StatusCode = 400
                };
            }

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                if (await IsExistCodeMelliAsync(model.CodeMelli))
                {
                    return new OutPutModel<bool>
                    {
                         Result=false,
                         StatusCode=400,
                         Message="دانشجو گرامی شما قبلا ثبت نام کردید"
                    };
                }

                var newUser = new User()
                {
                    CodeMelli = model.CodeMelli,
                    Password = PasswordHelper.EncodePasswordSHA1(model.Password),
                    RoleId = model.RoleId
                };
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                var newStuddent = new Student()
                {
                    Email = model.Email,
                    UserId = newUser.UserId,
                    family = model.family,
                    PhoneNumber = model.PhoneNumber,
                    GradeId = model.GradeId,
                    Name = model.Name,


                };
                await _context.Students.AddAsync(newStuddent);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return new OutPutModel<bool>
                {
                    Message = "",
                    Result = true,
                    StatusCode = 200

                };
            }
            catch (Exception ex)
            {

                var errorMessage = "خطای غیرمنتظره ای رخ داد مجدد تلاش کنید.";
                _logger.LogError(errorMessage, ex);
                return new OutPutModel<bool>
                {
                    Message = errorMessage,
                    Result = false,
                    StatusCode = 500

                };
            }
        }

        public async Task<OutPutModel<bool>> RegisterTeacherAsync(RegisterTeacherDTO model)
        {
            if (!ValidateModel.Validate(model, out var validationResult))
            {
                _logger.LogError(validationResult);

                return new OutPutModel<bool>
                {
                    Message = validationResult,
                    Result = false,
                    StatusCode = 400
                };
            }

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                if (await IsExistCodeMelliAsync(model.CodeMelli))
                {
                    return new OutPutModel<bool>
                    {
                        Result = false,
                        StatusCode = 400,
                        Message = "استاد گرامی شما قبلا ثبت نام کردید."
                    };
                }
                var newUser = new User()
                {
                    CodeMelli = model.CodeMelli,
                    Password = PasswordHelper.EncodePasswordSHA1(model.Password),
                    RoleId = model.RoleId
                };
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                var newTeacher = new Teacher()
                {
                    Email = model.Email,
                    UserId = newUser.UserId,
                    family = model.family,
                    PhoneNumber = model.PhoneNumber,
                    TeacherCode = model.TeacherCode,
                    Name = model.Name,


                };
                await _context.Teachers.AddAsync(newTeacher);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return new OutPutModel<bool>
                {
                    Message = "",
                    Result = true,
                    StatusCode = 200

                };
            }
            catch (Exception ex)
            {

                var errorMessage = ".خطای غیرمنتظره ای رخ داد مجدد تلاش کنید";
                _logger.LogError(errorMessage, ex);
                return new OutPutModel<bool>
                {
                    Message = errorMessage,
                    Result = false,
                    StatusCode = 500

                };
            }
        }
    }
}
