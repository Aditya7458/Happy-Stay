﻿using Cozy.Models;
using Cozy.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cozy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/User/NonAdminUsers
        [HttpGet("NonAdminUsers")]
        public async Task<IActionResult> GetNonAdminUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();

            // Filter out users with the "Admin" role
            var nonAdminUsers = users.Where(user => user.Role != "Admin").ToList();

            return Ok(nonAdminUsers);
        }
    }
}
