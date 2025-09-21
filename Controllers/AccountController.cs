
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;              // Async/await support
using TraNgheCore.Models;
using TraNgheCore.ViewModels;
using Microsoft.Extensions.Options;


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

        private readonly ReCaptchaSettings _captcha;

        public AccountController(UserManager<IdentityModel> userManager, SignInManager<IdentityModel> signInManager, IOptions<ReCaptchaSettings> captchaOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _captcha = captchaOptions.Value;
        }

        #region User Manager Setup


        #endregion

        #region Login Operations
        /// <summary>
        /// GET: /Account/Login
        /// Displays the login page to users
        /// </summary>
        [AllowAnonymous]
        public IActionResult Login()
        {
            // ViewBag.ReCaptchaSiteKey = "6Lft420rAAAAAAfty_DxE4PoYzNgUZrz8ApIKIuy";
            Console.WriteLine("Login GET SiteKey=" + _captcha.SiteKey);
            ViewBag.ReCaptchaSiteKey = _captcha.SiteKey;
            return View();
        }

        /// <summary>
        /// Handles POST requests for user login.
        /// Validates the login form, attempts to sign in the user, and redirects based on role.
        /// </summary>

        [HttpPost]                          // Only responds to POST requests
        [AllowAnonymous]                    //Allow anonymous users to access this action
        [ValidateAntiForgeryToken]           // Prevents CSRF attacks
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            //Captcha verification
            ViewBag.ReCaptchaSiteKey = _captcha.SiteKey;
            // Get the reCAPTCHA response token from the submitted form
            var captchaResponse = Request.Form["g-recaptcha-response"];

            if (string.IsNullOrWhiteSpace(captchaResponse))
            {
                ModelState.AddModelError(string.Empty, "Captcha required.");
                return View(model);
            }
            // Create an HTTP client to send a request to Google's reCAPTCHA verification API
            var http = new HttpClient();

            // Send a POST request to Google's API with your secret key and the user's response token
            var verify = await http.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={_captcha.SecretKey}&response={captchaResponse}", null);

            // Read the JSON response from Google
            var json = await verify.Content.ReadAsStringAsync();

            // Deserialize the JSON response to a dynamic object for easy access
            dynamic captchaResult = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            // If the CAPTCHA was not solved successfully
            if (captchaResult.success != true)
            {
                ModelState.AddModelError(string.Empty, "Captcha failed.");
                return View(model);
            }

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
                    return RedirectToAction("Index", "Dashboard", new { area = "User" });
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
