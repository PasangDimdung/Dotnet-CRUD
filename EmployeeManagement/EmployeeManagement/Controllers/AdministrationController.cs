using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
                                        UserManager<ApplicationUser> userManager
            )
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if(user == null)
            {
                return NoContent();
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles =  await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Roles = userRoles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NoContent();
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;
            }

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("ListUsers");
            }

            //we are looping through errors because.. for example if you try to edit email but it's already taken, Then email taken message is shown.
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description); 
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var identityRole = new IdentityRole { Name = model.RoleName };

                var result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View();
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRoles(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if(role == null)
            {
                return View();
            }

            var model = new EditRoleViewModel { Id = role.Id, RoleName = role.Name };

            foreach(var user in userManager.Users)
            {
                if(await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRoles(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                return View();
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            //storing role id for the view
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);

            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return NoContent();
            }

            //creating list of objects of UserRoleViewModel for view
            var model = new List<UserRoleViewModel>();

            //Looping all the users
            foreach (var user in userManager.Users)
            {
                //mapping user id and user name
                var userRoleViewModel = new UserRoleViewModel { UserId = user.Id, UserName = user.UserName };

                //check if the user is in role or not
                if (await userManager.IsInRoleAsync(user, role.Name)) 
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model,  string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return NoContent();
            }

            //looping through populated viewmodel 
            for (int i = 0; i < model.Count ; i++)
            {
                //retrieving single user by id. Eg: model[0].101 = user1
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                //add user to role if it is selected and user in not in that selected role.
                if(model[i].IsSelected &&  !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                   result = await userManager.AddToRoleAsync(user, role.Name);
                }
                //remove user to role if it isnot selected and user is in the role. 
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                // if the user is selected and is in role || if the user isnot selected and isnot in role || user clicks update without making form dirty
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    //check if there is any model left to process if left then it goes back to continue loop
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRoles", new { Id = roleId });
                    }

                }
            }

            return RedirectToAction("EditRoles", new { Id = roleId });
             
        }
    }
}
