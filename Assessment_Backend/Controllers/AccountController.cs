namespace Assessment_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userServies;
        public AccountController(IUserService userServies)
        {
            _userServies = userServies;
        }


        [HttpPost("Login")]
        public async Task<ActionResult<OutPutModel<UserProfileDTO>>> Login(LoginDTO model)
        {
            var result= await _userServies.LoginAsync(model);    
            return result;
        }


        [HttpPost("RegisterTeacher")]
        public async Task<ActionResult<OutPutModel<bool>>> RegisterTeacher(RegisterTeacherDTO model)
        {
            var result=await _userServies.RegisterTeacherAsync(model);

            return result;
        }


        [HttpPost("RegisterStudent")]
        public async Task<ActionResult<OutPutModel<bool>>> RegisterStudent(RegisterStudentDTO register)
        {
           var result=await _userServies.RegisterStudentAsync(register);
            return result;
        }

        [HttpGet("GetRoles")]
        public async Task<ActionResult<OutPutModel<List<RoleDTO>>>> GetRoles()
        {
            var result=await _userServies.GetAllRolesAsync();
            return result;
        }
    }
}
