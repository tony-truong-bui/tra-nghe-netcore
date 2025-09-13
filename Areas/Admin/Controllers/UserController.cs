
using Microsoft.AspNetCore.Mvc;

using TraNgheCore.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TraNgheCore.Areas.Admin.Controllers
{
    /// <summary>
    /// User Management Controller for Restaurant Admin Panel
    /// Located in Admin area - only restaurant administrators can access
    /// Handles creating new user accounts (staff, customers, managers)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [Area("Admin")] // Specifies this controller belongs to the Admin area
    public class UserController : Controller
    {
        #region Main Navigation

        /// <summary>
        /// GET: /Admin/User/Index
        /// Main user management dashboard - shows list of all users
        /// Entry point for restaurant user administration
        /// </summary>
        public IActionResult Index()
        {
            // ⚠️ INCOMPLETE IMPLEMENTATION
            // Currently returns empty view - needs user listing functionality
            // In complete system would show:
            // - All users table with pagination
            // - Search/filter capabilities  
            // - User status indicators (Active, Suspended, etc.)
            // - Quick action buttons (Edit, Delete, Reset Password)
            return View();
        }
        #endregion

        #region Identity Manager Setup

        /// <summary>
        /// Private field for caching the Identity UserManager instance
        /// </summary>
        private readonly UserManager<IdentityModel> UserManager;

        private readonly RoleManager<IdentityRole> RoleManager;

        public UserController(   UserManager<IdentityModel> _userManager,   RoleManager<IdentityRole> _roleManager)
        {
            UserManager = _userManager;
            RoleManager = _roleManager;
        }



        #endregion

        #region User Creation (Complete Implementation)

        /// <summary>
        /// GET: /Admin/User/Create
        /// Displays user creation form for restaurant administrators
        /// Allows creating both staff accounts and customer accounts
        /// </summary>

        public async Task<IActionResult> Create()
        {
            // Prepare role dropdown for admin selection
            // Restaurant context: Admin = staff/managers, User = customers
            //ViewBag.Roles = new SelectList(new[] { "User", "Admin" });

            if (RoleManager != null)
            {
                ViewBag.Roles = new SelectList(RoleManager.Roles.Select(r => r.Name).ToList());
            }
            else
            {
                ViewBag.Roles = new SelectList(new[] { "User", "Admin" }); // Fallback  
            }

            // Get all users from the database as a list.
            var userList = UserManager.Users.ToList();

            // For each user, asynchronously fetch their roles and build a UserViewModel.
            //// Task.WhenAll runs all role-fetching tasks in parallel for better performance.
            //var userViewModels = await Task.WhenAll(userList.Select(async u => new UserViewModel
            //{
            //    Id = u.Id,
            //    UserName = u.UserName,
            //    Email = u.Email,
            //    Roles = (await UserManager.GetRolesAsync(u)).ToList() // Fetch roles for this user
            //}));
            var userViewModels = new List<UserViewModel>();
            foreach (var u in userList)
            {
                var roles = await UserManager.GetRolesAsync(u);
                userViewModels.Add(new UserViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Roles = roles.ToList()
                });
            }



            var model = new UserListViewModel
            {
                CreateUser = new CreateUserViewModel(),     // Empty form for new user creation
                Users = userViewModels.ToList(),            // List of all users with their roles

            };

            return View(model);
        }

        /// <summary>
        /// POST: /Admin/User/Create
        /// Processes new user account creation from admin interface
        /// Implements complete user setup with role assignment
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserListViewModel model)
        {
            // === INPUT VALIDATION PHASE ===
            if (ModelState.IsValid)
            {
                // === USER CREATION PROCESS ===

                // Step 1: Create user object with email-based authentication
                var user = new IdentityModel { UserName = model.CreateUser.Email, Email = model.CreateUser.Email };
                // Additional restaurant fields could be set here:
                // StaffId = GenerateStaffId(),
                // HireDate = DateTime.Now,
                // RestaurantLocation = "Main Branch"

                // Step 2: Create user account with password hashing
                var result = await UserManager.CreateAsync(user, model.CreateUser.Password);
                if (result.Succeeded)
                {
                    // === SUCCESS WORKFLOW ===
                    // Step 3: Assign selected role to new user

                    foreach (var role in model.CreateUser.Role)
                    {
                        await UserManager.AddToRoleAsync(user, role);
                    }

                    // Step 4: Success notification for admin
                    TempData["Success"] = "User created successfully.";

                    // Step 5: Redirect to user creation page for next entry
                    // This allows admin to create multiple users in one session
                    return RedirectToAction("Create");
                }

                // === FAILURE HANDLING ===

                // Display all Identity framework errors
                // Common issues: weak password, duplicate email, invalid characters
                ModelState.AddModelError("", string.Join("; ", result.Errors));
            }

            // === ERROR RECOVERY ===

            // Repopulate dropdown list maintaining user's previous selection
            ViewBag.Roles = new SelectList(RoleManager.Roles.Select(r => r.Name).ToList());

            // Return to form with validation errors displayed
            return View(model);
        }
        #endregion

        [HttpPost]
        //⚡ Async for better performance
        public async Task<JsonResult> AjaxDelete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
                return Json(new { success = false, message = "User not found." });

            // 🗑️ STEP 2: Attempt to delete user from database
            var result = await UserManager.DeleteAsync(user);
            if (result.Succeeded)
                return Json(new { success = true, message = "User deleted successfully." });

            // ❌ FAILURE: Return error details from Identity system
            return Json(new { success = false, message = string.Join("; ", result.Errors) });
        }


        [HttpPost]
        //⚡ Async for better performance
        public async Task<JsonResult> AjaxUpdate(string id, string email, string userName, List<string> role)
        {
            try
            {
                // ⚠️ INPUT VALIDATION: Ensure all fields are provided
                if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(userName) || role.Count != 0)
                {
                    return Json(new { success = false, message = "All fields are required." });
                }
                // 🔍 STEP 1: Find user by ID in database
                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                    return Json(new { success = false, message = "User not found." });


                user.Email = email;
                user.UserName = userName;

                // 💾 STEP 3: Save user changes to database
                var updateResult = await UserManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    return Json(new { success = false, message = string.Join("; ", updateResult.Errors) });

                // 🔄 STEP 4: Update user roles (complex process)

                // 4a. Get user's current roles
                var currentRoles = await UserManager.GetRolesAsync(user);

                // 4b. Remove ALL current roles first
                var removeResult = await UserManager.RemoveFromRolesAsync(user, currentRoles.ToArray());
                if (!removeResult.Succeeded)
                    return Json(new { success = false, message = string.Join("; ", removeResult.Errors) });

                // 4c. Add the new single role

                //Role validation
                foreach (var r in role)
                {
                    var rExists = await RoleManager.RoleExistsAsync(r);
                    if (!rExists)
                    {
                        return Json(new { success = false, message = $"Role {r} doesn't exist." });
                    }
                }

                //Update new roles into user
                foreach (var r in role)
                {
                    var addResult = await UserManager.AddToRoleAsync(user, r);
                    if (!addResult.Succeeded)
                        return Json(new { success = false, message = string.Join("; ", addResult.Errors) });


                }
                return Json(new { success = true, message = "User updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex });
            }
        }

    }
}