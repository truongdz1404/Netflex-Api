using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Netflex.Entities;
using Netflex.Models;
using Netflex.Models.User;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList.Extensions;
using OfficeOpenXml;

namespace Netflex.Controllers
{
    public class UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private const int PAGE_SIZE = 5;

        // GET: UserController
        [Route("/dashboard/user")]
        public async Task<IActionResult> Index(
            int? page, string searchTerm,
            string userId, string userName,
            string email, string phoneNumber,
            string selectedRoles, string sortOrder)
        {
            int pageNumber = page ?? 1;

            var users = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u => (u.UserName != null && u.UserName.Contains(searchTerm)) ||
                                         (u.Email != null && u.Email.Contains(searchTerm)) ||
                                         (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)));
            }

            if (!string.IsNullOrEmpty(userName))
            {
                users = users.Where(u => !string.IsNullOrEmpty(u.UserName) && u.UserName.Contains(userName));
            }

            if (!string.IsNullOrEmpty(email))
            {
                users = users.Where(u => !string.IsNullOrEmpty(u.Email) && u.Email.Contains(email));
            }

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                users = users.Where(u => !string.IsNullOrEmpty(u.PhoneNumber) && u.PhoneNumber.Contains(phoneNumber));
            }

            switch (sortOrder)
            {
                case "userName_asc":
                    users = users.OrderBy(u => u.UserName);
                    break;
                case "userName_desc":
                    users = users.OrderByDescending(u => u.UserName);
                    break;
                case "email_asc":
                    users = users.OrderBy(u => u.Email);
                    break;
                case "email_desc":
                    users = users.OrderByDescending(u => u.Email);
                    break;
                default:
                    users = users.OrderBy(u => u.UserName);
                    break;
            }

            var usersList = await users.ToListAsync();

            var models = new List<UserViewModels>();
            foreach (var user in usersList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                models.Add(new UserViewModels()
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

            var pagedModels = models.ToPagedList(pageNumber, PAGE_SIZE);

            return View("~/Views/Dashboard/User/Index.cshtml", pagedModels);
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var users = _userManager.Users.AsQueryable();
            var usersList = await users.ToListAsync();

            var models = new List<UserViewModels>();
            foreach (var user in usersList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                models.Add(new UserViewModels()
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
            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Users");

                // Thêm các tiêu đề cột
                worksheet.Cells[1, 1].Value = "User ID";
                worksheet.Cells[1, 2].Value = "User Name";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Phone Number";
                worksheet.Cells[1, 5].Value = "Role";
                worksheet.Cells[1, 6].Value = "Status";

                // Thêm dữ liệu vào các hàng
                for (int i = 0; i < models.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = models[i].Id;
                    worksheet.Cells[i + 2, 2].Value = models[i].UserName;
                    worksheet.Cells[i + 2, 3].Value = models[i].Email;
                    worksheet.Cells[i + 2, 4].Value = models[i].PhoneNumber;
                    worksheet.Cells[i + 2, 5].Value = models[i].Roles;

                    var status = models[i].LockoutEnabled ? "Non-Active" : "Active";
                    worksheet.Cells[i + 2, 6].Value = status;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var fileContent = package.GetAsByteArray();
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UserData.xlsx");
            }
        }

        // GET: UserController/Details/5
        [Route("/dashboard/user/detail/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var model = new DetailsUserViewModels
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = roles.ToList(),
                LockoutEnd = user.LockoutEnd,
                LockoutEnabled = user.LockoutEnabled
            };

            return View("~/Views/Dashboard/User/Details.cshtml", model);
        }

        // GET: UserController/Create
        [Route("/dashboard/user/create")]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var model = new UserViewModels
            {
                AvailableRoles = roles.Select(r => r.Name ?? string.Empty).ToList()
            };
            return View("~/Views/Dashboard/User/Create.cshtml", model);
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/dashboard/user/create")]
        public async Task<IActionResult> Create(UserViewModels model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password ?? string.Empty);
                if (result.Succeeded)
                {
                    if (model.SelectedRoles != null && model.SelectedRoles.Any())
                    {
                        var addRoleResult = await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                        if (!addRoleResult.Succeeded)
                        {
                            foreach (var error in addRoleResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            var roles = await _roleManager.Roles.ToListAsync();
            model.AvailableRoles = roles.Select(r => r.Name ?? string.Empty).ToList();
            return View(model);
        }


        // GET: UserController/Edit/5
        [Route("/dashboard/user/edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToListAsync();
            var currentRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModels
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AvailableRoles = roles.Select(r => r.Name ?? string.Empty).ToList(),
                SelectedRoles = currentRoles.ToList()
            };
            ViewBag.SelectedRoles = model.SelectedRoles;
            return View("~/Views/Dashboard/User/Edit.cshtml", model);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/dashboard/user/edit/{id}")]
        public async Task<IActionResult> Edit(string id, EditUserViewModels model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    if (model.SelectedRoles != null && model.SelectedRoles.Any())
                    {
                        // Remove current roles
                        var currentRoles = await _userManager.GetRolesAsync(user);
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);

                        // Add new roles
                        var addRoleResult = await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                        if (!addRoleResult.Succeeded)
                        {
                            foreach (var error in addRoleResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            var roles = await _roleManager.Roles.ToListAsync();
            model.AvailableRoles = roles.Select(r => r.Name ?? string.Empty).ToList();
            return View("~/Views/Dashboard/User/Edit.cshtml", model);
        }

        [Route("/dashboard/user/lock/{id}")]
        public async Task<IActionResult> Lock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.SetLockoutEnabledAsync(user, true);
            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(99));
            // if (result.Succeeded)
            // { 
            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
            // }

        }

        // GET: UserController/Unlock/5
        [Route("/dashboard/user/unlock/{id}")]
        public async Task<IActionResult> Unlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.SetLockoutEndDateAsync(user, null);
            await _userManager.SetLockoutEnabledAsync(user, false);

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }


    }
}
