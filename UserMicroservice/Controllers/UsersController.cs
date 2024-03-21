using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.DTOs;
using UserMicroservice.Entities;

namespace UserMicroservice.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMapper mapper, UserManager<User> userManager, IHttpClientFactory httpClientFactory, ILogger<UsersController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _mapper = mapper;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }
        [HttpGet("admins")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<IEnumerable<AdminDto>>> GetAdmins()
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync("Manager");
            return Ok(_mapper.Map<IEnumerable<AdminDto>>(adminUsers));
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUser(long userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> AddUser(UserDto userObj)
        {
            _logger.LogInformation("AddUser method called");

            var user = _mapper.Map<User>(userObj);
            user.Email = userObj.Email.ToLower();
            var result = await _userManager.CreateAsync(user, "TrashTalker00@");

            if (!result.Succeeded) return BadRequest(result.Errors);
            _logger.LogInformation("AddUser method called and user created.");

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);


            var userIdJson = JsonSerializer.Serialize(new { Id = user.Id });
            var content = new StringContent(userIdJson, Encoding.UTF8, "application/json");
            string token = Request.Headers["Authorization"];
            if (token != null && token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync("http://localhost:5280/api/users", content);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("AddUser method called and user created succesfully.");
                return Ok(_mapper.Map<UserDto>(user));
            }

            // If failed to create the user in the external system, you might want to delete the user created locally.
            await _userManager.DeleteAsync(user);

            return BadRequest(await response.Content.ReadAsStringAsync());
        }


        [HttpPut]
        public async Task<ActionResult<UserDto>> UpdateUser(UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id.ToString());

            if (user == null) return NotFound("User not found");

            user.UserName = userDto.UserName;
            user.Email = userDto.Email;
            user.PhoneNumber = userDto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUser(long userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) return NotFound("User not found");


            string token = Request.Headers["Authorization"];
            if (token != null && token.StartsWith("Bearer "))
            {
                token = token.Substring("Bearer ".Length).Trim();
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"http://devicemicroservice:80/api/users/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded) return BadRequest(result.Errors);

                return Ok("Successfully deleted");
            }
            else
            {
                return BadRequest(await response.Content.ReadAsStringAsync());
            }
        }

    }
}
