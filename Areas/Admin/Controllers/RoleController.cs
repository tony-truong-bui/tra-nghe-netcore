
using Microsoft.AspNetCore.Mvc;
using TraNgheCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace TraNgheCore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class RoleController : Controller
    {
        // GET: Admin/Role
        public IActionResult Index()
        {
            return View();
        }

        private RoleManager<IdentityRole> RoleManager;

        public RoleController (RoleManager<IdentityRole> _roleManager)
        {
            RoleManager = _roleManager;
        }

        //Role creation
        
        public IActionResult Create()
        {

            var roles = RoleManager.Roles.Select(r => new { r.Name }).ToList();
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoleViewModel role)
        {
            if (ModelState.IsValid)
            {
                //Role validation
                var roleExists = await RoleManager.RoleExistsAsync(role.RoleName);
                if (!roleExists)
                {
                    return Json(new { success = false, message = $"Role {role} doesn't exist." });
                }
                var newRole = new IdentityRole(role.RoleName);

                var result = await RoleManager.CreateAsync(newRole);


                if (result.Succeeded)
                {

                    TempData["Success"] = "User created successfully.";
                    //•	It checks if the submitted model is valid 
                    return RedirectToAction("Create"); // Redirect to show updated list
                }
                else
                {
                    ModelState.AddModelError("", string.Join(", ", result.Errors));
                }
            }
            ViewBag.Roles = RoleManager.Roles.Select(r => new { r.Name }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AjaxDelete(string roleName)
        {
            var role = await RoleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var result = await RoleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "Role deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = string.Join(", ", result.Errors) });
                }
            }
            return Json(new { success = false, message = "Role not found." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AjaxUpdate(string id, string roleName)
        {

            try
            {

                if (string.IsNullOrWhiteSpace(roleName))
                {
                    return Json(new { success = false, message = "Role name cannot be empty." });
                }

                var role = await RoleManager.FindByIdAsync(id);

                if (role != null)
                {
                    role.Name = roleName;
                    var result = await RoleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return Json(new { success = true, message = "Role updated successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = string.Join(", ", result.Errors) });
                    }
                } else
                {
                    return Json(new { success = false, message = "Role not found." });
                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while updating the role: " + ex.Message });
            }
        }

    }
}