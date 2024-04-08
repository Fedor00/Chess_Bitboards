using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using API.Controllers;
using API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Interfaces;
using UserMicroservice.Repositories;

namespace UserMicroservice.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController<UsersController>
    {
        private readonly IUserRepository _userRepository;

        public UsersController(ILogger<UsersController> logger, IUserRepository userRepository) : base(logger)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetUser(long userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            return Ok(user);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<User>> AddUser(User user)
        {
            _logger.LogInformation("AddUser method called");
            await _userRepository.Add(user);
            await _userRepository.SaveChangesAsync();
            _logger.LogInformation("User added");
            return Ok(user);
        }
    }
}
