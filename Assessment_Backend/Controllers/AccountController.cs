namespace Assessment_Backend.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        #region Constructor
        private readonly IUserService _userServies;
        public AccountController(IUserService userServies)
        {
            _userServies = userServies;
        }
        #endregion


        /// <summary>
        /// لاگین استاد و دانشجو
        /// </summary>
        /// <param name="model">شامل کدملی و کلمه عبور</param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<OutPutModel<UserProfileDTO>>> Login(LoginDTO model)
        {
            var result= await _userServies.LoginAsync(model);    
            return result;
        }

        /// <summary>
        /// ثبت نام استاد
        /// </summary>
        /// <param name="model">شامل مشخصات استاد</param>
        /// <returns></returns>
        [HttpPost("teacher")]
        public async Task<ActionResult<OutPutModel<bool>>> RegisterTeacher(RegisterTeacherDTO model)
        {
            var result=await _userServies.RegisterTeacherAsync(model);

            return result;
        }

        /// <summary>
        /// ثبت نام دانشجو 
        /// </summary>
        /// <param name="register">مشخصات دانشجو</param>
        /// <returns></returns>
        [HttpPost("student")]
        public async Task<ActionResult<OutPutModel<bool>>> RegisterStudent(RegisterStudentDTO register)
        {
           var result=await _userServies.RegisterStudentAsync(register);
            return result;
        }

        /// <summary>
        /// گرفتن نقش ها
        /// </summary>
        /// <returns></returns>
        [HttpGet("roles")]
        public async Task<ActionResult<OutPutModel<List<RoleDTO>>>> GetRoles()
        {
            var result=await _userServies.GetAllRolesAsync();
            return result;
        }
    }
}
