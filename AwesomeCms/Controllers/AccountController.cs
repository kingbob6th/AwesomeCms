using AwesomeCms.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AwesomeCms.HelperModels;
using AwesomeCms.HelperModels.Requests;
using Microsoft.Extensions.Options;

namespace AwesomeCms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly JWT _jwt;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;

            _jwt = jwt.Value;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "RegularUser");

                var token = await GenerateToken(user);
                return Ok(new
                {
                    message = "User created successfully",
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var token = await GenerateToken(user);
                return Ok(new
                {
                    message = "User successfully logged in.",
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized("User/Pass does not match");
        }

        private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            //var authClaims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name, user.UserName),
            //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            //};

            //foreach (var userRole in userRoles)
            //{
            //    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            //}

            var authClaims = new List<Claim>
            {
                new Claim("name", user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim("role", userRole));
            }

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                expires: DateTime.Now.AddMinutes(_jwt.DurationInMinutes),
                claims: authClaims,
                signingCredentials: signingCredentials
            );

            return token;
        }
    }
}
