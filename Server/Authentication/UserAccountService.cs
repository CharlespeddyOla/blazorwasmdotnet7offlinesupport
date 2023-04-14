using WebAppAcademics.Server.Interfaces;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Administration.Staff;
using WebAppAcademics.Shared.Models.Administration.Students;

namespace WebAppAcademics.Server.Authentication
{
    public class UserAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        SwitchModel _switch = new();
        ADMEmployee _staff = new();
        UserAccount _userAccount = new();
        ADMStudents _student = new();
        ResultCheckerAccount _resultCheckerAccount = new();
        CBTAccount _cbtAccount = new();

        public UserAccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserAccount> GetUserAccountDetailsl(string email, string password)
        {
            _switch.SwitchID = 1;
            _switch.StatusTypeID = 1;
            string userPassword = Utilities.Encrypt(password);
            var activeStaffList = await _unitOfWork.ADMEmployee.GetAllAsync(_switch);
            _staff = activeStaffList.FirstOrDefault(s => s.Email == email && s.Password == userPassword);

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

        public async Task<UserAccount> GetUserRegistrationDetails(string staffpin, string email, string password)
        {

            _switch.SwitchID = 1;
            _switch.StatusTypeID = 1;
            var activeStaffList = await _unitOfWork.ADMEmployee.GetAllAsync(_switch);
            _staff = activeStaffList.FirstOrDefault(s => s.StaffPIN == staffpin);

            if (_staff == null)
            {
                _userAccount.Response = "Invalid Staff PIN";
                return _userAccount;
            }

            if (_staff.RoleID == 0 || _staff.RoleID == 13)
            {
                _userAccount.Response = "Either Your Role Has Not Been Set Or Your Access Has Been Revoked. Please, contact the Administrator";

                return _userAccount;
            }

            bool UserEmailExist = activeStaffList.Where(u => u.Email == email).Any();
            if (UserEmailExist)
            {
                _userAccount.Response = "Email Already Exist";

                return _userAccount;
            }                       

            string userPassword = Utilities.Encrypt(password);
            _staff.Password = userPassword;
            _staff.Email = email;
            await _unitOfWork.ADMEmployee.UpdateAsync(3, _staff);

            _userAccount.StaffID = _staff.StaffID;
            _userAccount.StaffPIN = _staff.StaffPIN;
            _userAccount.Email = _staff.Email;
            _userAccount.StaffNameWithNo = _staff.StaffNameWithNo;
            _userAccount.Surname = _staff.Surname;
            _userAccount.RoleID = _staff.RoleID;
            _userAccount.Role = _staff.RoleDesc;

            return _userAccount;
        }


        public async Task<UserAccount> PasswordResetDetails(string email, string password)
        {
            _switch.SwitchID = 1;
            _switch.StatusTypeID = 1;
            var activeStaffList = await _unitOfWork.ADMEmployee.GetAllAsync(_switch);
            _staff = activeStaffList.FirstOrDefault(s => s.Email == email);

            if (_staff == null)
            {
                _userAccount.Response = "Email Does Not Exist";
                return _userAccount;
            }

            if (_staff.RoleID == 0 || _staff.RoleID == 13)
            {
                _userAccount.Response = "Your Access Has Been Revoked. Please, contact the Administrator";

                return _userAccount;
            }

            if (!_staff.ResetPassword)
            {
                _userAccount.Response = "You Are Not Allow To Reset Your Password. Please, contact the Administrator";
                return _userAccount;
            }

            string userPassword = Utilities.Encrypt(password);
            _staff.Password = userPassword;
            await _unitOfWork.ADMEmployee.UpdateAsync(6, _staff);

            _userAccount.StaffID = _staff.StaffID;
            _userAccount.StaffPIN = _staff.StaffPIN;
            _userAccount.Email = _staff.Email;
            _userAccount.StaffNameWithNo = _staff.StaffNameWithNo;
            _userAccount.Surname = _staff.Surname;
            _userAccount.RoleID = _staff.RoleID;
            _userAccount.Role = _staff.RoleDesc;

            return _userAccount;
        }

        public async Task<ResultCheckerAccount> GetResultCheckerAccountDetailsl(string parentpin)
        {
            _switch.SwitchID = 3;
            _switch.StatusTypeID = 1;
            _switch.SearchCriteriaA = parentpin;
            var currentUser = await _unitOfWork.ADMStudents.SearchAsync(_switch);
            _student = currentUser.SingleOrDefault();

            if (currentUser.Count > 0)
            {
                if (!_student.ParentPinLock)
                {
                    _resultCheckerAccount.STDID = _student.STDID;
                    _resultCheckerAccount.ParentPin = _student.ParentPin;
                    _resultCheckerAccount.AdmissionNo = _student.AdmissionNo;
                    _resultCheckerAccount.Surname = _student.Surname;
                    _resultCheckerAccount.FirstName = _student.FirstName;
                    _resultCheckerAccount.MiddleName = _student.MiddleName;
                    _resultCheckerAccount.StudentName = _student.StudentName;
                    _resultCheckerAccount.ParentPinCount = _student.ParentPinCount;
                    _resultCheckerAccount.ParentPinLock = _student.ParentPinLock;
                    _resultCheckerAccount.ResultTermID = _student.ResultTermID;
                }
                else
                {
                    _resultCheckerAccount.Response = "Your PIN Has Expired. Please Contact The School. Thank You.";
                }
            }
            else
            {
                _resultCheckerAccount.Response = "Invalid Authentication. Wrong PIN.";
            }

            return _resultCheckerAccount;
        }

        public async Task<CBTAccount> GetCBTAccountDetailsl(string studentpin, string password)
        {
            _switch.SwitchID = 2;
            _switch.StatusTypeID = 1;
            _switch.SearchCriteriaA = studentpin;
            string userPassword = Utilities.Encrypt(password);
            _switch.SearchCriteriaB = userPassword;
            var currentUser = await _unitOfWork.ADMStudents.SearchAsync(_switch);
            _student = currentUser.SingleOrDefault();


            if (currentUser.Count > 0)
            {
                if (!_student.CBTLock)
                {
                    _cbtAccount.STDID = _student.STDID;
                    _cbtAccount.ClassListID = _student.ClassListID;
                    _cbtAccount.StudentPin = _student.StudentPin;
                    _cbtAccount.AdmissionNo = _student.AdmissionNo;
                    _cbtAccount.StudentName = _student.StudentName;
                    _cbtAccount.CBTLock = _student.CBTLock;
                }
                else
                {
                    _cbtAccount.Response = "Your CBT Exam Has Been Locked. Please Contact The School Authority To UnLock Your CBT Exam.";
                }
            }
            else
            {
                _cbtAccount.Response = "Invalid Authentication. Wrong PIN Or Password.";
            }

            return _cbtAccount;
        }
    }
}
