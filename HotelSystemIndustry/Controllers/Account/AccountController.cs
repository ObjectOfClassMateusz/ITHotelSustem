using System.Text;
using HotelSystemIndustry.Infrastructure;
using HotelSystemIndustry.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol;

namespace HotelSystemIndustry.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure:false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Hotels");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    if (await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        return RedirectToAction("RegisterConfirmation", "Account", new { email = model.Email });
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt!");
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName,
                UserName = model.Email,
                NormalizedUserName = model.Email.ToUpper(),
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("RegisterConfirmation", "Account", new {email = model.Email});
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(PasswordChangingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Hotels");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Hotels");
        }

        [HttpGet]
        public async Task<IActionResult> AccountInfo()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> RegisterConfirmation(string email)
        {
            if (email == null)
            {
                return RedirectToAction("Index", "Hotels");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to find user with email: {email}.");
            }

            var userId = user.Id;

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action(
                "ConfirmEmail",
                "Account",
                new { userId = userId, code = code },
                Request.Scheme);

            return View(new RegisterConfirmationModel {ConfirmationUrl = callbackUrl ?? ""});
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to find user by id.");
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return BadRequest("Invalid email confirmation token.");
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageUsers([FromQuery] UserQueryModel queryModel)
        {
            var usersQuery = _userManager.Users.AsNoTracking();

            if (!string.IsNullOrEmpty(queryModel.FullNameFilter))
                usersQuery = usersQuery.Where(u => u.FullName.ToUpper().Contains(queryModel.FullNameFilter.ToUpper()));

            if (!string.IsNullOrEmpty(queryModel.EmailFilter))
                usersQuery = usersQuery.Where(u => (u.Email ?? "").ToUpper().Contains(queryModel.EmailFilter.ToUpper()));

            var users = await usersQuery
                .OrderBy(u => u.FullName)
                .ToListAsync();


            UserManagementModel model = new UserManagementModel();

            foreach (var user in users)
            {
                UserManagementInfo userInfo = new UserManagementInfo
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? "",
                    EmailVerified = user.EmailConfirmed
                };

                var roles = await _userManager.GetRolesAsync(user);

                foreach (var role in roles)
                {
                    userInfo.Roles.Add(role);
                }

                model.Users.Add(userInfo);
            }

            ViewBag.UserQueryModel = queryModel;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Could not find user to delete: {id}");
            }

            var roles = await _roleManager.Roles.AsNoTracking().ToListAsync();

            UserRolesEditViewModel model = new UserRolesEditViewModel
            {
                Id = id
            };

            foreach (var role in roles)
            {
                model.Roles.Add(new UserRolesEditRole
                {
                    Name = role.Name ?? "",
                    HasRole = await _userManager.IsInRoleAsync(user, role.Name ?? "")
                });
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserRoles(UserRolesEditViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound($"Could not find user to change roles: {model.Id}");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            List<string> rolesToRemove = new();
            List<string> rolesToAdd = new();

            foreach (var role in model.Roles)
            {
                if (!role.HasRole && currentRoles.Contains(role.Name))
                {
                    rolesToRemove.Add(role.Name);
                }
                else if (role.HasRole && !currentRoles.Contains(role.Name))
                {
                    rolesToAdd.Add(role.Name);
                }
            }


            if (rolesToRemove.Count > 0)
            {
                var result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!result.Succeeded)
                {
                    return BadRequest("Error while removing user from roles");
                }
            }

            if (rolesToAdd.Count > 0)
            {
                var result = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!result.Succeeded)
                {
                    return BadRequest("Error while adding user to roles");
                }
            }

            return RedirectToAction("ManageUsers", "Account");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmUserDeleteByAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Could not find user to delete: {id}");
            }

            UserManagementInfo userInfo = new UserManagementInfo
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                EmailVerified = user.EmailConfirmed
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                userInfo.Roles.Add(role);
            }

            return View(userInfo);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserByAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Could not find user to delete: {id}");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("ManageUsers", "Account");
            }
            else
            {
                return BadRequest($"Could not delete user by id: {id}");
            }
        }
    }
}