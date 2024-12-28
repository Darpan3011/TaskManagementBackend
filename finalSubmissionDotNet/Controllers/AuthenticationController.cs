using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using finalSubmission.Core.Domain.Entities;
using ContactsManager.Core.Domain.IdentityEntities;
using Microsoft.AspNetCore.Authorization;
using finalSubmission.Core.ServiceContracts.IUserService;
using finalSubmissionDotNet.Filters;
using Microsoft.EntityFrameworkCore;
using finalSubmissionDotNet.Models;
using finalSubmissionDotNet.Helper;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Reflection;

namespace finalSubmissionDotNet.Controllers
{
    /// <summary>
    /// For Login and Register
    /// </summary>
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ICreateUser _createUser;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ICreateUser createUser, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _createUser = createUser;
            _configuration = configuration;
        }

        /// <summary>
        /// Roles: 0 Admin, 1 User
        /// </summary>
        public enum UserType
        {
            Admin, User
        }

        /// <summary>
        /// Register a new user 0 for Admin, 1 for User (0 and 1 is for testing purpose only)
        /// </summary>
        [HttpPost("[action]/{usertype}")]
        public async Task<IActionResult> Register([FromBody] RegisterUser model, [FromRoute] UserType usertype)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid userid = Guid.NewGuid();

            ApplicationUser user = new ApplicationUser
            {
                UserName = model.UserName,
                Id = userid
            };

            User user1 = new User() { UserName = model.UserName, UserId = userid };

            await _createUser.CreateAnUser(user1);

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Assign the role as per selected option (just for testing purpose)
            if (await _roleManager.RoleExistsAsync(usertype.ToString()))
            {
                await _userManager.AddToRoleAsync(user, usertype.ToString());
            }

            return Ok(new { message = $"{(usertype == UserType.Admin ? "Admin" : "User")} registered successfully!" });
        }

        /// <summary>
        /// Login a user and generate a JWT token (paste the token in swagger in Authorize input as "Bearer jwt_generated_token")
        /// </summary>
        [HttpPost("[action]")]
        [TypeFilter(typeof(ModelValidationActionFilter))]
        public async Task<IActionResult> Login([FromBody] LoginUser model)
        {

            ApplicationUser? user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken? token =  new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            string refreshToken = GenerateRefresh.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                // generate token
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                refreshToken = refreshToken
            });
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(ModelValidationActionFilter))]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
        {
            ApplicationUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == model.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken? token =  new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        
    }
}
