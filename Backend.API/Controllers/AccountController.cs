using Backend.API.Contracts;
using Backend.API.Helpers;
using Backend.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        public IConfiguration _configuration;
        private readonly IUserRepository _userRepo;

        public AccountController(IConfiguration configuration, IUserRepository userRepo)
        {
            _configuration = configuration;
            _userRepo = userRepo;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (email != null && password != null)
            {
                var user = await _userRepo.CheckLogin(email, password);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id", user.Id.ToString()),
                        new Claim("Email", user.Email),
                        new Claim("Username", user.Username),
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserModel user)
        {
            try
            {
                if (user.Email != null && user.Password != null) 
                {
                    var userDb = await _userRepo.Register(user);

                    return Ok(userDb);
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }


        // UPDATE api/<AccountController>/
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserModel user)
        {
            try
            {
                if (user != null)
                {
                    var userDb = await _userRepo.UpdateUser(user);

                    return Ok(userDb);
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }


        // DELETE api/<AccountController>/email
        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string email)
        {
            try
            {
                if (email != null)
                {
                    var userDb = await _userRepo.DeleteUser(email);

                    return Ok(userDb);
                }
                return BadRequest();

            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }
    }
}
