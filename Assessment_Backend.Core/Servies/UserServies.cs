

using Assessment_Backend.Core.Security;
using Assessment_Backend.DataLayer.Entities.User;

namespace Assessment_Backend.Core.Servies
{
    public class UserServies : IUserServies
    {
        private readonly AssessmentDbContext _context;
        private readonly ILogger<UserServies> _logger;

        public UserServies(AssessmentDbContext context, ILogger<UserServies> logger)
        {
            _context = context;
            _logger = logger;
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
                var errorMessage = "Unexpected error occurred";
                _logger.LogError(errorMessage, ex);
                return new OutPutModel<List<RoleDTO>>
                {
                    Message = errorMessage,
                    Result = null,
                    StatusCode = 500

                };
            }
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

                var exsitingUser = await _context.Users
                   .SingleOrDefaultAsync(u => u.CodeMelli == model.CodeMelli && u.Password == password);
                return new OutPutModel<UserProfileDTO>
                {

                };

            }
            catch (Exception ex)
            {

                var errorMessage = "Unexpected error occurred";
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

                var errorMessage = "Unexpected error occurred";
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

                var errorMessage = "Unexpected error occurred";
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
