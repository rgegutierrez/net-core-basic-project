using CoreJwtExample.IRepository;
using CoreJwtExample.Models;
using CoreJwtExample.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CoreJwtExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _config;
        IUserRepository _oUserRepository = null;
        private readonly IMailService _mailService;

        public UserController(IConfiguration config, IUserRepository oUserRepository, IMailService mailService)
        {
            _config = config;
            _oUserRepository = oUserRepository;
            _mailService = mailService;
        }

        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Registration([FromBody] User model)
        {
            try
            {
                model = await _oUserRepository.Save(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("RequestActivation/{username}")]
        public async Task<IActionResult> RequestActivation(string username)
        {
            try
            {
                Random generator = new Random();
                String password = generator.Next(0, 1000000).ToString("D6");
                User model = new User()
                {
                    Username = username,
                    Password = password
                };

                var user = await ActivationCodeUser(model);
                if (user.UserId == 0) return StatusCode((int)HttpStatusCode.NotFound, "Invalid user");

                UserActivationCode request = new UserActivationCode()
                {
                    ToEmail = user.Email,
                    UserName = user.Username,
                    ActivationCode = user.Password
                };
                await _mailService.SendActivationCodeAsync(request);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("Signin/{username}/{password}")]
        public async Task<IActionResult> Signin(string username, string password)
        {
            try
            {
                User model = new User()
                {
                    Username = username,
                    Password = password
                };

                var user = await AuthenticationUser(model);
                if (user.UserId == 0) return StatusCode((int)HttpStatusCode.NotFound, "Invalid user");
                user.Token = GenerateToken(model);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private string GenerateToken(User model) {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"], 
                _config["Jwt:Issuer"], 
                null, 
                expires: DateTime.Now.AddMinutes(120), 
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<User> ActivationCodeUser(User user)
        {
            var model = await _oUserRepository.GetByUsername(user);
            if (model.UserId == 0) return model;
            model.Password = user.Password;

            return await _oUserRepository.Save(model);
        }

        private async Task<User> AuthenticationUser(User user)
        {
            return await _oUserRepository.GetByUsernamePassword(user);
        }
    }
}
