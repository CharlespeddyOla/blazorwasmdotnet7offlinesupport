using System.Text;
using WebAppAcademics.Shared.Models.Administration.Staff;
using WebAppAcademics.Shared.Models.LoginModels;

namespace WebAppAcademics.Client.OfflineAuth
{
    public class JwtAuthenticationManager
    {
        public const string JWT_SECURITY_KEY = "thisisasecretkeyanddontsharewithanyone";
        public const int JWT_TOKEN_VALIDITY_MINS = 60;

        private UserAccountService _userAccountService;

        public JwtAuthenticationManager(UserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        public async Task<UserSession> GenerateJwtToken(string email, string password, List<ADMEmployee> sList)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            //Validating the User Credentials
            var userAccount = await _userAccountService.GetUserAccountDetailsl(email, password, sList);
            if (userAccount == null) return null;

            /*Generate JWT Token */
            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(JWT_TOKEN_VALIDITY_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(JWT_SECURITY_KEY);

            byte[] time = BitConverter.GetBytes(tokenExpiryTimeStamp.ToBinary());
            string token = Convert.ToBase64String(time.Concat(tokenKey).ToArray());

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
    }
}
