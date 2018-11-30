using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using auth.API.Data;
using auth.API.Dtos;
using auth.API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace auth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IAuthRepository _repo ;
        private readonly SignInManager<User>  _signInManager ; 
        private readonly UserManager<User> _userManager;
        public AuthController(IAuthRepository repo ,
         UserManager<User> userManager ,
          SignInManager<User> signInManager ,
           IConfiguration config ,
           IMapper mapper )
    {
        _config = config ;
        _repo = repo;
        _userManager = userManager ;
        _signInManager = signInManager;
        _mapper = mapper;
 
    }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            
            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);

           await _userManager.AddToRoleAsync(userToCreate , "Doctor");
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
             var user = await _userManager.FindByNameAsync(userForLoginDto.Username);
             var result = await _signInManager.CheckPasswordSignInAsync(user, userForLoginDto.Password, false);

               if (result.Succeeded)
            {
                 var userras = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == userForLoginDto.Username.ToUpper());

                return Ok(new
                {
                    token = GenerateJwtToken(userras)
                });
            }
             
             return Unauthorized();


        }

         private async Task<string> GenerateJwtToken(User user)
        {
            
             var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}




//FromBody tells the api where to find the data in the body of the request 