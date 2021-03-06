﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobsityChatroom.WebAPI.Models;
using JobsityChatroom.WebAPI.Models.Authentication;
using JobsityChatroom.WebAPI.Models.Chatroom;
using JobsityChatroom.WebAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobsityChatroom.WebAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthenticationController(IUserService userService,
            ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] AuthViewModel authModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _userService.UserExists(authModel.Username))
                return BadRequest("User doesn't exist.");

            var authResult = await _userService.Login(authModel);
            if (authResult == null)
                return Unauthorized("Invalid username/password");

            var accessToken = _tokenService.GetAccessToken(authResult);

            return Ok(BuildResponse(authResult, accessToken));
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] SignupViewModel authModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userService.UserExists(authModel.Username))
                return BadRequest("User already exists.");
            
            var authResult = await _userService.Register(authModel);
            if (authResult == null)
                return Unauthorized("Failed to register user");

            var accessToken = _tokenService.GetAccessToken(authResult);

            return Ok(BuildResponse(authResult, accessToken));
        }

        private AuthResponse BuildResponse(IdentityUser user, string accessToken)
        {
            var response = new AuthResponse
            {
                AccessToken = accessToken,
                LoggedUser = new UserViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Email = user.Email
                }
            };
            return response;
        }
    }
}
