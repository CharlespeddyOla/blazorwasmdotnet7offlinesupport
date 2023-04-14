using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAppAcademics.Shared.Models.Administration.Staff;
using WebAppAcademics.Shared.Models.LoginModels;

namespace WebAppAcademics.Server.Authentication
{
    public class JwtAuthenticationManager
    {
        public const string JWT_SECURITY_KEY = "thisisasecretkeyanddontsharewithanyone";
        public const int JWT_TOKEN_VALIDITY_MINS = 20;

        private UserAccountService _userAccountService;

        public JwtAuthenticationManager(UserAccountService userAccountService)
        {
            _userAccountService = userAccountService; 
        }

        public async Task<UserSession> GenerateJwtToken(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            //Validating the User Credentials
            var userAccount = await _userAccountService.GetUserAccountDetailsl(email, password);
            if (userAccount == null) return null;

            /*Generate JWT Token */
            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, userAccount.StaffNameWithNo),
                new Claim(ClaimTypes.Role, userAccount.Role),
                new Claim(ClaimTypes.Surname, userAccount.Surname),
                new Claim(ClaimTypes.Email, userAccount.Email),
                new Claim(ClaimTypes.PrimaryGroupSid, Convert.ToString(userAccount.RoleID)),
                new Claim(ClaimTypes.PrimarySid, Convert.ToString(userAccount.StaffID))
            });
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature);
            var securityTokenDisriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = signingCredentials
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDisriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            //Returning the User Session object
            var userSession = new UserSession
            {
                StaffID = userAccount.StaffID,
                Email = userAccount.Email,
                StaffNameWithNo = userAccount.StaffNameWithNo,
                Surname = userAccount.Surname,
                RoleID = userAccount.RoleID,
                Role = userAccount.Role,
                Token = token,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds
            };

            return userSession;
        }

        public async Task<UserSession> RegisterUser(string staffpin, string email, string password)
        {
            var userAccount = await _userAccountService.GetUserRegistrationDetails(staffpin, email, password);
            if (userAccount.StaffID == 0)
            {
                var userSessionError = new UserSession
                {
                    Response = userAccount.Response
                };

                return userSessionError;
            }

            var userSession = new UserSession
            {
                StaffID = userAccount.StaffID,
                Email = userAccount.Email,
                StaffNameWithNo = userAccount.StaffNameWithNo,
                Surname = userAccount.Surname,
                RoleID = userAccount.RoleID,
                Role = userAccount.Role
            };

            return userSession;
        }

        public async Task<UserSession> ResetPassword(string email, string password)
        {
            var userAccount = await _userAccountService.PasswordResetDetails(email, password);
            if (userAccount.StaffID == 0)
            {
                var userSessionError = new UserSession
                {
                    Response = userAccount.Response
                };

                return userSessionError;
            }

            var userSession = new UserSession
            {
                StaffID = userAccount.StaffID,
                Email = userAccount.Email,
                StaffNameWithNo = userAccount.StaffNameWithNo,
                Surname = userAccount.Surname,
                RoleID = userAccount.RoleID,
                Role = userAccount.Role
            };

            return userSession;
        }

        public async Task<ResultCheckerSession> ResultChecker(string parentpin)
        {
            var userAccount = await _userAccountService.GetResultCheckerAccountDetailsl(parentpin);
            if (userAccount.STDID == 0)
            {
                var userSessionError = new ResultCheckerSession
                {
                    Response = userAccount.Response,
                };

                return userSessionError;
            }

            /*Generate JWT Token */
            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
               new Claim(ClaimTypes.PrimarySid, Convert.ToString(userAccount.STDID)),
                new Claim(ClaimTypes.Name, userAccount.StudentName),
                new Claim(ClaimTypes.Country, userAccount.AdmissionNo),
                new Claim(ClaimTypes.PrimaryGroupSid, Convert.ToString(userAccount.ResultTermID)),
                new Claim(ClaimTypes.GroupSid, Convert.ToString(userAccount.ParentPinCount)),
            });
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature);
            var securityTokenDisriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = signingCredentials
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDisriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            //Returning the User Session object
            var userSession = new ResultCheckerSession
            {
                STDID = userAccount.STDID,
                ParentPinCount = userAccount.ParentPinCount,
                AdmissionNo = userAccount.AdmissionNo,
                StudentName = userAccount.StudentName,
                ResultTermID = userAccount.ResultTermID,
                Token = token,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds
            };

            return userSession;
        }

        public async Task<CBTSession> GenerateCBTJwtToken(string studentpin, string password)
        {
            //Validating the User Credentials
            var userAccount = await _userAccountService.GetCBTAccountDetailsl(studentpin, password);
            if (userAccount.STDID == 0)
            {
                var userSessionError = new CBTSession
                {
                    Response = userAccount.Response,
                };

                return userSessionError;
            }

            /*Generate JWT Token */
            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, userAccount.StudentName),
                new Claim(ClaimTypes.Role, userAccount.AdmissionNo),
                new Claim(ClaimTypes.PrimaryGroupSid, Convert.ToString(userAccount.ClassListID)),
                new Claim(ClaimTypes.PrimarySid, Convert.ToString(userAccount.STDID))
            });
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature);
            var securityTokenDisriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = signingCredentials
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDisriptor);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);

            //Returning the User Session object
            var userSession = new CBTSession
            {
                STDID = userAccount.STDID,
                ClassListID = userAccount.ClassListID,
                AdmissionNo = userAccount.AdmissionNo,
                StudentName = userAccount.StudentName,
                Token = token,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds
            };

            return userSession;
        }
    }
}
