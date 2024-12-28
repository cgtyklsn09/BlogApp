using System.Security.Claims;
using BlogApp.Data.Abstract;
using BlogApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName || x.Email == model.Email);
                if (user == null)
                {
                    user = _userRepository.CreateUser(new Entity.User
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        Name = model.Name,
                        Password = model.Password,
                        Image = "avatar.png"
                    });

                    await LoginUser(user!);
                    return RedirectToAction("Index", "Posts");
                }
                else
                {
                    ModelState.AddModelError("", "It looks like there’s already an account with this email or username. If you forgot your password, you can reset it.");
                }
            }

            return View(model);
        }


        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Posts");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userRepository.Users.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);

                if (user == null)
                    ModelState.AddModelError("", "Username or password is not valid.");
                else
                {
                    await LoginUser(user);
                    return RedirectToAction("Index", "Posts");
                }
            }

            return View(model);
        }

        private async Task LoginUser(Entity.User user)
        {
            //cookies
            var userClaims = new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new(ClaimTypes.Name, user.UserName ?? ""),
                        new(ClaimTypes.GivenName, user.Name ?? ""),
                        new(ClaimTypes.UserData, user.Image ?? "")
                    };

            if (user.Email == "info@cagataykalsan.com")
                userClaims.Add(new(ClaimTypes.Role, "admin"));

            var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();
            authProperties.IsPersistent = true;

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync
            (
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Profile(string userName)
        {
            if(string.IsNullOrEmpty(userName))
                return NotFound();

            var user = await _userRepository
                            .Users
                            .Include(x => x.Posts)
                            .Include(x => x.Comments)
                            .ThenInclude(x => x.Post)
                            .FirstOrDefaultAsync(x => x.UserName == userName);

            if(user == null)
                return NotFound();

            return View(user);
        }
    }
}