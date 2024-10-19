using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using Team_12.Models;


namespace Team_12.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // GET: api/usermanagement/profile
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && Guid.TryParse(c.Value, out _))?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID is missing or incorrectly formatted in the token.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                UserId = userId,
                user.Name,
                user.Surname,
                user.Email,
                user.PhoneNumber,
                user.Birthday,
                Roles = roles
            });
        }



        // PUT: api/usermanagement/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfile model)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && Guid.TryParse(c.Value, out _))?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID is missing or incorrectly formatted in the token.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            // Update the user's profile information
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Birthday = model.Birthday;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors.Select(e => e.Description));

            return Ok(new { Message = "Profile updated successfully." });
        }



        // PUT: api/usermanagement/admin/profile/{userId}
        [HttpPut("admin/profile/{userId}")]
        public async Task<IActionResult> AdminUpdateProfile(string userId, [FromBody] UpdateProfile model)
        {
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out _))
                return BadRequest("Invalid user ID.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            // Update the user's profile information
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Birthday = model.Birthday;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors.Select(e => e.Description));

            return Ok(new { Message = "Profile updated successfully." });
        }

        // GET: api/usermanagement/all-profiles
        [HttpGet("all-profiles")]
        public async Task<IActionResult> GetAllProfiles()
        {
            var users = _userManager.Users.ToList();

            var userProfiles = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userProfiles.Add(new
                {
                    user.Id,
                    user.Name,
                    user.Surname,
                    user.Email,
                    user.PhoneNumber,
                    user.Birthday,
                    Roles = roles
                });
            }

            return Ok(userProfiles);
        }

        // GET: api/usermanagement/roles
        [HttpGet("roles")]
        public IActionResult GetAllRoles()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return Ok(roles);
        }

        // GET: api/usermanagement/check-roles
        [HttpGet("check-roles")]
        public IActionResult CheckRoles()
        {
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            return Ok(roles);
        }
        // GET: api/usermanagement/clients
        [HttpGet("clients")]
        public async Task<IActionResult> GetAllClients()
        {
            var users = _userManager.Users.ToList();
            var clientProfiles = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Client"))
                {
                    clientProfiles.Add(new
                    {
                        user.Id,
                        user.Name,
                        user.Surname,
                        user.Email,
                        user.PhoneNumber,
                        user.Birthday,
                        Roles = roles
                    });
                }
            }

            return Ok(clientProfiles);
        }

        // GET: api/usermanagement/admins
        [HttpGet("admins")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var users = _userManager.Users.ToList();
            var adminProfiles = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    adminProfiles.Add(new
                    {
                        user.Id,
                        user.Name,
                        user.Surname,
                        user.Email,
                        user.PhoneNumber,
                        user.Birthday,
                        Roles = roles
                    });
                }
            }

            return Ok(adminProfiles);
        }

    }
}