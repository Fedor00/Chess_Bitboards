using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.DTOs;
using UserMicroservice.Entities;
using UserMicroservice.Interfaces;

namespace UserMicroservice.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        public AccountController(IMapper mapper, ITokenService tokenService, UserManager<User> userManager)
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _userManager = userManager;
        }
        [HttpPost("register")] //POST api/account/register
        public async Task<ActionResult<AccountDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Email)) return BadRequest("Account already exists");
            var user = _mapper.Map<User>(registerDto);
            user.Email = registerDto.Email.ToLower();
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);
            var accountDto = _mapper.Map<AccountDto>(user);
            accountDto.Token = await _tokenService.CreateToken(user);
            accountDto.Roles = await _userManager.GetRolesAsync(user);
            return Ok(accountDto);
        }
        [HttpPost("login")]
        public async Task<ActionResult<AccountDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x => x.Email == loginDto.Email);
            if (user == null) return Unauthorized("Invalid email.");
            var validPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!validPassword) return Unauthorized("Password does not match.");
            var userDto = _mapper.Map<AccountDto>(user);
            userDto.Token = await _tokenService.CreateToken(user);
            userDto.Roles = await _userManager.GetRolesAsync(user);
            return Ok(userDto);
        }
        private async Task<bool> UserExists(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }
    }
}