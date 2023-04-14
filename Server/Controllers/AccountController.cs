using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAppAcademics.Server.Authentication;
using WebAppAcademics.Shared.Models.LoginModels;

namespace WebAppAcademics.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private UserAccountService _userAccountService;

        public AccountController(UserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserSession>> Login([FromBody] LoginRequest loginRequest)
        {
            var jwtAuthenticationManager = new JwtAuthenticationManager(_userAccountService);
            var userSession = await jwtAuthenticationManager.GenerateJwtToken(loginRequest.Email, loginRequest.Password);
            if (userSession is null)
                return Unauthorized();
            else
                return userSession;
        }

        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserSession>> Register([FromBody] RegistrationRequest registrationRequest)
        {
            var jwtAuthenticationManager = new JwtAuthenticationManager(_userAccountService);
            var userSession = await jwtAuthenticationManager.RegisterUser(registrationRequest.StaffPIN, registrationRequest.Email, registrationRequest.Password);
           
            return userSession;
        }

        [HttpPost]
        [Route("ResetPassword")]
        [AllowAnonymous]
        public async Task<ActionResult<UserSession>> ResetPassword([FromBody] PasswordResetRequest passwordResetRequest)
        {
            var jwtAuthenticationManager = new JwtAuthenticationManager(_userAccountService);
            var userSession = await jwtAuthenticationManager.ResetPassword(passwordResetRequest.Email, passwordResetRequest.Password);

            return userSession;
        }


        [HttpPost]
        [Route("ResultCheckerLogin")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultCheckerSession>> ResultCheckerLogin([FromBody] ResultCheckerLoginRequest loginRequest)
        {
            var jwtAuthenticationManager = new JwtAuthenticationManager(_userAccountService);
            var userSession = await jwtAuthenticationManager.ResultChecker(loginRequest.ParentPin);

            return userSession;
        }

        [HttpPost]
        [Route("CBTLogin")]
        [AllowAnonymous]
        public async Task<ActionResult<CBTSession>> CBTLogin([FromBody] CBTLoginRequest loginRequest)
        {
            var jwtAuthenticationManager = new JwtAuthenticationManager(_userAccountService);
            var userSession = await jwtAuthenticationManager.GenerateCBTJwtToken(loginRequest.StudentPin, loginRequest.Password);

            return userSession;
        }


    }
}
