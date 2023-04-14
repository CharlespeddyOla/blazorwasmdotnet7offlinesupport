using WebAppAcademics.Server.Interfaces.Staff;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Administration.Staff;
using System.Data;
using System.Data.SqlClient;

namespace WebAppAcademics.Server.Repositories
{    
    public class ADMStaffRepository : IADMStaffRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMEmployee> staffs { get; set; }
        ADMEmployee staff = new();

        public ADMStaffRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
               
        public async Task<IReadOnlyList<ADMEmployee>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: //Filter By StatusType
                            sql = "SELECT A.StaffID, A.TermID, A.SchInfoID, A.PrefixID, A.EmployeeID, A.TitleID, A.EmployeeGroupID, A.GenderID, A.MStatusID, " +
                                "A.StatusTypeID, A.JobTypeID,  A.StudentCount, A.CountryID, A.StateID, A.LGAID, A.NationalID, A.PensionID, A.HealthInsureNo, " +
                                "A.ACL, A.Surname, A.FirstName, A.MiddleName, A.JobTitle, A.Email, A.BirthDate, A.HireDate, A.Qualification, A.EmployeeAddr, " +
                                "A.PhoneNos, A.PhoneNosAlternate,  A.NextOfKin, A.NextOfKinPhone, A.NextOfKinAddress, A.NextOfKinRelationship, A.BankID, " +
                                "A.AcctName, A.AcctNumber, A.Memo, A.photoStatus, A.employeePicture, A.ReligionID, A.LocID, A.signPicture, A.DeleteName, A.RoleID, " +
                                "A.ResetPassword, LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, " +
                                "C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') AS StaffNo, D.Title, E.EmployeeGroup, E.MarkAsTeacher, " +
                                "E.MarkAsPrincipal, F.Gender, G.MStatus, H.StatusType, I.JobType, J.Country, K.State, L.LGA, M.BankAcctName, N.Religion, O.Location, " +
                                "P.RoleDesc, A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StaffName, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo, " +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS StaffInitialsWithNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName, " +
                                "ROW_NUMBER() Over (PARTITION BY E.EmployeeGroup ORDER BY A.Surname) AS SN, " +
                                "A.AddOn, A.ModifiedOn, A.StaffPIN, A.ResetPassword, A.Password, A.RefreshToken, A.RefreshTokenExpiryTime " +
                                "FROM ADMEmployee A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle D ON D.TitleID = A.TitleID " +
                                "LEFT OUTER JOIN ADMEmployeeDepts E ON E.EmployeeGroupID = A.EmployeeGroupID " +
                                "LEFT OUTER JOIN SETGender F ON F.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMEmployeeMaritalStatus G ON G.MStatusID = A.MStatusID " +
                                "LEFT OUTER JOIN SETStatusType H ON H.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN ADMEmployeeJobType I ON I.JobTypeID = A.JobTypeID " +
                                "LEFT OUTER JOIN SETCountries J ON J.CountryID = A.CountryID " +
                                "LEFT OUTER JOIN SETStates K ON K.StateID = A.StateID " +
                                "LEFT OUTER JOIN SETLGA L ON L.LGAID = A.LGAID " +
                                "LEFT OUTER JOIN FINBankDetails M ON M.BankID = A.BankID " +
                                "LEFT OUTER JOIN SETReligion N ON N.ReligionID = A.ReligionID " +
                                "LEFT OUTER JOIN ADMEmployeeLocation O ON O.LocID = A.LocID " +
                                "LEFT OUTER JOIN SETRole P ON P.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID ORDER BY E.EmployeeGroup, A.Surname;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql,
                                new
                                {
                                    _switch.StatusTypeID 
                                });
                            break;
                        case 2: //Filter By Department And StatusType
                            sql = "SELECT A.StaffID, A.TermID, A.SchInfoID, A.PrefixID, A.EmployeeID, A.TitleID, A.EmployeeGroupID, A.GenderID, A.MStatusID, " +
                                "A.StatusTypeID, A.JobTypeID,  A.StudentCount, A.CountryID, A.StateID, A.LGAID, A.NationalID, A.PensionID, A.HealthInsureNo, " +
                                "A.ACL, A.Surname, A.FirstName, A.MiddleName, A.JobTitle, A.Email, A.BirthDate, A.HireDate, A.Qualification, A.EmployeeAddr, " +
                                "A.PhoneNos, A.PhoneNosAlternate,  A.NextOfKin, A.NextOfKinPhone, A.NextOfKinAddress, A.NextOfKinRelationship, A.BankID, " +
                                "A.AcctName, A.AcctNumber, A.Memo, A.photoStatus, A.employeePicture, A.ReligionID, A.LocID, A.signPicture, A.DeleteName, A.RoleID, " +
                                "A.ResetPassword, LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, " +
                                "C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') AS StaffNo, D.Title, E.EmployeeGroup, E.MarkAsTeacher, " +
                                "E.MarkAsPrincipal, F.Gender, G.MStatus, H.StatusType, I.JobType, J.Country, K.State, L.LGA, M.BankAcctName, N.Religion, O.Location, " +
                                "P.RoleDesc, A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StaffName, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo, " +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS StaffInitialsWithNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName, " +
                                "ROW_NUMBER() Over (PARTITION BY E.EmployeeGroup ORDER BY A.Surname) AS SN, " +
                                "A.AddOn, A.ModifiedOn, A.StaffPIN, A.ResetPassword, A.Password, A.RefreshToken, A.RefreshTokenExpiryTime " +
                                "FROM ADMEmployee A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle D ON D.TitleID = A.TitleID " +
                                "LEFT OUTER JOIN ADMEmployeeDepts E ON E.EmployeeGroupID = A.EmployeeGroupID " +
                                "LEFT OUTER JOIN SETGender F ON F.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMEmployeeMaritalStatus G ON G.MStatusID = A.MStatusID " +
                                "LEFT OUTER JOIN SETStatusType H ON H.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN ADMEmployeeJobType I ON I.JobTypeID = A.JobTypeID " +
                                "LEFT OUTER JOIN SETCountries J ON J.CountryID = A.CountryID " +
                                "LEFT OUTER JOIN SETStates K ON K.StateID = A.StateID " +
                                "LEFT OUTER JOIN SETLGA L ON L.LGAID = A.LGAID " +
                                "LEFT OUTER JOIN FINBankDetails M ON M.BankID = A.BankID " +
                                "LEFT OUTER JOIN SETReligion N ON N.ReligionID = A.ReligionID " +
                                "LEFT OUTER JOIN ADMEmployeeLocation O ON O.LocID = A.LocID " +
                                "LEFT OUTER JOIN SETRole P ON P.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False'  AND A.StatusTypeID = @StatusTypeID AND A.EmployeeGroupID = @EmployeeGroupID " +
                                "ORDER BY E.EmployeeGroup, A.Surname;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql,
                                new
                                {
                                    _switch.StatusTypeID,
                                    _switch.EmployeeGroupID
                                });
                            break;
                        case 3: //Filter By JobType And StatusType
                            sql = "SELECT A.StaffID, A.TermID, A.SchInfoID, A.PrefixID, A.EmployeeID, A.TitleID, A.EmployeeGroupID, A.GenderID, A.MStatusID, " +
                                "A.StatusTypeID, A.JobTypeID,  A.StudentCount, A.CountryID, A.StateID, A.LGAID, A.NationalID, A.PensionID, A.HealthInsureNo, " +
                                "A.ACL, A.Surname, A.FirstName, A.MiddleName, A.JobTitle, A.Email, A.BirthDate, A.HireDate, A.Qualification, A.EmployeeAddr, " +
                                "A.PhoneNos, A.PhoneNosAlternate,  A.NextOfKin, A.NextOfKinPhone, A.NextOfKinAddress, A.NextOfKinRelationship, A.BankID, " +
                                "A.AcctName, A.AcctNumber, A.Memo, A.photoStatus, A.employeePicture, A.ReligionID, A.LocID, A.signPicture, A.DeleteName, A.RoleID, " +
                                "A.ResetPassword, LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, " +
                                "C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') AS StaffNo, D.Title, E.EmployeeGroup, E.MarkAsTeacher, " +
                                "E.MarkAsPrincipal, F.Gender, G.MStatus, H.StatusType, I.JobType, J.Country, K.State, L.LGA, M.BankAcctName, N.Religion, O.Location, " +
                                "P.RoleDesc, A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StaffName, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo, " +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS StaffInitialsWithNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName, " +
                                "ROW_NUMBER() Over (PARTITION BY E.EmployeeGroup ORDER BY A.Surname) AS SN, " +
                                "A.AddOn, A.ModifiedOn, A.StaffPIN, A.ResetPassword, A.Password, A.RefreshToken, A.RefreshTokenExpiryTime " +
                                "FROM ADMEmployee A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle D ON D.TitleID = A.TitleID " +
                                "LEFT OUTER JOIN ADMEmployeeDepts E ON E.EmployeeGroupID = A.EmployeeGroupID " +
                                "LEFT OUTER JOIN SETGender F ON F.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMEmployeeMaritalStatus G ON G.MStatusID = A.MStatusID " +
                                "LEFT OUTER JOIN SETStatusType H ON H.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN ADMEmployeeJobType I ON I.JobTypeID = A.JobTypeID " +
                                "LEFT OUTER JOIN SETCountries J ON J.CountryID = A.CountryID " +
                                "LEFT OUTER JOIN SETStates K ON K.StateID = A.StateID " +
                                "LEFT OUTER JOIN SETLGA L ON L.LGAID = A.LGAID " +
                                "LEFT OUTER JOIN FINBankDetails M ON M.BankID = A.BankID " +
                                "LEFT OUTER JOIN SETReligion N ON N.ReligionID = A.ReligionID " +
                                "LEFT OUTER JOIN ADMEmployeeLocation O ON O.LocID = A.LocID " +
                                "LEFT OUTER JOIN SETRole P ON P.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID AND A.JobTypeID = @JobTypeID " +
                                "ORDER BY E.EmployeeGroup, A.Surname;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql,
                                new
                                {
                                    _switch.StatusTypeID,
                                    _switch.JobTypeID
                                });
                            break;
                        case 4: //Filter By Branch/Location And StatusType
                            sql = "SELECT A.StaffID, A.TermID, A.SchInfoID, A.PrefixID, A.EmployeeID, A.TitleID, A.EmployeeGroupID, A.GenderID, A.MStatusID, " +
                                "A.StatusTypeID, A.JobTypeID,  A.StudentCount, A.CountryID, A.StateID, A.LGAID, A.NationalID, A.PensionID, A.HealthInsureNo, " +
                                "A.ACL, A.Surname, A.FirstName, A.MiddleName, A.JobTitle, A.Email, A.BirthDate, A.HireDate, A.Qualification, A.EmployeeAddr, " +
                                "A.PhoneNos, A.PhoneNosAlternate,  A.NextOfKin, A.NextOfKinPhone, A.NextOfKinAddress, A.NextOfKinRelationship, A.BankID, " +
                                "A.AcctName, A.AcctNumber, A.Memo, A.photoStatus, A.employeePicture, A.ReligionID, A.LocID, A.signPicture, A.DeleteName, A.RoleID, " +
                                "A.ResetPassword, LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, " +
                                "C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') AS StaffNo, D.Title, E.EmployeeGroup, E.MarkAsTeacher, " +
                                "E.MarkAsPrincipal, F.Gender, G.MStatus, H.StatusType, I.JobType, J.Country, K.State, L.LGA, M.BankAcctName, N.Religion, O.Location, " +
                                "P.RoleDesc, A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StaffName, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo, " +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS StaffInitialsWithNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName, " +
                                "ROW_NUMBER() Over (PARTITION BY E.EmployeeGroup ORDER BY A.Surname) AS SN, " +
                                "A.AddOn, A.ModifiedOn, A.StaffPIN, A.ResetPassword, A.Password, A.RefreshToken, A.RefreshTokenExpiryTime " +
                                "FROM ADMEmployee A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle D ON D.TitleID = A.TitleID " +
                                "LEFT OUTER JOIN ADMEmployeeDepts E ON E.EmployeeGroupID = A.EmployeeGroupID " +
                                "LEFT OUTER JOIN SETGender F ON F.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMEmployeeMaritalStatus G ON G.MStatusID = A.MStatusID " +
                                "LEFT OUTER JOIN SETStatusType H ON H.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN ADMEmployeeJobType I ON I.JobTypeID = A.JobTypeID " +
                                "LEFT OUTER JOIN SETCountries J ON J.CountryID = A.CountryID " +
                                "LEFT OUTER JOIN SETStates K ON K.StateID = A.StateID " +
                                "LEFT OUTER JOIN SETLGA L ON L.LGAID = A.LGAID " +
                                "LEFT OUTER JOIN FINBankDetails M ON M.BankID = A.BankID " +
                                "LEFT OUTER JOIN SETReligion N ON N.ReligionID = A.ReligionID " +
                                "LEFT OUTER JOIN ADMEmployeeLocation O ON O.LocID = A.LocID " +
                                "LEFT OUTER JOIN SETRole P ON P.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False'  AND A.StatusTypeID = @StatusTypeID AND A.LocID = @LocID " +
                                "ORDER BY E.EmployeeGroup, A.Surname;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql,
                                new
                                {
                                    _switch.StatusTypeID,
                                    _switch.LocID
                                });
                            break;
                        case 5: //Filter By Department, JobType And StatusType
                            sql = "SELECT A.StaffID, A.TermID, A.SchInfoID, A.PrefixID, A.EmployeeID, A.TitleID, A.EmployeeGroupID, A.GenderID, A.MStatusID, " +
                                "A.StatusTypeID, A.JobTypeID,  A.StudentCount, A.CountryID, A.StateID, A.LGAID, A.NationalID, A.PensionID, A.HealthInsureNo, " +
                                "A.ACL, A.Surname, A.FirstName, A.MiddleName, A.JobTitle, A.Email, A.BirthDate, A.HireDate, A.Qualification, A.EmployeeAddr, " +
                                "A.PhoneNos, A.PhoneNosAlternate,  A.NextOfKin, A.NextOfKinPhone, A.NextOfKinAddress, A.NextOfKinRelationship, A.BankID, " +
                                "A.AcctName, A.AcctNumber, A.Memo, A.photoStatus, A.employeePicture, A.ReligionID, A.LocID, A.signPicture, A.DeleteName, A.RoleID, " +
                                "A.ResetPassword, LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, " +
                                "C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') AS StaffNo, D.Title, E.EmployeeGroup, E.MarkAsTeacher, " +
                                "E.MarkAsPrincipal, F.Gender, G.MStatus, H.StatusType, I.JobType, J.Country, K.State, L.LGA, M.BankAcctName, N.Religion, O.Location, " +
                                "P.RoleDesc, A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StaffName, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo, " +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS StaffInitialsWithNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName, " +
                                "ROW_NUMBER() Over (PARTITION BY E.EmployeeGroup ORDER BY A.Surname) AS SN, " +
                                "A.AddOn, A.ModifiedOn, A.StaffPIN, A.ResetPassword, A.Password, A.RefreshToken, A.RefreshTokenExpiryTime " +
                                "FROM ADMEmployee A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle D ON D.TitleID = A.TitleID " +
                                "LEFT OUTER JOIN ADMEmployeeDepts E ON E.EmployeeGroupID = A.EmployeeGroupID " +
                                "LEFT OUTER JOIN SETGender F ON F.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMEmployeeMaritalStatus G ON G.MStatusID = A.MStatusID " +
                                "LEFT OUTER JOIN SETStatusType H ON H.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN ADMEmployeeJobType I ON I.JobTypeID = A.JobTypeID " +
                                "LEFT OUTER JOIN SETCountries J ON J.CountryID = A.CountryID " +
                                "LEFT OUTER JOIN SETStates K ON K.StateID = A.StateID " +
                                "LEFT OUTER JOIN SETLGA L ON L.LGAID = A.LGAID " +
                                "LEFT OUTER JOIN FINBankDetails M ON M.BankID = A.BankID " +
                                "LEFT OUTER JOIN SETReligion N ON N.ReligionID = A.ReligionID " +
                                "LEFT OUTER JOIN ADMEmployeeLocation O ON O.LocID = A.LocID " +
                                "LEFT OUTER JOIN SETRole P ON P.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID " +
                                "AND A.EmployeeGroupID = @EmployeeGroupID AND A.JobTypeID = @JobTypeID " +
                                "ORDER BY E.EmployeeGroup, A.Surname;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql,
                                new
                                {
                                    _switch.StatusTypeID,
                                    _switch.EmployeeGroupID,
                                    _switch.JobTypeID
                                });
                            break;
                        case 6:  //Filter By Department, Branch/Location And StatusType
                            sql = "SELECT A.StaffID, A.TermID, A.SchInfoID, A.PrefixID, A.EmployeeID, A.TitleID, A.EmployeeGroupID, A.GenderID, A.MStatusID, " +
                                "A.StatusTypeID, A.JobTypeID,  A.StudentCount, A.CountryID, A.StateID, A.LGAID, A.NationalID, A.PensionID, A.HealthInsureNo, " +
                                "A.ACL, A.Surname, A.FirstName, A.MiddleName, A.JobTitle, A.Email, A.BirthDate, A.HireDate, A.Qualification, A.EmployeeAddr, " +
                                "A.PhoneNos, A.PhoneNosAlternate,  A.NextOfKin, A.NextOfKinPhone, A.NextOfKinAddress, A.NextOfKinRelationship, A.BankID, " +
                                "A.AcctName, A.AcctNumber, A.Memo, A.photoStatus, A.employeePicture, A.ReligionID, A.LocID, A.signPicture, A.DeleteName, A.RoleID, " +
                                "A.ResetPassword, LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, " +
                                "C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') AS StaffNo, D.Title, E.EmployeeGroup, E.MarkAsTeacher, " +
                                "E.MarkAsPrincipal, F.Gender, G.MStatus, H.StatusType, I.JobType, J.Country, K.State, L.LGA, M.BankAcctName, N.Religion, O.Location, " +
                                "P.RoleDesc, A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StaffName, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo, " +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS StaffInitialsWithNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName, " +
                                "ROW_NUMBER() Over (PARTITION BY E.EmployeeGroup ORDER BY A.Surname) AS SN, " +
                                "A.AddOn, A.ModifiedOn, A.StaffPIN, A.ResetPassword, A.Password, A.RefreshToken, A.RefreshTokenExpiryTime " +
                                "FROM ADMEmployee A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle D ON D.TitleID = A.TitleID " +
                                "LEFT OUTER JOIN ADMEmployeeDepts E ON E.EmployeeGroupID = A.EmployeeGroupID " +
                                "LEFT OUTER JOIN SETGender F ON F.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMEmployeeMaritalStatus G ON G.MStatusID = A.MStatusID " +
                                "LEFT OUTER JOIN SETStatusType H ON H.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN ADMEmployeeJobType I ON I.JobTypeID = A.JobTypeID " +
                                "LEFT OUTER JOIN SETCountries J ON J.CountryID = A.CountryID " +
                                "LEFT OUTER JOIN SETStates K ON K.StateID = A.StateID " +
                                "LEFT OUTER JOIN SETLGA L ON L.LGAID = A.LGAID " +
                                "LEFT OUTER JOIN FINBankDetails M ON M.BankID = A.BankID " +
                                "LEFT OUTER JOIN SETReligion N ON N.ReligionID = A.ReligionID " +
                                "LEFT OUTER JOIN ADMEmployeeLocation O ON O.LocID = A.LocID " +
                                "LEFT OUTER JOIN SETRole P ON P.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID " +
                                "AND A.EmployeeGroupID = @EmployeeGroupID AND A.LocID = @LocID  " +
                                "ORDER BY E.EmployeeGroup, A.Surname;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql,
                                new
                                {
                                    _switch.StatusTypeID,
                                    _switch.EmployeeGroupID,
                                    _switch.LocID
                                });
                            break;
                        case 7:  //Filter By Department, JobType, Branch/Location And StatusType
                            sql = "SELECT A.StaffID, A.TermID, A.SchInfoID, A.PrefixID, A.EmployeeID, A.TitleID, A.EmployeeGroupID, A.GenderID, A.MStatusID, " +
                                "A.StatusTypeID, A.JobTypeID,  A.StudentCount, A.CountryID, A.StateID, A.LGAID, A.NationalID, A.PensionID, A.HealthInsureNo, " +
                                "A.ACL, A.Surname, A.FirstName, A.MiddleName, A.JobTitle, A.Email, A.BirthDate, A.HireDate, A.Qualification, A.EmployeeAddr, " +
                                "A.PhoneNos, A.PhoneNosAlternate,  A.NextOfKin, A.NextOfKinPhone, A.NextOfKinAddress, A.NextOfKinRelationship, A.BankID, " +
                                "A.AcctName, A.AcctNumber, A.Memo, A.photoStatus, A.employeePicture, A.ReligionID, A.LocID, A.signPicture, A.DeleteName, A.RoleID, " +
                                "A.ResetPassword, LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, " +
                                "C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') AS StaffNo, D.Title, E.EmployeeGroup, E.MarkAsTeacher, " +
                                "E.MarkAsPrincipal, F.Gender, G.MStatus, H.StatusType, I.JobType, J.Country, K.State, L.LGA, M.BankAcctName, N.Religion, O.Location, " +
                                "P.RoleDesc, A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StaffName, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo, " +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS StaffInitialsWithNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName, " +
                                "ROW_NUMBER() Over (PARTITION BY E.EmployeeGroup ORDER BY A.Surname) AS SN, " +
                                "A.AddOn, A.ModifiedOn, A.StaffPIN, A.ResetPassword, A.Password, A.RefreshToken, A.RefreshTokenExpiryTime " + "FROM ADMEmployee A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle D ON D.TitleID = A.TitleID " +
                                "LEFT OUTER JOIN ADMEmployeeDepts E ON E.EmployeeGroupID = A.EmployeeGroupID " +
                                "LEFT OUTER JOIN SETGender F ON F.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMEmployeeMaritalStatus G ON G.MStatusID = A.MStatusID " +
                                "LEFT OUTER JOIN SETStatusType H ON H.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN ADMEmployeeJobType I ON I.JobTypeID = A.JobTypeID " +
                                "LEFT OUTER JOIN SETCountries J ON J.CountryID = A.CountryID " +
                                "LEFT OUTER JOIN SETStates K ON K.StateID = A.StateID " +
                                "LEFT OUTER JOIN SETLGA L ON L.LGAID = A.LGAID " +
                                "LEFT OUTER JOIN FINBankDetails M ON M.BankID = A.BankID " +
                                "LEFT OUTER JOIN SETReligion N ON N.ReligionID = A.ReligionID " +
                                "LEFT OUTER JOIN ADMEmployeeLocation O ON O.LocID = A.LocID " +
                                "LEFT OUTER JOIN SETRole P ON P.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID " +
                                "AND A.EmployeeGroupID = @EmployeeGroupID AND A.JobTypeID = @JobTypeID AND A.LocID = @LocID  " +
                                "ORDER BY E.EmployeeGroup, A.Surname;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql,
                                new
                                {
                                    _switch.StatusTypeID,
                                    _switch.EmployeeGroupID,
                                    _switch.JobTypeID,
                                    _switch.LocID
                                });
                            break;
                        case 8:  //Filter By Teacher And StatusType
                            sql = "SELECT A.StaffID, A.TermID, A.SchInfoID, A.PrefixID, A.EmployeeID, A.TitleID, A.EmployeeGroupID, A.GenderID, A.MStatusID, " +
                                "A.StatusTypeID, A.JobTypeID,  A.StudentCount, A.CountryID, A.StateID, A.LGAID, A.NationalID, A.PensionID, A.HealthInsureNo, " +
                                "A.ACL, A.Surname, A.FirstName, A.MiddleName, A.JobTitle, A.Email, A.BirthDate, A.HireDate, A.Qualification, A.EmployeeAddr, " +
                                "A.PhoneNos, A.PhoneNosAlternate,  A.NextOfKin, A.NextOfKinPhone, A.NextOfKinAddress, A.NextOfKinRelationship, A.BankID, " +
                                "A.AcctName, A.AcctNumber, A.Memo, A.photoStatus, A.employeePicture, A.ReligionID, A.LocID, A.signPicture, A.DeleteName, A.RoleID, " +
                                "A.ResetPassword, LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, " +
                                "C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') AS StaffNo, D.Title, E.EmployeeGroup, E.MarkAsTeacher, " +
                                "E.MarkAsPrincipal, F.Gender, G.MStatus, H.StatusType, I.JobType, J.Country, K.State, L.LGA, M.BankAcctName, N.Religion, O.Location, " +
                                "P.RoleDesc, A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StaffName, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo, " +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS StaffInitialsWithNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName, " +
                                "ROW_NUMBER() Over (PARTITION BY E.EmployeeGroup ORDER BY A.Surname) AS SN, " +
                                "A.AddOn, A.ModifiedOn, A.StaffPIN, A.ResetPassword, A.Password, A.RefreshToken, A.RefreshTokenExpiryTime " +
                                "FROM ADMEmployee A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle D ON D.TitleID = A.TitleID " +
                                "LEFT OUTER JOIN ADMEmployeeDepts E ON E.EmployeeGroupID = A.EmployeeGroupID " +
                                "LEFT OUTER JOIN SETGender F ON F.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMEmployeeMaritalStatus G ON G.MStatusID = A.MStatusID " +
                                "LEFT OUTER JOIN SETStatusType H ON H.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN ADMEmployeeJobType I ON I.JobTypeID = A.JobTypeID " +
                                "LEFT OUTER JOIN SETCountries J ON J.CountryID = A.CountryID " +
                                "LEFT OUTER JOIN SETStates K ON K.StateID = A.StateID " +
                                "LEFT OUTER JOIN SETLGA L ON L.LGAID = A.LGAID " +
                                "LEFT OUTER JOIN FINBankDetails M ON M.BankID = A.BankID " +
                                "LEFT OUTER JOIN SETReligion N ON N.ReligionID = A.ReligionID " +
                                "LEFT OUTER JOIN ADMEmployeeLocation O ON O.LocID = A.LocID " +
                                "LEFT OUTER JOIN SETRole P ON P.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID " +
                                "AND E.MarkAsTeacher = 'True' ORDER BY E.EmployeeGroup, A.Surname;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql,
                                new
                                {
                                    _switch.StatusTypeID
                                });
                            break;
                        case 9: //Filter By Principal And StatusType
                            sql = "SELECT A.StaffID, A.TermID, A.SchInfoID, A.PrefixID, A.EmployeeID, A.TitleID, A.EmployeeGroupID, A.GenderID, A.MStatusID, " +
                                "A.StatusTypeID, A.JobTypeID,  A.StudentCount, A.CountryID, A.StateID, A.LGAID, A.NationalID, A.PensionID, A.HealthInsureNo, " +
                                "A.ACL, A.Surname, A.FirstName, A.MiddleName, A.JobTitle, A.Email, A.BirthDate, A.HireDate, A.Qualification, A.EmployeeAddr, " +
                                "A.PhoneNos, A.PhoneNosAlternate,  A.NextOfKin, A.NextOfKinPhone, A.NextOfKinAddress, A.NextOfKinRelationship, A.BankID, " +
                                "A.AcctName, A.AcctNumber, A.Memo, A.photoStatus, A.employeePicture, A.ReligionID, A.LocID, A.signPicture, A.DeleteName, A.RoleID, " +
                                "A.ResetPassword, LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, " +
                                "C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') AS StaffNo, D.Title, E.EmployeeGroup, E.MarkAsTeacher, " +
                                "E.MarkAsPrincipal, F.Gender, G.MStatus, H.StatusType, I.JobType, J.Country, K.State, L.LGA, M.BankAcctName, N.Religion, O.Location, " +
                                "P.RoleDesc, A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StaffName, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo, " +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS StaffInitialsWithNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName, " +
                                "ROW_NUMBER() Over (PARTITION BY E.EmployeeGroup ORDER BY A.Surname) AS SN, " +
                                "A.AddOn, A.ModifiedOn, A.StaffPIN, A.ResetPassword, A.Password, A.RefreshToken, A.RefreshTokenExpiryTime " +
                                "FROM ADMEmployee A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle D ON D.TitleID = A.TitleID " +
                                "LEFT OUTER JOIN ADMEmployeeDepts E ON E.EmployeeGroupID = A.EmployeeGroupID " +
                                "LEFT OUTER JOIN SETGender F ON F.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMEmployeeMaritalStatus G ON G.MStatusID = A.MStatusID " +
                                "LEFT OUTER JOIN SETStatusType H ON H.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN ADMEmployeeJobType I ON I.JobTypeID = A.JobTypeID " +
                                "LEFT OUTER JOIN SETCountries J ON J.CountryID = A.CountryID " +
                                "LEFT OUTER JOIN SETStates K ON K.StateID = A.StateID " +
                                "LEFT OUTER JOIN SETLGA L ON L.LGAID = A.LGAID " +
                                "LEFT OUTER JOIN FINBankDetails M ON M.BankID = A.BankID " +
                                "LEFT OUTER JOIN SETReligion N ON N.ReligionID = A.ReligionID " +
                                "LEFT OUTER JOIN ADMEmployeeLocation O ON O.LocID = A.LocID " +
                                "LEFT OUTER JOIN SETRole P ON P.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID " +
                                "AND E.MarkAsPrincipal = 'True' ORDER BY E.EmployeeGroup, A.Surname;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql,
                                new
                                {
                                    _switch.StatusTypeID
                                });
                            break;
                        case 10:  //Filter By Teachers - Both Active And InActive
                            sql = "SELECT A.StaffID, A.TermID, A.SchInfoID, A.PrefixID, A.EmployeeID, A.TitleID, A.EmployeeGroupID, A.GenderID, A.MStatusID, " +
                                "A.StatusTypeID, A.JobTypeID,  A.StudentCount, A.CountryID, A.StateID, A.LGAID, A.NationalID, A.PensionID, A.HealthInsureNo, " +
                                "A.ACL, A.Surname, A.FirstName, A.MiddleName, A.JobTitle, A.Email, A.BirthDate, A.HireDate, A.Qualification, A.EmployeeAddr, " +
                                "A.PhoneNos, A.PhoneNosAlternate,  A.NextOfKin, A.NextOfKinPhone, A.NextOfKinAddress, A.NextOfKinRelationship, A.BankID, " +
                                "A.AcctName, A.AcctNumber, A.Memo, A.photoStatus, A.employeePicture, A.ReligionID, A.LocID, A.signPicture, A.DeleteName, A.RoleID, " +
                                "A.ResetPassword, LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, " +
                                "C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') AS StaffNo, D.Title, E.EmployeeGroup, E.MarkAsTeacher, " +
                                "E.MarkAsPrincipal, F.Gender, G.MStatus, H.StatusType, I.JobType, J.Country, K.State, L.LGA, M.BankAcctName, N.Religion, O.Location, " +
                                "P.RoleDesc, A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StaffName, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo, " +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS StaffInitialsWithNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName, " +
                                "ROW_NUMBER() Over (PARTITION BY E.EmployeeGroup ORDER BY A.Surname) AS SN, " +
                                "A.AddOn, A.ModifiedOn, A.StaffPIN, A.ResetPassword, A.Password, A.RefreshToken, A.RefreshTokenExpiryTime " +
                                "FROM ADMEmployee A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle D ON D.TitleID = A.TitleID " +
                                "LEFT OUTER JOIN ADMEmployeeDepts E ON E.EmployeeGroupID = A.EmployeeGroupID " +
                                "LEFT OUTER JOIN SETGender F ON F.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMEmployeeMaritalStatus G ON G.MStatusID = A.MStatusID " +
                                "LEFT OUTER JOIN SETStatusType H ON H.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN ADMEmployeeJobType I ON I.JobTypeID = A.JobTypeID " +
                                "LEFT OUTER JOIN SETCountries J ON J.CountryID = A.CountryID " +
                                "LEFT OUTER JOIN SETStates K ON K.StateID = A.StateID " +
                                "LEFT OUTER JOIN SETLGA L ON L.LGAID = A.LGAID " +
                                "LEFT OUTER JOIN FINBankDetails M ON M.BankID = A.BankID " +
                                "LEFT OUTER JOIN SETReligion N ON N.ReligionID = A.ReligionID " +
                                "LEFT OUTER JOIN ADMEmployeeLocation O ON O.LocID = A.LocID " +
                                "LEFT OUTER JOIN SETRole P ON P.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' AND E.MarkAsTeacher = 'True' " +
                                "ORDER BY E.EmployeeGroup, A.Surname;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql, new { });
                            break;
                        case 11:  //Filter By Principals - Both Active And InActive
                            sql = "SELECT A.StaffID, A.TermID, A.SchInfoID, A.PrefixID, A.EmployeeID, A.TitleID, A.EmployeeGroupID, A.GenderID, A.MStatusID, " +
                                "A.StatusTypeID, A.JobTypeID,  A.StudentCount, A.CountryID, A.StateID, A.LGAID, A.NationalID, A.PensionID, A.HealthInsureNo, " +
                                "A.ACL, A.Surname, A.FirstName, A.MiddleName, A.JobTitle, A.Email, A.BirthDate, A.HireDate, A.Qualification, A.EmployeeAddr, " +
                                "A.PhoneNos, A.PhoneNosAlternate,  A.NextOfKin, A.NextOfKinPhone, A.NextOfKinAddress, A.NextOfKinRelationship, A.BankID, " +
                                "A.AcctName, A.AcctNumber, A.Memo, A.photoStatus, A.employeePicture, A.ReligionID, A.LocID, A.signPicture, A.DeleteName, A.RoleID, " +
                                "A.ResetPassword, LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, " +
                                "C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') AS StaffNo, D.Title, E.EmployeeGroup, E.MarkAsTeacher, " +
                                "E.MarkAsPrincipal, F.Gender, G.MStatus, H.StatusType, I.JobType, J.Country, K.State, L.LGA, M.BankAcctName, N.Religion, O.Location, " +
                                "P.RoleDesc, A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StaffName, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo, " +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS StaffInitialsWithNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName, " +
                                "ROW_NUMBER() Over (PARTITION BY E.EmployeeGroup ORDER BY A.Surname) AS SN, " +
                                "A.AddOn, A.ModifiedOn, A.StaffPIN, A.ResetPassword, A.Password, A.RefreshToken, A.RefreshTokenExpiryTime " +
                                "FROM ADMEmployee A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle D ON D.TitleID = A.TitleID " +
                                "LEFT OUTER JOIN ADMEmployeeDepts E ON E.EmployeeGroupID = A.EmployeeGroupID " +
                                "LEFT OUTER JOIN SETGender F ON F.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMEmployeeMaritalStatus G ON G.MStatusID = A.MStatusID " +
                                "LEFT OUTER JOIN SETStatusType H ON H.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN ADMEmployeeJobType I ON I.JobTypeID = A.JobTypeID " +
                                "LEFT OUTER JOIN SETCountries J ON J.CountryID = A.CountryID " +
                                "LEFT OUTER JOIN SETStates K ON K.StateID = A.StateID " +
                                "LEFT OUTER JOIN SETLGA L ON L.LGAID = A.LGAID " +
                                "LEFT OUTER JOIN FINBankDetails M ON M.BankID = A.BankID " +
                                "LEFT OUTER JOIN SETReligion N ON N.ReligionID = A.ReligionID " +
                                "LEFT OUTER JOIN ADMEmployeeLocation O ON O.LocID = A.LocID " +
                                "LEFT OUTER JOIN SETRole P ON P.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' AND E.MarkAsPrincipal = 'True' " +
                                "ORDER BY E.EmployeeGroup, A.Surname;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql, new { });
                            break;
                        case 12: //Filter By Birthdate
                            sql = "SELECT A.StaffID, A.TermID, A.SchInfoID, A.PrefixID, A.EmployeeID, A.TitleID, A.EmployeeGroupID, A.GenderID, A.MStatusID, " +
                                "A.StatusTypeID, A.JobTypeID,  A.StudentCount, A.CountryID, A.StateID, A.LGAID, A.NationalID, A.PensionID, A.HealthInsureNo, " +
                                "A.ACL, A.Surname, A.FirstName, A.MiddleName, A.JobTitle, A.Email, A.BirthDate, A.HireDate, A.Qualification, A.EmployeeAddr, " +
                                "A.PhoneNos, A.PhoneNosAlternate,  A.NextOfKin, A.NextOfKinPhone, A.NextOfKinAddress, A.NextOfKinRelationship, A.BankID, " +
                                "A.AcctName, A.AcctNumber, A.Memo, A.photoStatus, A.employeePicture, A.ReligionID, A.LocID, A.signPicture, A.DeleteName, A.RoleID, " +
                                "A.ResetPassword, LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, " +
                                "C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') AS StaffNo, D.Title, E.EmployeeGroup, E.MarkAsTeacher, " +
                                "E.MarkAsPrincipal, F.Gender, G.MStatus, H.StatusType, I.JobType, J.Country, K.State, L.LGA, M.BankAcctName, N.Religion, O.Location, " +
                                "P.RoleDesc, A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StaffName, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo, " +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS StaffInitialsWithNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName, " +
                                "ROW_NUMBER() Over (PARTITION BY E.EmployeeGroup ORDER BY A.Surname) AS SN, " +
                                "A.AddOn, A.ModifiedOn, A.StaffPIN, A.ResetPassword, A.Password, A.RefreshToken, A.RefreshTokenExpiryTime " +
                                "FROM ADMEmployee A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle D ON D.TitleID = A.TitleID " +
                                "LEFT OUTER JOIN ADMEmployeeDepts E ON E.EmployeeGroupID = A.EmployeeGroupID " +
                                "LEFT OUTER JOIN SETGender F ON F.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMEmployeeMaritalStatus G ON G.MStatusID = A.MStatusID " +
                                "LEFT OUTER JOIN SETStatusType H ON H.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN ADMEmployeeJobType I ON I.JobTypeID = A.JobTypeID " +
                                "LEFT OUTER JOIN SETCountries J ON J.CountryID = A.CountryID " +
                                "LEFT OUTER JOIN SETStates K ON K.StateID = A.StateID " +
                                "LEFT OUTER JOIN SETLGA L ON L.LGAID = A.LGAID " +
                                "LEFT OUTER JOIN FINBankDetails M ON M.BankID = A.BankID " +
                                "LEFT OUTER JOIN SETReligion N ON N.ReligionID = A.ReligionID " +
                                "LEFT OUTER JOIN ADMEmployeeLocation O ON O.LocID = A.LocID " +
                                "LEFT OUTER JOIN SETRole P ON P.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID AND " +
                                "DATEPART( Week, DATEADD( Year, DATEPART( Year, GETDATE()) - DATEPART( Year, A.BirthDate), A.BirthDate)) = DATEPART(Week, GETDATE()) " +
                                "ORDER BY E.EmployeeGroup, A.Surname;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql,
                                new
                                {
                                    _switch.StatusTypeID
                                });
                            break;
                        default: //Filter By Staffs - Both Active And InActive
                            sql = "SELECT A.StaffID, A.TermID, A.SchInfoID, A.PrefixID, A.EmployeeID, A.TitleID, A.EmployeeGroupID, A.GenderID, A.MStatusID, " +
                                "A.StatusTypeID, A.JobTypeID,  A.StudentCount, A.CountryID, A.StateID, A.LGAID, A.NationalID, A.PensionID, A.HealthInsureNo, " +
                                "A.ACL, A.Surname, A.FirstName, A.MiddleName, A.JobTitle, A.Email, A.BirthDate, A.HireDate, A.Qualification, A.EmployeeAddr, " +
                                "A.PhoneNos, A.PhoneNosAlternate,  A.NextOfKin, A.NextOfKinPhone, A.NextOfKinAddress, A.NextOfKinRelationship, A.BankID, " +
                                "A.AcctName, A.AcctNumber, A.Memo, A.photoStatus, A.employeePicture, A.ReligionID, A.LocID, A.signPicture, A.DeleteName, A.RoleID, " +
                                "A.ResetPassword, LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, " +
                                "C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') AS StaffNo, D.Title, E.EmployeeGroup, E.MarkAsTeacher, " +
                                "E.MarkAsPrincipal, F.Gender, G.MStatus, H.StatusType, I.JobType, J.Country, K.State, L.LGA, M.BankAcctName, N.Religion, O.Location, " +
                                "P.RoleDesc, A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StaffName, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo, " +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS StaffInitialsWithNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName, " +
                                "ROW_NUMBER() Over (PARTITION BY E.EmployeeGroup ORDER BY A.Surname) AS SN, " +
                                "A.AddOn, A.ModifiedOn, A.StaffPIN, A.ResetPassword, A.Password, A.RefreshToken, A.RefreshTokenExpiryTime " +
                                "FROM ADMEmployee A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle D ON D.TitleID = A.TitleID " +
                                "LEFT OUTER JOIN ADMEmployeeDepts E ON E.EmployeeGroupID = A.EmployeeGroupID " +
                                "LEFT OUTER JOIN SETGender F ON F.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMEmployeeMaritalStatus G ON G.MStatusID = A.MStatusID " +
                                "LEFT OUTER JOIN SETStatusType H ON H.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN ADMEmployeeJobType I ON I.JobTypeID = A.JobTypeID " +
                                "LEFT OUTER JOIN SETCountries J ON J.CountryID = A.CountryID " +
                                "LEFT OUTER JOIN SETStates K ON K.StateID = A.StateID " +
                                "LEFT OUTER JOIN SETLGA L ON L.LGAID = A.LGAID " +
                                "LEFT OUTER JOIN FINBankDetails M ON M.BankID = A.BankID " +
                                "LEFT OUTER JOIN SETReligion N ON N.ReligionID = A.ReligionID " +
                                "LEFT OUTER JOIN ADMEmployeeLocation O ON O.LocID = A.LocID " +
                                "LEFT OUTER JOIN SETRole P ON P.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' ORDER BY E.EmployeeGroup, A.Surname;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql, new { });
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return staffs;
        }

        public async Task<ADMEmployee> GetByIdAsync(int id)
        {

            staff = new ADMEmployee();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.StaffID, A.TermID, A.SchInfoID, A.PrefixID, A.EmployeeID, A.TitleID, A.EmployeeGroupID, A.GenderID, A.MStatusID, " +
                "A.StatusTypeID, A.JobTypeID,  A.StudentCount, A.CountryID, A.StateID, A.LGAID, A.NationalID, A.PensionID, A.HealthInsureNo, " +
                "A.ACL, A.Surname, A.FirstName, A.MiddleName, A.JobTitle, A.Email, A.BirthDate, A.HireDate, A.Qualification, A.EmployeeAddr, " +
                "A.PhoneNos, A.PhoneNosAlternate,  A.NextOfKin, A.NextOfKinPhone, A.NextOfKinAddress, A.NextOfKinRelationship, A.BankID, " +
                "A.AcctName, A.AcctNumber, A.Memo, A.photoStatus, A.employeePicture, A.ReligionID, A.LocID, A.signPicture, A.DeleteName, A.RoleID, " +
                "A.ResetPassword, LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' ' + B.SchTerm AS AcademicSession, " +
                "C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') AS StaffNo, D.Title, E.EmployeeGroup, E.MarkAsTeacher, " +
                "E.MarkAsPrincipal, F.Gender, G.MStatus, H.StatusType, I.JobType, J.Country, K.State, L.LGA, M.BankAcctName, N.Religion, O.Location, " +
                "P.RoleDesc, A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StaffName, " +
                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo, " +
                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + C.PrefixName + REPLACE(STR(A.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName " +
                "FROM ADMEmployee A " +
                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                "LEFT OUTER JOIN ADMEmployeeTitle D ON D.TitleID = A.TitleID " +
                "LEFT OUTER JOIN ADMEmployeeDepts E ON E.EmployeeGroupID = A.EmployeeGroupID " +
                "LEFT OUTER JOIN SETGender F ON F.GenderID = A.GenderID " +
                "LEFT OUTER JOIN ADMEmployeeMaritalStatus G ON G.MStatusID = A.MStatusID " +
                "LEFT OUTER JOIN SETStatusType H ON H.StatusTypeID = A.StatusTypeID " +
                "LEFT OUTER JOIN ADMEmployeeJobType I ON I.JobTypeID = A.JobTypeID " +
                "LEFT OUTER JOIN SETCountries J ON J.CountryID = A.CountryID " +
                "LEFT OUTER JOIN SETStates K ON K.StateID = A.StateID " +
                "LEFT OUTER JOIN SETLGA L ON L.LGAID = A.LGAID " +
                "LEFT OUTER JOIN FINBankDetails M ON M.BankID = A.BankID " +
                "LEFT OUTER JOIN SETReligion N ON N.ReligionID = A.ReligionID " +
                "LEFT OUTER JOIN ADMEmployeeLocation O ON O.LocID = A.LocID " +
                "LEFT OUTER JOIN SETRole P ON P.RoleID = A.RoleID " +
                "WHERE A.StaffID = @StaffID;";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                staff = await connection.QuerySingleOrDefaultAsync<ADMEmployee>(sql, new { StaffID = id });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return staff;
        }

        public async Task<ADMEmployee> AddAsync(ADMEmployee entity)
        {
            entity.AddOn = DateTime.Now;
            entity.ModifiedOn = DateTime.Now;

             sql = @"INSERT INTO ADMEmployee (TermID, SchInfoID, EmployeeID, TitleID, EmployeeGroupID, GenderID, MStatusID, JobTypeID, 
                CountryID, StateID, LGAID, NationalID, PensionID, HealthInsureNo, ACL, Surname, FirstName, MiddleName,
                JobTitle, Email, BirthDate, HireDate, Qualification, EmployeeAddr, PhoneNos, PhoneNosAlternate, 
                NextOfKin, NextOfKinPhone, NextOfKinAddress, NextOfKinRelationship, BankID, AcctName, AcctNumber, Memo, 
                photoStatus, employeePicture, ReligionID, LocID, signPicture, RoleID) OUTPUT INSERTED.StaffID VALUES 
                (@TermID, @SchInfoID, @EmployeeID, @TitleID, @EmployeeGroupID, @GenderID, @MStatusID, @JobTypeID, 
                @CountryID, @StateID, @LGAID, @NationalID, @PensionID, @HealthInsureNo, @ACL, @Surname, @FirstName, 
                @MiddleName, @JobTitle, @Email, @BirthDate, @HireDate, @Qualification, @EmployeeAddr, @PhoneNos, 
                @PhoneNosAlternate, @NextOfKin, @NextOfKinPhone, @NextOfKinAddress, @NextOfKinRelationship, @BankID, 
                @AcctName, @AcctNumber, @Memo, @photoStatus, @employeePicture, @ReligionID, @LocID, @signPicture, @RoleID);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.StaffID = result;
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return entity;
        }

        public async Task<ADMEmployee> UpdateAsync(int id, ADMEmployee entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            entity.ModifiedOn = DateTime.Now;

            switch (id)
            {
                case 1: //Update Staff Info
                    sql = "UPDATE ADMEmployee SET TermID = @TermID, SchInfoID = @SchInfoID, EmployeeID = @EmployeeID, TitleID = @TitleID, " +
                       "EmployeeGroupID = @EmployeeGroupID, GenderID = @GenderID, MStatusID = @MStatusID, StatusTypeID = @StatusTypeID, " +
                       "JobTypeID = @JobTypeID, CountryID = @CountryID, StateID = @StateID, LGAID = @LGAID, NationalID = @NationalID, " +
                       "PensionID = @PensionID, HealthInsureNo = @HealthInsureNo, ACL = @ACL, Surname = @Surname, FirstName = @FirstName, " +
                       "MiddleName = @MiddleName, JobTitle = @JobTitle, Email = @Email, BirthDate = @BirthDate, HireDate = @HireDate, " +
                       "Qualification = @Qualification, EmployeeAddr = @EmployeeAddr, PhoneNos = @PhoneNos, PhoneNosAlternate = @PhoneNosAlternate, " +
                       "NextOfKin = @NextOfKin, NextOfKinPhone = @NextOfKinPhone, NextOfKinAddress = @NextOfKinAddress, " +
                       "NextOfKinRelationship = @NextOfKinRelationship, BankID = @BankID, AcctName = @AcctName, AcctNumber = @AcctNumber, Memo = @Memo, " +
                       "photoStatus = @photoStatus, employeePicture = @employeePicture, ReligionID = @ReligionID, LocID = @LocID, " +
                       "signPicture = @signPicture WHERE StaffID = @staffid;";
                    break;
                case 2: //Update Staff PIN
                    sql = "UPDATE ADMEmployee SET StaffPIN = @StaffPIN WHERE StaffID = @staffid;";
                    break;
                case 3: //Update Staff Email And Password
                    sql = "UPDATE ADMEmployee SET Email = @Email, Password = @Password WHERE StaffID = @staffid;";
                    break;
                case 4: //Update Staff Authentication Token
                    sql = "UPDATE ADMEmployee SET RefreshToken = @RefreshToken, RefreshTokenExpiryTime = @RefreshTokenExpiryTime " +
                        "WHERE StaffID = @staffid;";
                    break;
                case 5: //Update Staff Refresh Token
                    sql = "UPDATE ADMEmployee SET RefreshToken = @RefreshToken WHERE StaffID = @staffid;";
                    break;
                case 6: //Reset Staff Password
                    sql = "UPDATE ADMEmployee SET Password = @Password WHERE StaffID = @staffid;";
                    break;
                case 7: //Delete Staff
                    sql = "UPDATE ADMEmployee SET DeleteName = @DeleteName WHERE StaffID = @StaffID;";
                    break;
                case 8: //Batch Update of Staff Info
                    sql = "UPDATE ADMEmployee SET TermID = @TermID, EmployeeID = @EmployeeID, Surname = @Surname, FirstName = @FirstName, " +
                            "MiddleName = @MiddleName, PhoneNos = @PhoneNos, Email = @Email WHERE StaffID = @StaffID;";
                    break;
                case 9: //Update Staff Access Role
                    sql = "UPDATE ADMEmployee SET RoleID = @RoleID, Email = @Email, ResetPassword = @ResetPassword " +
                            "WHERE StaffID = @StaffID;";
                    break;
                case 10:
                    sql = "UPDATE ADMEmployee SET Id = @Id WHERE StaffID = @StaffID;";
                    break;
            }

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, entity);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            sql = "DELETE FROM ADMEmployee WHERE StaffID = @StaffID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { StaffID = id });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public Task<int> CountAsync(SwitchModel _switchd)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<ADMEmployee>> SearchAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: //Search By Staff PIN For Staff Login Registration
                            sql = "SELECT A.StaffID, A.Surname, A.FirstName, A.MiddleName, A.Email, A.Password, A.StaffPIN, A.RoleID, C.RoleDesc, A.RefreshToken, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + B.PrefixName + REPLACE(STR(A.EmployeeID, B.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo " +
                                "FROM ADMEmployee A " +
                                "INNER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                                "INNER JOIN SETRole C ON C.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID AND A.StaffPIN = @StaffPIN;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql,
                                new
                                {
                                    _switch.StatusTypeID,
                                    StaffPIN = _switch.SearchCriteriaA
                                });
                            break;
                        case 2: //Search By Staff Email And Password For Staff Login
                            sql = "SELECT A.StaffID, A.Surname, A.FirstName, A.MiddleName, A.Email, A.Password, A.StaffPIN, A.RoleID, C.RoleDesc, A.RefreshToken, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + B.PrefixName + REPLACE(STR(A.EmployeeID, B.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo " +
                                "FROM ADMEmployee A " +
                                "INNER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                                "INNER JOIN SETRole C ON C.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID " +
                                "AND A.Email = @Email AND A.Password = @Password;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql,
                                new
                                {
                                    _switch.StatusTypeID,
                                    Email = _switch.SearchCriteriaA,
                                    Password = _switch.SearchCriteriaB
                                });
                            break;
                        case 3: //Search By Staff Email And RefreshToken For Staff Login
                            sql = "SELECT A.StaffID, A.Surname, A.FirstName, A.MiddleName, A.Email, A.Password, A.StaffPIN, A.RoleID, C.RoleDesc, A.RefreshToken, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + B.PrefixName + REPLACE(STR(A.EmployeeID, B.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo " +
                                "FROM ADMEmployee A " +
                                "INNER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                                "INNER JOIN SETRole C ON C.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID " +
                                "AND A.Email = @Email AND A.RefreshToken = @RefreshToken;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql,
                                new
                                {
                                    _switch.StatusTypeID,
                                    Email = _switch.SearchCriteriaA,
                                    RefreshToken = _switch.SearchCriteriaB
                                });
                            break;
                        case 4: //Search By Staff Email For Password Reset
                            sql = "SELECT A.StaffID, A.Surname, A.FirstName, A.MiddleName, A.Email, A.Password, A.StaffPIN, A.RoleID, C.RoleDesc, A.RefreshToken, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + B.PrefixName + REPLACE(STR(A.EmployeeID, B.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo " +
                                "FROM ADMEmployee A " +
                                "INNER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                                "INNER JOIN SETRole C ON C.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID " +
                                "AND A.Email = @Email;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql,
                                new
                                {
                                    _switch.StatusTypeID,
                                    Email = _switch.SearchCriteriaA
                                });
                            break;
                        case 5: //Search For Staff By Staff ID
                            sql = "SELECT A.StaffID, A.Surname, A.FirstName, A.MiddleName, A.Email, A.Password, A.StaffPIN, A.RoleID, C.RoleDesc, A.RefreshToken, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') + ' [' + B.PrefixName + REPLACE(STR(A.EmployeeID, B.PrefixDigits), SPACE(1), '0') + ']' AS StaffNameWithNo " +
                                "FROM ADMEmployee A " +
                                "INNER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                                "INNER JOIN SETRole C ON C.RoleID = A.RoleID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID AND A.StaffID = @StaffID" +
                                "AND A.Email = @Email;";

                            staffs = (List<ADMEmployee>)await connection.QueryAsync<ADMEmployee>(sql,
                                new
                                {
                                    StatusTypeID = _switch.StatusTypeID,
                                    StaffID = _switch.SearchById
                                });
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return staffs;
        }
    }
    
    public class ADMStaffDeptsRepository : IADMStaffDeptsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMEmployeeDepts> departments { get; set; }
        ADMEmployeeDepts department = new();

        public ADMStaffDeptsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ADMEmployeeDepts>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        default:
                            sql = "SELECT * FROM ADMEmployeeDepts WHERE [Delete] = 'False' ORDER BY EmployeeGroup;";

                            departments = (IReadOnlyList<ADMEmployeeDepts>)await connection.QueryAsync<ADMEmployeeDepts>(sql, new { });
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return departments;
        }

        public async Task<ADMEmployeeDepts> GetByIdAsync(int id)
        {
            department = new ADMEmployeeDepts();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ADMEmployeeDepts WHERE EmployeeGroupID = @EmployeeGroupID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                department = await connection.QuerySingleOrDefaultAsync<ADMEmployeeDepts>(sql, new { EmployeeGroupID = id });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return department;
        }

        public async Task<ADMEmployeeDepts> AddAsync(ADMEmployeeDepts entity)
        {
            sql = @"INSERT INTO ADMEmployeeDepts (EmployeeGroup, AccessLevel, MarkAsTeacher, MarkAsPrincipal) OUTPUT INSERTED.EmployeeGroupID VALUES 
                (@EmployeeGroup, @AccessLevel, @MarkAsTeacher, @MarkAsPrincipal);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.EmployeeGroupID = result;
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return entity;
        }

        public async Task<ADMEmployeeDepts> UpdateAsync(int id, ADMEmployeeDepts entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMEmployeeDepts SET EmployeeGroup = @EmployeeGroup, MarkAsTeacher = @MarkAsTeacher, " +
                            "MarkAsPrincipal = @MarkAsPrincipal WHERE EmployeeGroupID = @EmployeeGroupID;";
                    break;
                case 2:
                    sql = "UPDATE ADMEmployeeDepts SET Id = @Id WHERE EmployeeGroupID = @EmployeeGroupID;";
                    break;
            }

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, entity);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            sql = "DELETE FROM ADMEmployeeDepts WHERE EmployeeGroupID = @EmployeeGroupID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { EmployeeGroupID = id });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public Task<int> CountAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ADMEmployeeDepts>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ADMStaffJobTypeRepository : IADMStaffJobTypeRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMEmployeeJobType> _list { get; set; }
        ADMEmployeeJobType _details = new();

        public ADMStaffJobTypeRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ADMEmployeeJobType>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ADMEmployeeJobType;";

                            _list = (List<ADMEmployeeJobType>)await connection.QueryAsync<ADMEmployeeJobType>(sql, new { });
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return _list;
        }

        public async Task<ADMEmployeeJobType> GetByIdAsync(int id)
        {
            _details = new ADMEmployeeJobType();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ADMEmployeeJobType WHERE JobTypeID = @JobTypeID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstAsync<ADMEmployeeJobType>(sql, new { JobTypeID = id });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return _details;
        }

        public async Task<ADMEmployeeJobType> AddAsync(ADMEmployeeJobType entity)
        {
            sql = @"INSERT INTO ADMEmployeeJobType (JobType)  OUTPUT INSERTED.JobTypeID VALUES (@JobType);";
           
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.JobTypeID = result;
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return entity;
        }

        public async Task<ADMEmployeeJobType> UpdateAsync(int id, ADMEmployeeJobType entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMEmployeeJobType SET JobType = @JobType WHERE JobTypeID = @JobTypeID;";
                    break;
                case 2:
                    sql = "UPDATE ADMEmployeeJobType SET Id = @Id WHERE JobTypeID = @JobTypeID;";
                    break;
            }

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, entity);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            sql = "DELETE FROM ADMEmployeeJobType WHERE JobTypeID = @JobTypeID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { JobTypeID = id });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public Task<int> CountAsync(SwitchModel _switchd)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ADMEmployeeJobType>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class StaffLocationRepository : IADMStaffLocationRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMEmployeeLocation> _list { get; set; }
        ADMEmployeeLocation _details = new();

        public StaffLocationRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ADMEmployeeLocation>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ADMEmployeeLocation;";

                            _list = (List<ADMEmployeeLocation>)await connection.QueryAsync<ADMEmployeeLocation>(sql, new { });
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return _list;
        }

        public async Task<ADMEmployeeLocation> GetByIdAsync(int id)
        {
            _details = new ADMEmployeeLocation();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ADMEmployeeLocation WHERE LocID = @LocID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstAsync<ADMEmployeeLocation>(sql, new { LocID = id });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return _details;
        }

        public async Task<ADMEmployeeLocation> AddAsync(ADMEmployeeLocation entity)
        {
            sql = @"INSERT INTO ADMEmployeeLocation (Location) OUTPUT INSERTED.LocID VALUES (@Location);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.LocID = result;
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return entity;
        }

        public async Task<ADMEmployeeLocation> UpdateAsync(int id, ADMEmployeeLocation entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMEmployeeLocation SET Location = @Location WHERE LocID = @LocID;";
                    break;
                case 2:
                    sql = "UPDATE ADMEmployeeLocation SET Id = @Id WHERE LocID = @LocID;";
                    break;
            }

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, entity);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            sql = "DELETE FROM ADMEmployeeLocation WHERE LocID = @LocID";
            
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { LocID = id });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public Task<int> CountAsync(SwitchModel _switchd)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ADMEmployeeLocation>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class StaffMaritalStatusRepository : IADMStaffMaritalStatusRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMEmployeeMaritalStatus> _list { get; set; }
        ADMEmployeeMaritalStatus _details = new();

        public StaffMaritalStatusRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ADMEmployeeMaritalStatus>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ADMEmployeeMaritalStatus;";

                            _list = (List<ADMEmployeeMaritalStatus>)await connection.QueryAsync<ADMEmployeeMaritalStatus>(sql, new { });
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return _list;
        }

        public async Task<ADMEmployeeMaritalStatus> GetByIdAsync(int id)
        {
            _details = new ADMEmployeeMaritalStatus();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ADMEmployeeMaritalStatus WHERE MStatusID = @MStatusID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstAsync<ADMEmployeeMaritalStatus>(sql, new { MStatusID = id });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return _details;
        }

        public async Task<ADMEmployeeMaritalStatus> AddAsync(ADMEmployeeMaritalStatus entity)
        {
            sql = @"INSERT INTO ADMEmployeeMaritalStatus (MStatus) OUTPUT INSERTED.MStatusID VALUES (@MStatus);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.MStatusID = result;
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return entity;
        }

        public async Task<ADMEmployeeMaritalStatus> UpdateAsync(int id, ADMEmployeeMaritalStatus entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMEmployeeMaritalStatus SET MStatus = @MStatus WHERE MStatusID = @MStatusID;";
                    break;
                case 2:
                    sql = "UPDATE ADMEmployeeMaritalStatus SET Id = @Id WHERE MStatusID = @MStatusID;";
                    break;
            }

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, entity);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            sql = "DELETE FROM ADMEmployeeMaritalStatus WHERE MStatusID = @MStatusID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { MStatusID = id });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public Task<int> CountAsync(SwitchModel _switchid)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ADMEmployeeMaritalStatus>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ADMStaffTitleRepository : IADMStaffTitleRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMEmployeeTitle> _list { get; set; }
        ADMEmployeeTitle _details = new();

        public ADMStaffTitleRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ADMEmployeeTitle>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ADMEmployeeTitle;";

                            _list = (List<ADMEmployeeTitle>)await connection.QueryAsync<ADMEmployeeTitle>(sql, new { });
                            break;
                    }
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return _list;
        }

        public async Task<ADMEmployeeTitle> GetByIdAsync(int id)
        {
            _details = new ADMEmployeeTitle();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ADMEmployeeTitle WHERE TitleID = @TitleID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstAsync<ADMEmployeeTitle>(sql, new { TitleID = id });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return _details;
        }

        public async Task<ADMEmployeeTitle> AddAsync(ADMEmployeeTitle entity)
        {
            sql = @"INSERT INTO ADMEmployeeTitle (Title) OUTPUT INSERTED.TitleID VALUES (@Title);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.TitleID = result;
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return entity;
        }

        public async Task<ADMEmployeeTitle> UpdateAsync(int id, ADMEmployeeTitle entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMEmployeeTitle SET Title = @Title WHERE TitleID = @TitleID;";
                    break;
                case 2:
                    sql = "UPDATE ADMEmployeeTitle SET Id = @Id WHERE MStatusID = @MStatusID;";
                    break;
            }

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, entity);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            sql = "DELETE FROM ADMEmployeeTitle WHERE TitleID = @TitleID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { TitleID = id });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return true;
        }

        public Task<int> CountAsync(SwitchModel _switchid)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ADMEmployeeTitle>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }
}
