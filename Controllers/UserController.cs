using AuthenticationWebApi.Data;
using AuthenticationWebApi.Entities;
using AuthenticationWebApi.JWT;
using AuthenticationWebApi.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AuthenticationDBContext _context;
        private readonly JWTSettings _jwtSettings;

        public UserController(AuthenticationDBContext context, IOptions<JWTSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
        }
        


        [HttpGet]
        [Route("login")]
        public ActionResult<UserWithToken> Login([FromBody] User user)
        {
            user = _context.Users.Where(x => x.Login == user.Login && x.Password == x.Password).FirstOrDefault();

            if (user == null) return NotFound();
            
            UserWithToken userWithToken = new(user);

            if (userWithToken == null)
            {
                return NotFound();
            }

            //sign token here
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Login)
                }),
                Expires = DateTime.UtcNow.AddMonths(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            userWithToken.AccessToken = tokenHandler.WriteToken(token);

            var us = new CurrentUsers()
            { 
                Id = userWithToken.Id,
                Token = userWithToken.AccessToken
            };

            var l = new LoginedUser();
            l.Add(us);
            //CurrentUsers.currentUsers.Add(userWithToken);
            return Ok(userWithToken);
        }

        [HttpGet("register")]
        public ActionResult<string> Register([FromBody] User user)
        {
            if(string.IsNullOrEmpty(user.Login) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest();
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("Go to \"api/User/login\" to get your token");
        }


        [Authorize]
        [HttpGet]
        [Route("all")]
        public ActionResult<IEnumerable<User>> GetAll()
        {
            var result = _context.Users;
            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        //[Authorize]
        [HttpGet]
        [Route("currentUsers")]
        public ActionResult<IEnumerable<CurrentUsers>> GetAllCurrentUsers([FromHeader]string auth)
        {
            Console.WriteLine(auth);
            var l = new LoginedUser();
            var result = l.GetAll();
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
