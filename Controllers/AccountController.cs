
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;              // Async/await support
using TraNgheCore.Models;
using TraNgheCore.ViewModels;


namespace TraNgheCore.Controllers
{
    /// <summary>
    /// Handles user authentication operations for the TraNghe restaurant system.
    /// Manages login, registration, logout, and role-based redirections.
    /// </summary>
    /// <remarks>
    /// To force users to always sign in before accessing any part of the application,
    /// apply the [Authorize] attribute globally in Program.cs or Startup.cs, or use [Authorize] on all controllers.
    /// Exclude only the login and registration actions with [AllowAnonymous].
    /// </remarks>
    /// 
    [Authorize] // Requires authentication for all actions by default
    public class AccountController : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>

        private readonly UserManager<IdentityModel> _userManager;
        private readonly SignInManager<IdentityModel> _signInManager;

        public AccountController(UserManager<IdentityModel> userManager, SignInManager<IdentityModel> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region User Manager Setup


        #endregion

        #region Login Operations
        /// <summary>
        /// GET: /Account/Login
        /// Displays the login page to users
        /// </summary>
        [AllowAnonymous]
        public IActionResult Login() => View();

        /// <summary>
        /// Handles POST requests for user login.
        /// Validates the login form, attempts to sign in the user, and redirects based on role.
        /// </summary>

        [HttpPost]                          // Only responds to POST requests
        [AllowAnonymous]                    //Allow anonymous users to access this action
        [ValidateAntiForgeryToken]           // Prevents CSRF attacks
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (await _userManager.IsInRoleAsync(user, "Admin"))

                    // Redirect admin users to the admin dashboard
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                else
                    // Redirect regular users to the user home page
                    return RedirectToAction("Index", "Home", new { area = "User" });
            }

            // If login failed, add a generic error message to the form
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        #endregion


        #region Registration Operations
        /// <summary>
        /// GET: /Account/Register
        /// Displays the user registration page
        /// </summary>
        ///[AllowAnonymous]
        //public ActionResult Register() => View();

        /// <summary>
        /// POST: /Account/Register
        /// Creates new user accounts with email/password
        /// Automatically assigns "User" role to new registrations
        /// </summary>
        //[HttpPost]
        //[ValidateAntiForgeryToken]  // CSRF protection
        //public async Task<ActionResult> Register(string email, string password)
        //{
        //    // Create new user instance with email as both username and email
        //    var user = new IdentityModel { UserName = email, Email = email };

        //    // Attempt to create the user account
        //    // Password is automatically hashed by Identity framework
        //    var result = await UserManager.CreateAsync(user, password);

        //    if (result.Succeeded)       // Registration successful
        //    {
        //        // Assign default "User" role to new accounts
        //        // Admin accounts would need to be created manually or through admin panel
        //        await UserManager.AddToRoleAsync(user.Id, "User");

        //        // Redirect to login page so user can sign in
        //        return RedirectToAction("Login");
        //    }

        //    // Registration failed - display all error messages
        //    // Joins multiple errors with semicolon separator
        //    ModelState.AddModelError("", string.Join("; ", result.Errors));

        //    return View(); // Return to registration page with errors
        //}

        #endregion

        #region Logout Operations

        /// <summary>
        /// Logs out the current user and redirects to login page
        /// Clears authentication cookies and session data
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF protection
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        #endregion

    }
}