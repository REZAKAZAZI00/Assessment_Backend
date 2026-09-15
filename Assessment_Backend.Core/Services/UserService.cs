namespace Assessment_Backend.Core.Servies
{
    public class UserService : IUserService
    {
        #region Constructor
        private readonly AssessmentDbContext _context;
        private readonly ITokenHelperService _tokenHelperService;
        private readonly ILogger<UserService> _logger;

        public UserService(AssessmentDbContext context, ILogger<UserService> logger, ITokenHelperService tokenHelperService)
        {
            _context = context;
            _logger = logger;
            _tokenHelperService = tokenHelperService;
        }

        #endregion

        public async Task<bool> IsExistCodeMelliAsync(string code)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.CodeMelli == code);
        }

        public async Task<OutPutModel<UserProfileDTO>> LoginAsync(LoginDTO model)
        {
            ValidateModel.ValidateOrThrow(model);

            string password = PasswordHelper.EncodePasswordSHA1(model.Password);

            var existingUser = await _context.Users
               .AsNoTracking()
               .SingleOrDefaultAsync(u => u.CodeMelli == model.CodeMelli && u.Password == password);

            if (existingUser is null)
            {
                _logger.LogWarning("Failed login attempt for CodeMelli {CodeMelli}", model.CodeMelli);
                throw new BusinessException("اطلاعات وارد شده صحیح نمی باشد.", 400);
            }

            var userProfile = new UserProfileDTO
            {
                UserId = existingUser.UserId,
                CodeMelli = existingUser.CodeMelli,
                Role = (DTOs.Account.RoleDTO)existingUser.Role,
                Token = ""
            };

            var student = await _context.Students
                .AsNoTracking()
                .Include(g => g.Grade)
                .SingleOrDefaultAsync(s => s.UserId == existingUser.UserId);

            if (student != null)
            {
                userProfile.Name = student.Name;
                userProfile.Email = student.Email;
                userProfile.PhoneNumber = student.PhoneNumber;
                userProfile.Grade = student.Grade!.Title;
                userProfile.family = student.family;
                userProfile.StudentId = student.StudentId;
                userProfile.Token = _tokenHelperService.GenerateToken<Student>(existingUser, student);
            }
            else
            {
                var teacher = await _context.Teachers
                    .AsNoTracking()
                    .SingleOrDefaultAsync(t => t.UserId == existingUser.UserId);

                if (teacher != null)
                {
                    userProfile.TeacherId = teacher.TeacherId;
                    userProfile.Name = teacher.Name;
                    userProfile.family = teacher.family;
                    userProfile.Email = teacher.Email;
                    userProfile.PhoneNumber = teacher.PhoneNumber;
                    userProfile.TeacherCode = teacher.TeacherCode;
                    userProfile.Token = _tokenHelperService.GenerateToken<Teacher>(existingUser, teacher);
                }
            }

            return new OutPutModel<UserProfileDTO>
            {
                Result = userProfile,
                StatusCode = 200,
                Message = ""
            };
        }

        public async Task<OutPutModel<bool>> RegisterStudentAsync(RegisterStudentDTO model)
        {
            ValidateModel.ValidateOrThrow(model);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (await IsExistCodeMelliAsync(model.CodeMelli))
                throw new BusinessException("دانشجو گرامی شما قبلا ثبت نام کردید", 403);

            var newUser = new User()
            {
                CodeMelli = model.CodeMelli,
                Password = PasswordHelper.EncodePasswordSHA1(model.Password),
                Role = (Role)model.Role
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            var newStudent = new Student()
            {
                Email = model.Email,
                UserId = newUser.UserId,
                family = model.family,
                PhoneNumber = model.PhoneNumber,
                GradeId = model.GradeId,
                Name = model.Name,
            };
            await _context.Students.AddAsync(newStudent);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new OutPutModel<bool>
            {
                Message = "دانشجو گرامی ثبت نام شما با موفقیت انجام شد.",
                Result = true,
                StatusCode = 200
            };
        }

        public async Task<OutPutModel<bool>> RegisterTeacherAsync(RegisterTeacherDTO model)
        {
            ValidateModel.ValidateOrThrow(model);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (await IsExistCodeMelliAsync(model.CodeMelli))
                throw new BusinessException("استاد گرامی شما قبلا ثبت نام کردید.", 403);

            var newUser = new User()
            {
                CodeMelli = model.CodeMelli,
                Password = PasswordHelper.EncodePasswordSHA1(model.Password),
                Role = (Role)model.Role
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
                Message = "استاد گرامی ثبت نام شما با موفقیت انجام شد.",
                Result = true,
                StatusCode = 200
            };
        }
    }
}
