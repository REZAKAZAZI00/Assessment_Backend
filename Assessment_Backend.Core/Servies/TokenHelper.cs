namespace Assessment_Backend.Core.Servies
{
    public class TokenHelper : ITokenHelperService
    {
        private readonly IConfiguration _configuration;
        public TokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
       
        public string GenerateToken<T>(User user, T entity) where T : class
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>
            {
                new Claim("userId", user.UserId.ToString()),
                new Claim("RoleId", user.RoleId.ToString()),
                new Claim("CodeMelli",user.CodeMelli)

            };

            // Check if the entity is a student
            if (entity is Student student)
            {
                claimsForToken.Add(new Claim("StudentId", student.StudentId.ToString()));
            }

            // Check if the entity is a teacher
            else if (entity is Teacher teacher)
            {
                claimsForToken.Add(new Claim("TeacherId", teacher.TeacherId.ToString()));
            }

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(30.0),
                signingCredentials
            );

            var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return tokenToReturn;
        }

    }
}
