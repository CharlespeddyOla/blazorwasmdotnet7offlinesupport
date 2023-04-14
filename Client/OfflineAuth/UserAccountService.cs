using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Administration.Staff;

namespace WebAppAcademics.Client.OfflineAuth
{
    public class UserAccountService
    {
        ADMEmployee _staff = new();
        UserAccount _userAccount = new();

        public async Task<UserAccount> GetUserAccountDetailsl(string email, string password, List<ADMEmployee> sList)
        {
           string userPassword = Utilities.Encrypt(password);
            _staff = sList.FirstOrDefault(s => s.Email == email && s.Password == userPassword);

            if (_staff == null) return null;

            if (_staff.RoleID == 0 || _staff.RoleID == 13) return null;

            _userAccount.StaffID = _staff.StaffID;
            _userAccount.StaffPIN = _staff.StaffPIN;
            _userAccount.Email = _staff.Email;
            _userAccount.StaffNameWithNo = _staff.StaffNameWithNo;
            _userAccount.Surname = _staff.Surname;
            _userAccount.RoleID = _staff.RoleID;
            _userAccount.Role = _staff.RoleDesc;

            await Task.CompletedTask;

            return _userAccount;
        }
    }
}
