using LoginJwt.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Logging.Console;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace LoginJwt.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly AppDBContext _dbContext;
        private readonly IConfiguration _configuration;

        public ValuesController(AppDBContext dBContext,IConfiguration configuration)
        {
            _dbContext = dBContext;
            _configuration = configuration;
        }

        [HttpPost]

        [Route("Registration")]

        public IActionResult Registration(UserDTO userDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var checkuser = _dbContext.Users.FirstOrDefault(options => options.Email == userDTO.Email);
            if(checkuser == null) {
                _dbContext.Users.Add(new models.User
                {
                    FirstName = userDTO.FirstName,
                    LastName = userDTO.LastName,
                    Email = userDTO.Email,
                    Password = userDTO.Password
                });
                _dbContext.SaveChanges();
                return Ok("Users Created Successfully");
            }
            else
            {
                return Ok("Users Already exists");
            }
        }

        [HttpPost]
        [Route("Login")]

        public IActionResult Login(LoginDTO loginDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loginuser = _dbContext.Users.FirstOrDefault(x => x.Email == loginDTO.Email && x.Password == loginDTO.Password);

            if (loginuser != null)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub,_configuration["Jwt:Subject"]),
                     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                     new Claim("Users",loginuser.UserId.ToString()),
                     new Claim("Email",loginuser.Email.ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims , expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: credentials
                );

                string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new { Token = jwtToken, User = loginuser });
            }
            return NoContent();
        }

        [HttpGet]
        [Route("GetAllUsers")]

        public IActionResult GetAllUsers()
        {
            var res = _dbContext.Users.ToList();
            return Ok(res);
        }

        [Authorize]
        [HttpGet]
        [Route("GetUserbyId")]
       
        public IActionResult GetUser(int id)
        {
            var byid = _dbContext.Users.FirstOrDefault(x => x.UserId == id);
            if (byid != null)
            {
                return Ok(byid);
            }
            else
            {
                return NoContent();
            }
        }
    }

   

}
