using asp_identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asp_identity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;

        public HomeController(ILogger<HomeController> logger,
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            _logger = logger;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }        
        
        public async Task<IActionResult> ClientIndex()
        {            
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            StringBuilder sb = new StringBuilder($"I am {user.UserName} and my roles are: ");
            var roles = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                sb.Append(role + " ");
            }

            return Json(sb.ToString());
        }
        public async Task<IActionResult> Index()
        {
            List<AppUser> users = new();

            var all = userManager.Users.ToList();

            foreach (var user in all)
            {
                var task = await userManager.GetRolesAsync(user);
                var roles = task.ToList();
                
                if (roles.Contains("ADMIN") == false)
                {
                    users.Add(user);
                }
            }


            return View(users);
        }

        public async Task<IActionResult> Edit(string username)
        {            
            var user = await userManager.FindByNameAsync(username);
            var correspondingRoles = await userManager.GetRolesAsync(user);


            UserViewModel viewModel = new()
            {
                Username = user.UserName,
                Roles = new List<RoleViewModel>()
            };

            var allRoles = roleManager.Roles.Select(r => r.Name).ToHashSet();
            var roleNames = await userManager.GetRolesAsync(user);

            for (int i = 0; i < roleNames.Count; i++)
            {
                viewModel.Roles.Add(new RoleViewModel
                {
                    Name = roleNames[i],
                    IsEnabled = true
                });

                allRoles.Remove(roleNames[i]);
            }

            foreach (var role in allRoles)
            {
                viewModel.Roles.Add(new RoleViewModel
                {
                    Name = role
                });
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (model != null)
            {
                var user = await userManager.FindByNameAsync(model.Username);
                var correspondingRoles = await userManager.GetRolesAsync(user);

                var roles = model.Roles.Where(r => r.IsEnabled).ToList();

                if (roles.Count == 0)
                {
                    await userManager.RemoveFromRolesAsync(user, correspondingRoles);
                }
                else
                {

                    for (int i = 0; i < correspondingRoles.Count; i++)
                    {
                        if (roles.Any(r => r.Name == correspondingRoles[i]) == false)
                        {
                            await userManager.RemoveFromRoleAsync(user, correspondingRoles[i]);
                        }
                    }

                    var data = roles.Select(r => r.Name).ToList();

                    for (int i = 0; i < data.Count; i++)
                    {
                        await userManager.AddToRoleAsync(user, data[i]);
                    }

                    //await userManager.AddToRolesAsync(user, data);
                }


            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
