using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CitiesManager.WebAPI.Controllers.v1
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    [ApiVersion("1.0")]
    public class AccountController : CustomControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="jwtService"></param>
        public AccountController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            RoleManager<ApplicationRole> roleManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registerDTO"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        public async Task<ActionResult<ApplicationUser>> Register(RegisterDTO registerDTO)
        {
            // Validation
            if (ModelState.IsValid == false)
            {
                string errorMessage = String.Join(" | ", ModelState.Values
                    .SelectMany(temp => temp.Errors)
                    .SelectMany(error => error.ErrorMessage));

                return Problem(errorMessage);
            }

            ApplicationUser applicationUser = new ApplicationUser()
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                PersonName = registerDTO.PersonName,
                PhoneNumber = registerDTO.PhoneNumber,
            };

            IdentityResult result = 
                await _userManager.CreateAsync(applicationUser, 
                registerDTO.Password!);

            if (result.Succeeded)
            {
                // sign-in the User
                await _signInManager.SignInAsync(applicationUser, isPersistent: false);

                AuthenticationResponse response = 
                    _jwtService.CreateJwtToken(applicationUser);

                applicationUser.RefreshToken = response.RefreshToken;
                applicationUser.RefreshTokenExpirationDateTime =
                    response.RefreshTokenExpirationDate;

                await _userManager.UpdateAsync(applicationUser);



                return Ok(response);
            }
            else
            {
                // error 1 | error 2
                string errorMessage = string.Join(" | ",
                    result.Errors.Select(e => e.Description));

                return Problem(errorMessage);
            }

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (ModelState.IsValid == false)
            {
                string errorMessage = String.Join(" | ", ModelState.Values
                                    .SelectMany(temp => temp.Errors)
                                    .SelectMany(error => error.ErrorMessage));

                return Problem(errorMessage);
            }

            var result =  await _signInManager.PasswordSignInAsync(
                loginDTO.Email, 
                loginDTO.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                ApplicationUser? user = await _userManager.FindByEmailAsync(loginDTO.Email);

                // This will never execute
                if (user == null)
                {
                    return NoContent();
                }

                //sign-in
                await _signInManager.SignInAsync(user, isPersistent: false);

                AuthenticationResponse response =
                    _jwtService.CreateJwtToken(user);

                user.RefreshToken = response.RefreshToken;
                user.RefreshTokenExpirationDateTime =
                    response.RefreshTokenExpirationDate;

                await _userManager.UpdateAsync(user);

                return Ok(response);
            }
            else
            {
                return Problem("Invalid email or password");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return NoContent();
        }

        [HttpPost("generate-new-jwt-token")]
        public async Task<IActionResult> GenerateNewAccessToken(TokenModel tokenModel)
        {
            if(tokenModel == null)
            {
                return BadRequest("Invalid client request");
            }


            ClaimsPrincipal? principal = _jwtService.GetPrincipalFromJwtToken(tokenModel.Token);

            if(principal == null)
            {
                return BadRequest("Invalid jwt access token");
            }

            string? email = principal.FindFirstValue(ClaimTypes.Email);

            ApplicationUser? user = await _userManager.FindByEmailAsync(email);

            if(user == null || user.RefreshToken != tokenModel.RefreshToken || 
                user.RefreshTokenExpirationDateTime <= DateTime.Now)
            {
                return BadRequest("Invalid refresh token");
            }

            AuthenticationResponse response = _jwtService.CreateJwtToken(user);

            user.RefreshToken = response.RefreshToken;
            user.RefreshTokenExpirationDateTime = response.RefreshTokenExpirationDate;

            await _userManager.UpdateAsync(user);

            return Ok(response);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet("IsEmailRegistered")]
        public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);

            if(user == null)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }
    }
}
