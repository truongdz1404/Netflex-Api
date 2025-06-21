using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflex.Entities;
using Netflex.Models.User;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Netflex.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserViewModels>>> GetUsers(
            string? searchTerm = null,
            string? userName = null,
            string? email = null,
            string? phoneNumber = null,
            string? sortOrder = "userName_asc")
        {
            var users = _userManager.Users.AsQueryable();
            if (!string.IsNullOrEmpty(userName))
                users = users.Where(u => u.UserName.Contains(userName));
            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u => u.UserName.Contains(searchTerm) || u.Email.Contains(searchTerm) || u.PhoneNumber.Contains(searchTerm));
            }
           
            if (!string.IsNullOrEmpty(email))
                users = users.Where(u => u.Email.Contains(email));
            if (!string.IsNullOrEmpty(phoneNumber))
                users = users.Where(u => u.PhoneNumber.Contains(phoneNumber));

            users = sortOrder switch
            {
                "userName_desc" => users.OrderByDescending(u => u.UserName),
                "email_asc" => users.OrderBy(u => u.Email),
                "email_desc" => users.OrderByDescending(u => u.Email),
                _ => users.OrderBy(u => u.UserName)
            };

            var userList = await users.ToListAsync();
            var models = new List<UserViewModels>();

            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                models.Add(new UserViewModels
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    LockoutEnd = user.LockoutEnd,
                    LockoutEnabled = user.LockoutEnabled,
                    Roles = string.Join(", ", roles)
                });
            }

            return Ok(models);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DetailsUserViewModels>> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new DetailsUserViewModels
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                LockoutEnd = user.LockoutEnd,
                LockoutEnabled = user.LockoutEnabled,
                Roles = roles.ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModels model)
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password ?? string.Empty);
            if (!result.Succeeded) return BadRequest(result.Errors);

            if (model.SelectedRoles != null && model.SelectedRoles.Any())
            {
                var roleResult = await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);
            }

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, EditUserViewModels model)
        {
            if (id != model.Id) return BadRequest("Invalid ID");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded) return BadRequest(updateResult.Errors);

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (model.SelectedRoles != null && model.SelectedRoles.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                if (!addResult.Succeeded) return BadRequest(addResult.Errors);
            }

            return Ok();
        }

        [HttpPost("lock/{id}")]
        public async Task<IActionResult> Lock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(99));
            await _userManager.UpdateAsync(user);

            return Ok();
        }

        [HttpPost("unlock/{id}")]
        public async Task<IActionResult> Unlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.SetLockoutEnabledAsync(user, false);
            await _userManager.SetLockoutEndDateAsync(user, null);
            await _userManager.UpdateAsync(user);

            return Ok();
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportToExcel()
        {
            var users = await _userManager.Users.ToListAsync();
            var models = new List<UserViewModels>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                models.Add(new UserViewModels
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    LockoutEnabled = user.LockoutEnabled,
                    Roles = string.Join(", ", roles)
                });
            }

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Users");
            worksheet.Cells[1, 1].Value = "User ID";
            worksheet.Cells[1, 2].Value = "User Name";
            worksheet.Cells[1, 3].Value = "Email";
            worksheet.Cells[1, 4].Value = "Phone Number";
            worksheet.Cells[1, 5].Value = "Role";
            worksheet.Cells[1, 6].Value = "Status";

            for (int i = 0; i < models.Count; i++)
            {
                var status = models[i].LockoutEnabled ? "Non-Active" : "Active";
                worksheet.Cells[i + 2, 1].Value = models[i].Id;
                worksheet.Cells[i + 2, 2].Value = models[i].UserName;
                worksheet.Cells[i + 2, 3].Value = models[i].Email;
                worksheet.Cells[i + 2, 4].Value = models[i].PhoneNumber;
                worksheet.Cells[i + 2, 5].Value = models[i].Roles;
                worksheet.Cells[i + 2, 6].Value = status;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            var fileBytes = package.GetAsByteArray();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Users.xlsx");
        }
    }
}
