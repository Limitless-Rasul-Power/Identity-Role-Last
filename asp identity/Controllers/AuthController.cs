using asp_identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp_identity.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly SignInManager<AppUser> signInManager;

        public AuthController(UserManager<AppUser> userManager,
                            RoleManager<AppRole> roleManager,
                            SignInManager<AppUser> signInManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!await roleManager.RoleExistsAsync("ADMIN"))
            {
                AppRole ar1 = new() { Name = "USER" };
                AppRole ar2 = new() { Name = "ADMIN" };
                AppRole ar3 = new() { Name = "COMPANY" };

                await roleManager.CreateAsync(ar1);
                await roleManager.CreateAsync(ar2);
                await roleManager.CreateAsync(ar3);

                var u1 = new AppUser { UserName = "demo1" };
                var u2 = new AppUser { UserName = "demo2" };
                var u3 = new AppUser { UserName = "demo3" };

                var list = new List<AppUser> { u1, u2, u3 };

                for (int i = 0; i < list.Count; i++)
                {
                    await userManager.CreateAsync(list[i], "dEmlo1!");
                    await userManager.AddToRoleAsync(list[i], "USER");
                }

                var admin = new AppUser { UserName = "admin" };
                await userManager.CreateAsync(admin, "dEmlo1!");
                await userManager.AddToRoleAsync(admin, "ADMIN");

                var company = new AppUser { UserName = "company" };
                await userManager.CreateAsync(company, "dEmlo1!");
                await userManager.AddToRoleAsync(company, "COMPANY");

            }


            var user = await userManager.FindByNameAsync(model.Username);

            if (user != null)
            {                
                var result = await userManager.CheckPasswordAsync(user, model.Password);

                if (result)
                {
                    var response = await signInManager.PasswordSignInAsync(user, model.Password, true, true);
                    var roles = await userManager.GetRolesAsync(user);

                    if (response.Succeeded)
                    {
                        return roles.Contains("ADMIN") ? RedirectToAction("Index", "Home") :
                            RedirectToAction("ClientIndex", "Home");
                    }                    

                }

            }

            return View(model);
        }

        public IActionResult SignUp()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> SignUp(RegisterViewModel model)
        {
            if (model != null)
            {
                AppUser user = new()
                {
                    UserName = model.Username,
                    Email = model.Email
                };

                var result = await userManager.CreateAsync(user, model.Password);//1-ci user add olur

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "USER");//2-ci var olan user-i goturub role-la elaqelendiririk
                    return View(nameof(Login));
                }


            }

            return View();
        }

    }
}