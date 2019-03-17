using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repository;

        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository repository,IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto model)
        {
            //validate the user

            model.Username = model.Username.ToLower();

            if (await _repository.UserExists(model.Username))
                return BadRequest("Username already exits");

            var userToCreate = new User
            {
                Username = model.Username
            };

            var createdUser = await _repository.Register(userToCreate, model.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto model)
        {
            var userFromRepo = await _repository.Login(model.Username.ToLower(), model.Password);

            if (userFromRepo == null)
                return Unauthorized();

            //Building token to retrun to user
            //our token gonnan contain two bit of info
            //userID,userName (we can add more if we want)

            //we are gonna create a var of type arry
            var claims = new[]
            {
                //Select a claim type for Id of user and set its value
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                //Create second claim and select name type to store username
                new Claim(ClaimTypes.Name,userFromRepo.Username)
            };

            //Now we also gonna need a key to assign to our token (type of hash)
            //So the key we gonna hash is we are going to set it in appsetting.json and get it from configration by inducing DI to configuration
            var key = new SymmetricSecurityKey(Encoding
                .UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            //Now genrate some singing creationals

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //now we need create security security token descriptor
            //which is gonna contain our 
            //claims , expair date  and singing creadtionals
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = cred
            };

            //now we need token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            //now we will create our token 
            var token = tokenHandler.CreateToken(tokenDescriptor);//so this gonna contain our jwt token
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}



