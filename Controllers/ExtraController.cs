using Cozy.Models;
using Cozy.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cozy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExtraController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public ExtraController(IUserRepository userRepository)
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
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var isDeleted = await _userRepository.DeleteUserAsync(id);

            if (!isDeleted)
            {
                return NotFound(new { message = "User not found or already deleted." });
            }

            return Ok(new { message = "User deleted successfully." });
        }

    }
}
