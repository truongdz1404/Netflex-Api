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

            var models = await users.Select(b => new UserViewModels()
            {
                Id = b.Id,
                UserName = b.UserName,
                Email = b.Email,
                PhoneNumber = b.PhoneNumber,
                LockoutEnd = b.LockoutEnd,
                LockoutEnabled = b.LockoutEnabled,
            }).ToListAsync();

            var pagedModels = models.ToPagedList(pageNumber, PAGE_SIZE);

            return View(pagedModels);
        }
        public IActionResult ExportToExcel()
        {
            var users = _userManager.Users.AsQueryable();
            var userList = users.Select(u => new UserViewModels
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber
            }).ToList();

            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Users");

                // Thêm các tiêu đề cột
                worksheet.Cells[1, 1].Value = "User ID";
                worksheet.Cells[1, 2].Value = "User Name";
                worksheet.Cells[1, 3].Value = "Email";
                worksheet.Cells[1, 4].Value = "Phone Number";

                // Thêm dữ liệu vào các hàng
                for (int i = 0; i < userList.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = userList[i].Id;
                    worksheet.Cells[i + 2, 2].Value = userList[i].UserName;
                    worksheet.Cells[i + 2, 3].Value = userList[i].Email;
                    worksheet.Cells[i + 2, 4].Value = userList[i].PhoneNumber;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var fileContent = package.GetAsByteArray();
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "UserData.xlsx");
            }
        }


        // public async Task<IActionResult> Index(int? page)
        // {
        //     int pageNumber = page ?? 1;

        //     // Lấy tất cả người dùng
        //     var users = await _userManager.Users.ToListAsync();

        //     // Tạo danh sách các tác vụ bất đồng bộ để lấy vai trò cho từng người dùng
        //     var userViewModelsTasks = users.Select(async b =>
        //     {
        //         var roles = await _userManager.GetRolesAsync(b);  // Lấy các vai trò cho mỗi người dùng
        //         return new UserViewModels()
        //         {
        //             Id = b.Id,
        //             UserName = b.UserName,
        //             Email = b.Email,
        //             PhoneNumber = b.PhoneNumber,
        //             SelectedRoles = roles.ToList()
        //         };
        //     }).ToList();

        //     // Đợi tất cả các tác vụ bất đồng bộ hoàn thành
        //     var userViewModels = await Task.WhenAll(userViewModelsTasks);

        //     // Phân trang dữ liệu
        //     var pagedUsers = userViewModels.ToPagedList(pageNumber, PAGE_SIZE);

        //     return View(pagedUsers);  // Trả về view với dữ liệu đã phân trang
        // }

        // GET: UserController/Details/5
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

            return View(model);
        }

        // GET: UserController/Create
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var model = new UserViewModels
            {
                AvailableRoles = roles.Select(r => r.Name ?? string.Empty).ToList()
            };
            return View(model);
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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

                // Create user with password
                var result = await _userManager.CreateAsync(user, model.Password ?? string.Empty);
                if (result.Succeeded)
                {
                    // Add selected roles
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

            // If model state is invalid or any error occurs, return view
            var roles = await _roleManager.Roles.ToListAsync();
            model.AvailableRoles = roles.Select(r => r.Name ?? string.Empty).ToList();
            return View(model);
        }


        // GET: UserController/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToListAsync();
            var currentRoles = await _userManager.GetRolesAsync(user);

            var model = new UserViewModels
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AvailableRoles = roles.Select(r => r.Name ?? string.Empty).ToList(),
                SelectedRoles = currentRoles.ToList()
            };

            return View(model);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserViewModels model)
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
            return View(model);
        }

        // // GET: UserController/Lock/5
        // public async Task<IActionResult> Lock(string id)
        // {
        //     var user = await _userManager.FindByIdAsync(id);
        //     if (user == null)
        //     {
        //         return NotFound();
        //     }

        //     var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(1));
        //     if (result.Succeeded)
        //     {
        //         return RedirectToAction(nameof(Index));
        //     }
        //     return View();
        // }

        // // GET: UserController/Unlock/5
        // public async Task<IActionResult> Unlock(string id)
        // {
        //     var user = await _userManager.FindByIdAsync(id);
        //     if (user == null)
        //     {
        //         return NotFound();
        //     }

        //     var result = await _userManager.SetLockoutEndDateAsync(user, null);
        //     if (result.Succeeded)
        //     {
        //         return RedirectToAction(nameof(Index));
        //     }
        //     return View();
        // }
        // GET: UserController/Lock/5
        public async Task<IActionResult> Lock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(1));
            // if (result.Succeeded)
            // { 
            await _userManager.SetLockoutEnabledAsync(user, true);
            return RedirectToAction(nameof(Index));
            // }

        }

        // GET: UserController/Unlock/5
        public async Task<IActionResult> Unlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // var result = await _userManager.SetLockoutEndDateAsync(user, null);
            // if (result.Succeeded)
            // {
            await _userManager.SetLockoutEnabledAsync(user, false);

            return RedirectToAction(nameof(Index));
            // }

        }

    }
}
