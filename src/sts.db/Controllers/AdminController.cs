using System.Collections.Generic;
using System.Threading.Tasks;
using identity.Models;
using identity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace identity.Controllers
{
    public class AdminController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IPasswordValidator<ApplicationUser> passwordValidator;
        private IPasswordHasher<ApplicationUser> passwordHasher;

        public AdminController(UserManager<ApplicationUser> _userManager,
                               RoleManager<IdentityRole> roleManager,
            IPasswordValidator<ApplicationUser> passValidator,
            IPasswordHasher<ApplicationUser> passHasher)
        {
            userManager = _userManager;
            _roleManager = roleManager;
            passwordValidator = passValidator;
            passwordHasher = passHasher;
        }

        public IActionResult Index()
        {
            return View(userManager.Users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = model.UserName;
                user.Email = model.Email;
                //user.FirstName = model.FirstName;
                //user.LastName = model.LastName;

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    TempData["message"] = $"{user.UserName} has been created.";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    TempData["message"] = $"{user.UserName} has been deleted.";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "user not found");
            }
            return View("Index", userManager.Users);
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                var model = new UpdateUserModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    //FirstName = user.FirstName,
                    //LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber
                };
                return View(model);
            }
            else
            {
                ModelState.AddModelError("", "user not found");
            }
            return View("Index", userManager.Users);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateUserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    //user.FirstName = model.FirstName;
                    //user.LastName = model.LastName;
                    user.PhoneNumber = model.PhoneNumber;
                    var result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        TempData["message"] = $"{model.UserName} has been updated.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Infos(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                var vm = new ApplicationUserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    //FirstName = user.FirstName,
                    //LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = new List<string>()
                };

                vm.Roles = await userManager.GetRolesAsync(user);
                return View(vm);
            }

            else
                ModelState.AddModelError("", "user not found");

            return View("Index", userManager.Users);
        }


        //[HttpGet]
        //public async Task<IActionResult> Update(string Id)
        //{
        //    var user = await userManager.FindByIdAsync(Id);

        //    if (user != null)
        //    {
        //        return View(user);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> Update(string Id, string Password, string Email)
        //{
        //    var user = await userManager.FindByIdAsync(Id);

        //    if (user != null)
        //    {
        //        user.Email = Email;

        //        IdentityResult validPass = null;

        //        if (!string.IsNullOrEmpty(Password))
        //        {
        //            validPass = await passwordValidator.ValidateAsync(userManager, user, Password);

        //            if (validPass.Succeeded)
        //            {
        //                user.PasswordHash = passwordHasher.HashPassword(user, Password);
        //            }
        //            else
        //            {
        //                foreach (var item in validPass.Errors)
        //                {
        //                    ModelState.AddModelError("", item.Description);
        //                }
        //            }
        //        }

        //        if (validPass.Succeeded)
        //        {
        //            var result = await userManager.UpdateAsync(user);

        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("Index");
        //            }
        //            else
        //            {
        //                foreach (var item in result.Errors)
        //                {
        //                    ModelState.AddModelError("", item.Description);
        //                }
        //            }
        //        }

        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "User Not Found");
        //    }

        //    return View(user);
        //}

    }
}