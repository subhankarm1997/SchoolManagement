using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Data;
using SchoolManagement.Models;
using SchoolManagement.ViewModels;

namespace SchoolManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context
           )
        {
            _context = context;
            
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);

            var user = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);

            if (user != null)
            {
                //User is found, check password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (passwordCheck)
                {
                    //Password correct, sign in
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        if (User.IsInRole("Teacher"))
                        {
                            // Redirect to the Teacher Dashboard
                            return RedirectToAction("TeacherDashboard");
                        }
                      
                        else
                        {
                            // Redirect to the Student Dashboard
                            return RedirectToAction("StudentDashboard");
                        }
                      
                        
                    }
                }
                //Password is incorrect
                TempData["Error"] = "Wrong credentials. Please try again";
                return View(loginViewModel);
            }
            //User not found
            TempData["Error"] = "Wrong credentials. Please try again";
            return View(loginViewModel);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var response = new RegisterViewModel();
            return View(response);
        }

     
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if the email address is already in use
            var userExists = await _userManager.FindByEmailAsync(model.EmailAddress);
            if (userExists != null)
            {
                TempData["Error"] = "This email address is already in use";
                return View(model);
            }

            var user = new AppUser
            {
                UserName = model.EmailAddress,
                Email = model.EmailAddress,
                FullName = model.FullName,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                UserType = model.UserType
                // Add other properties based on the user type
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Add user to a default role (modify as needed)

                // Check UserType and add to specific role
                if (user.UserType == "Teacher")
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Teacher);
                    var teacher = new Teacher
                    {
                        
                        Email = user.Email,
                        FullName = user.FullName,
                        DateOfBirth = user.DateOfBirth,
                        Gender = user.Gender,
                        UserType = user.UserType
                    };
                    _context.Teachers.Add(teacher);
                    await _context.SaveChangesAsync();
                }
                else if (user.UserType == "Student")
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Student);
                    var student = new Student
                    {
                        Email = user.Email,
                        FullName = user.FullName,
                        DateOfBirth = user.DateOfBirth,
                        Gender = user.Gender,
                        UserType = user.UserType
                    };
                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();
                }




                await _signInManager.SignInAsync(user, isPersistent: false);

                // Check roles after sign-in
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains(UserRoles.Teacher))
                {
                    // Redirect to the Teacher Dashboard
                    return RedirectToAction("TeacherDashboard");
                }
                else if (roles.Contains(UserRoles.Student))
                {
                    // Redirect to the Student Dashboard
                    return RedirectToAction("StudentDashboard");
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // If registration fails, return to the registration page with validation errors
            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("Account/Welcome")]
        public async Task<IActionResult> Welcome(int page = 0)
        {
            if (page == 0)
            {
                return View();
            }
            return View();

        }
        public ActionResult TeacherDashboard()
        {
            // Logic for Teacher Dashboard
            return View();
        }

        public ActionResult StudentDashboard()
        {
            // Logic for Student Dashboard
            return View();
        }


    }
}
