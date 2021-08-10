using HotelManagement.Core.Security;
using HotelManagement.Data.Dtos;
using HotelManagement.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace HotelManagement.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJWT_TokenGenerator _tokenGenerator;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IJWT_TokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
        }

        
        // POST api/<AuthController>
        [HttpPost("login")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            // check if the login form was correctly filled
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Model", "Please fill out the form correctly");
                return BadRequest(ModelState);
            }

            // check if any user has the inputted email
            AppUser checkUser = await _userManager.FindByEmailAsync(model.Email);
            if (checkUser == null)
            {
                ModelState.AddModelError("Email", "Invalid Credential");
                return NotFound(ModelState);
            }

            // sign out any currently logged in user
            await _signInManager.SignOutAsync();

            // login the user after checking if password is correct
            var checkPassword = await _signInManager.PasswordSignInAsync(checkUser, model.Password, false, false);
            if (!checkPassword.Succeeded)
            {
                ModelState.AddModelError("Password", "Invalid Credential");
                return BadRequest(ModelState);
            }

            var getToken = await _tokenGenerator.GenerateToken(checkUser);

            var token = new LogInResponseDto
            {
                Token = getToken,
            };
            return Ok(token);   
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return Ok();
        }
    }
}
