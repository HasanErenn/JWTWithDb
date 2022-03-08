using JWTWithDb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWTWithDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase

    {
        public UserDto userDto= new UserDto();   
        private IConfiguration _configuration;
        private DataContext _context;

        public AuthController(IConfiguration configuration, Models.DataContext dataContext)
        {
            _configuration = configuration;
            _context = dataContext;
        }

        [HttpPost("Register")]

        public async Task<ActionResult<UserDto>> Register(User user)
        {
            if (checkUsername(user.Username))
             return BadRequest("Kullanici mevcut"); 

            CreatePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var newUser = new UserDto()
            {
                Username = user.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            _context.UserDtos.Add(newUser);
            _context.SaveChanges();
            return Ok(newUser);
        }
        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(User user)
        {
            var checkUser = _context.UserDtos.SingleOrDefault(x => x.Username == user.Username); 
            //kullanici adinin var olup olmadigini belirliyoruz. eger null degilse kullanicinin bilgilerine erisim saglayabiliyoruz.
            if (checkUser==null)
                return BadRequest("Kullanici bulunamadi");

                if (!verifyPasswordHash(user.Password, checkUser.PasswordHash, checkUser.PasswordSalt))
                    return BadRequest("Sifreniz hatali");
                
            var token= CreateToken(user);
            return Ok(token);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac=new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        private bool verifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac= new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private bool checkUsername(string username)
        {
            return _context.UserDtos.Any(x => x.Username == username);
            //kullanici adinin var olup olmadigini bool ile kontrol etme
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds= new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token =new JwtSecurityToken(
                claims:claims,
                expires:DateTime.Now.AddDays(1),
                signingCredentials:creds);

            var jwt=new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
