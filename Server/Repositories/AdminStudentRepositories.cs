using WebAppAcademics.Server.Interfaces.Students;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Administration.Students;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace WebAppAcademics.Server.Repositories
{
    public class ADMStudentsRepository : IADMStudentsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMStudents> students { get; set; }
        ADMStudents student = new();

        public ADMStudentsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
               
        public async Task<IReadOnlyList<ADMStudents>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: //Filter By StatusType
                                //sql = "SELECT A.STDID, A.SchInfoID, A.TermID, A.SchID, A.ClassListID, A.ClassID, A.ClassTeacherID, A.DisciplineID, " +
                                //    "A.PayeeTypeID, A.PayeeID, A.StaffID, A.CountryID, A.StateID, A.LGAID, A.EDUID, A.ReligionID, A.ClubID, A.RoleID," +
                                //    "A.StudentTypeID, A.StatusTypeID, A.ExitID, A.ExitDate, A.GenderID, A.PrefixID, A.StudentID, A.Surname, A.FirstName, " +
                                //    "ISNULL(A.MiddleName, ' ') AS MiddleName, A.Enrollment, A.EnrollmentDate, A.Email, A.DOB, A.Address, A.PhoneNumber, " +
                                //    "A.AltPhoneNumber, A.Memo, A.DateCreated, A.ModifyDate, A.photoStatus, A.studentPhoto, A.Promoted, A.ExamNo, A.ExamNoNECO, " +
                                //    "A.DeleteName, A.ClassPrevious, A.ClassIDPrevious, A.PayStatus, A.ParentPin, A.StudentPin, A.Password, A.CBTLock, " +
                                //    "LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, C.School, " +
                                //    "A.PrefixID, D.PrefixName, D.PrefixDigits, D.PrefixName + REPLACE(STR(A.StudentID, D.PrefixDigits), SPACE(1), '0') AS AdmissionNo,  " +
                                //    "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, X.Religion, " +
                                //    "A.Surname + ' ' + A.FirstName + ' ' +  ISNULL(A.MiddleName, ' ') + ' [' + D.PrefixName + REPLACE(STR(A.StudentID, D.PrefixDigits), SPACE(1), '0') + ']'  AS StudentNameWithNo," +
                                //    "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' ' + REPLACE(STR(A.StudentID, D.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                //    "E.StudentType, F.SchClass, G.ClassCode, (CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END) + ' ' + H.CATName AS ClassName, " +
                                //    "H.CATName, I.Gender, I.GenderAbrv, J.Discipline, K.StatusType, L.PayeeType, Q.ClubName, R.RoleName, S.Country, T.State, U.LGA, V.EDUInstitute, W.ExitType, " +
                                //    "CASE WHEN A.PayeeTypeID = 2 THEN M.ParentTitle + ' ' + M.ParentSurname + " +
                                //    "' [' + N.PrefixName + REPLACE(STR(M.PayeeID, N.PrefixDigits), SPACE(1), '0') + ']' ELSE " +
                                //    "O.Surname + ' ' + O.FirstName + ' ' + ISNULL(O.MiddleName, ' ') + " +
                                //    "' [' + P.PrefixName + REPLACE(STR(O.EmployeeID, P.PrefixDigits), SPACE(1), '0') + ']' END AS ParentName, " +
                                //    "ROW_NUMBER() Over (Partition By A.ClassListID, A.ClassID Order by A.Surname) AS SN " +
                                //    "FROM ADMStudents A " +
                                //    "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                //    "LEFT OUTER JOIN ADMSchlList AS C ON C.SchID = A.SchID " +
                                //    "LEFT OUTER JOIN SETPrefix D ON D.PrefixID = A.PrefixID " +
                                //    "LEFT OUTER JOIN ADMStudentType AS E ON E.StudentTypeID = A.StudentTypeID " +
                                //    "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = A.ClassListID " +
                                //    "LEFT OUTER JOIN ADMSchClassList AS G ON G.ClassID = A.ClassID " +
                                //    "LEFT OUTER JOIN ADMSchClassCategory AS H ON H.CATID = G.CATID " +
                                //    "LEFT OUTER JOIN SETGender AS I ON I.GenderID = A.GenderID " +
                                //    "LEFT OUTER JOIN ADMSchClassDiscipline AS J ON J.DisciplineID = A.DisciplineID " +
                                //    "LEFT OUTER JOIN SETStatusType AS K ON K.StatusTypeID = A.StatusTypeID " +
                                //    "LEFT OUTER JOIN SETPayeeType AS L ON L.PayeeTypeID = A.PayeeTypeID " +
                                //    "LEFT OUTER JOIN ADMSchParents M ON M.PayeeID = A.PayeeID " +
                                //    "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                //    "LEFT OUTER JOIN ADMEmployee O ON O.StaffID = A.StaffID " +
                                //    "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = O.PrefixID " +
                                //    "LEFT OUTER JOIN ADMSchClub Q ON Q.ClubID = A.ClubID " +
                                //    "LEFT OUTER JOIN ADMSchClubRole R ON R.RoleID = A.RoleID " +
                                //    "LEFT OUTER JOIN SETCountries S ON S.CountryID = A.CountryID " +
                                //    "LEFT OUTER JOIN SETStates T ON T.StateID = A.StateID " +
                                //    "LEFT OUTER JOIN SETLGA U ON U.LGAID = A.LGAID " +
                                //    "LEFT OUTER JOIN ADMSchEducationInstitute V ON V.EDUID = A.EDUID " +
                                //    "LEFT OUTER JOIN ADMStudentExit W ON W.ExitID = A.ExitID " +
                                //    "LEFT OUTER JOIN SETReligion X ON X.ReligionID = A.ReligionID " +
                                //    "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID ORDER BY A.ClassListID, A.ClassID, Surname;";

                            //sql = "SELECT A.STDID, B.PrefixName + REPLACE(STR(A.StudentID, B.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                            //    "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, A.PhoneNumber, " +
                            //    " C.School, G.StudentType, A.studentPhoto, A.Surname, A.FirstName, ISNULL(A.MiddleName, ' ') AS MiddleName,  " +
                            //    "(CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END) + ' ' + F.CATName AS ClassName, " +
                            //    "ROW_NUMBER() Over (Partition By A.ClassListID ORDER by A.ClassListID) AS SN " +
                            //    "FROM ADMStudents A " +
                            //    "INNER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                            //    "INNER JOIN ADMSchlList AS C ON C.SchID = A.SchID " +
                            //    "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = A.ClassListID " +
                            //    "INNER JOIN ADMSchClassList AS E ON E.ClassID = A.ClassID " +
                            //    "INNER JOIN ADMSchClassCategory AS F ON F.CATID = E.CATID " +
                            //    "INNER JOIN ADMStudentType AS G ON G.StudentTypeID = A.StudentTypeID " +
                            //    "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID;";
                            sql = "SELECT A.STDID, B.PrefixName + REPLACE(STR(A.StudentID, B.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, A.PhoneNumber, " +
                                " C.School, G.StudentType, A.studentPhoto, A.Surname, A.FirstName, ISNULL(A.MiddleName, ' ') AS MiddleName,  " +
                                "(CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END) + ' ' + F.CATName AS ClassName, " +
                                "LEFT(H.SchSession, 4) + '/' + RIGHT(H.SchSession, 4) + ' - ' + H.SchTerm  + ' Term' AS AcademicSession, " +
                                "I.StatusType, A.StudentID, A.Email, " +
                                "ROW_NUMBER() Over (Partition By A.ClassListID ORDER by A.ClassListID) AS SN " +
                                "FROM ADMStudents A " +
                                "INNER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                                "INNER JOIN ADMSchlList AS C ON C.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = A.ClassListID " +
                                "INNER JOIN ADMSchClassList AS E ON E.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassCategory AS F ON F.CATID = E.CATID " +
                                "INNER JOIN ADMStudentType AS G ON G.StudentTypeID = A.StudentTypeID " +
                                "INNER JOIN SETSchSessions H ON H.TermID = A.TermID " +
                                "INNER JOIN SETStatusType AS I ON I.StatusTypeID = A.StatusTypeID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID;";

                            students = (List<ADMStudents>)await connection.QueryAsync<ADMStudents>(sql,
                                    new
                                    {
                                        _switch.StatusTypeID
                                    });
                            break;
                        case 2: //Filter By School And StatusType
                            sql = "SELECT A.STDID, B.PrefixName + REPLACE(STR(A.StudentID, B.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                 "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, A.PhoneNumber, " +
                                 " C.School, G.StudentType, A.studentPhoto, A.Surname, A.FirstName, ISNULL(A.MiddleName, ' ') AS MiddleName,  " +
                                 "(CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END) + ' ' + F.CATName AS ClassName, " +
                                 "A.StudentID, A.Email, " +
                                 "ROW_NUMBER() Over (Partition By A.ClassListID ORDER by A.ClassListID) AS SN " +
                                 "FROM ADMStudents A " +
                                 "INNER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                                 "INNER JOIN ADMSchlList AS C ON C.SchID = A.SchID " +
                                 "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = A.ClassListID " +
                                 "INNER JOIN ADMSchClassList AS E ON E.ClassID = A.ClassID " +
                                 "INNER JOIN ADMSchClassCategory AS F ON F.CATID = E.CATID " +
                                 "INNER JOIN ADMStudentType AS G ON G.StudentTypeID = A.StudentTypeID " +
                                 "WHERE A.DeleteName = 'False' AND A.SchID = @SchID AND A.StatusTypeID = @StatusTypeID;";

                            students = (List<ADMStudents>)await connection.QueryAsync<ADMStudents>(sql,
                                    new
                                    {
                                        _switch.SchID,
                                        _switch.StatusTypeID
                                    });
                            break;
                        case 3: //Filter By Class And StatusType
                            sql = "SELECT A.STDID, B.PrefixName + REPLACE(STR(A.StudentID, B.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, A.PhoneNumber, " +
                                " C.School, G.StudentType, A.studentPhoto, A.Surname, A.FirstName, ISNULL(A.MiddleName, ' ') AS MiddleName,  " +
                                "(CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END) + ' ' + F.CATName AS ClassName, " +
                                "LEFT(H.SchSession, 4) + '/' + RIGHT(H.SchSession, 4) + ' - ' + H.SchTerm  + ' Term' AS AcademicSession, " +
                                "I.StatusType, A.StudentID, A.Email, A.ParentPin, A.StudentPin, A.Password, A.CBTLock, " +
                                "A.ParentPinCount, A.ParentPinLock, " +
                                "A.Surname + ' ' + A.FirstName + ' ' +  ISNULL(A.MiddleName, ' ') + ' [' + B.PrefixName + REPLACE(STR(A.StudentID, B.PrefixDigits), SPACE(1), '0') + ']'  AS StudentNameWithNo, " +
                                "ROW_NUMBER() Over (Partition By A.ClassListID ORDER by A.ClassListID) AS SN " +
                                "FROM ADMStudents A " +
                                "INNER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                                "INNER JOIN ADMSchlList AS C ON C.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = A.ClassListID " +
                                "INNER JOIN ADMSchClassList AS E ON E.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassCategory AS F ON F.CATID = E.CATID " +
                                "INNER JOIN ADMStudentType AS G ON G.StudentTypeID = A.StudentTypeID " +
                                "INNER JOIN SETSchSessions H ON H.TermID = A.TermID " +
                                "INNER JOIN SETStatusType AS I ON I.StatusTypeID = A.StatusTypeID " +
                                "WHERE A.DeleteName = 'False' AND A.ClassID = @ClassID AND A.StatusTypeID = @StatusTypeID;";

                            students = (List<ADMStudents>)await connection.QueryAsync<ADMStudents>(sql,
                                    new
                                    {
                                        _switch.ClassID,
                                        _switch.StatusTypeID
                                    });
                            break;
                        case 4: //Filter By School, StudentType And StatusType
                            sql = "SELECT A.STDID, B.PrefixName + REPLACE(STR(A.StudentID, B.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, A.PhoneNumber, " +
                                " C.School, G.StudentType, A.studentPhoto, A.Surname, A.FirstName, ISNULL(A.MiddleName, ' ') AS MiddleName,  " +
                                "(CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END) + ' ' + F.CATName AS ClassName, " +
                                "A.StudentID, A.Email, " +
                                "ROW_NUMBER() Over (Partition By A.ClassListID ORDER by A.ClassListID) AS SN " +
                                "FROM ADMStudents A " +
                                "INNER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                                "INNER JOIN ADMSchlList AS C ON C.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = A.ClassListID " +
                                "INNER JOIN ADMSchClassList AS E ON E.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassCategory AS F ON F.CATID = E.CATID " +
                                "INNER JOIN ADMStudentType AS G ON G.StudentTypeID = A.StudentTypeID " +
                                "WHERE A.DeleteName = 'False' AND A.SchID = @SchID AND A.StudentTypeID = @StudentTypeID " +
                                "AND A.StatusTypeID = @StatusTypeID;";

                            students = (List<ADMStudents>)await connection.QueryAsync<ADMStudents>(sql,
                                   new
                                   {
                                       _switch.SchID,
                                       _switch.StudentTypeID,
                                       _switch.StatusTypeID
                                   });
                            break;
                        case 5: //Filter By Class, StudentType And StatusType
                            sql = "SELECT A.STDID, B.PrefixName + REPLACE(STR(A.StudentID, B.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, A.PhoneNumber, " +
                                " C.School, G.StudentType, A.studentPhoto, A.Surname, A.FirstName, ISNULL(A.MiddleName, ' ') AS MiddleName,  " +
                                "(CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END) + ' ' + F.CATName AS ClassName, " +
                                "A.StudentID, A.Email, " +
                                "ROW_NUMBER() Over (Partition By A.ClassListID ORDER by A.ClassListID) AS SN " +
                                "FROM ADMStudents A " +
                                "INNER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                                "INNER JOIN ADMSchlList AS C ON C.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = A.ClassListID " +
                                "INNER JOIN ADMSchClassList AS E ON E.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassCategory AS F ON F.CATID = E.CATID " +
                                "INNER JOIN ADMStudentType AS G ON G.StudentTypeID = A.StudentTypeID " +
                                "WHERE A.DeleteName = 'False' AND A.ClassID = @ClassID AND A.StudentTypeID = @StudentTypeID " +
                                "AND A.StatusTypeID = @StatusTypeID;";

                            students = (List<ADMStudents>)await connection.QueryAsync<ADMStudents>(sql,
                                   new
                                   {
                                       _switch.ClassID,
                                       _switch.StudentTypeID,
                                       _switch.StatusTypeID
                                   });
                            break;
                        case 6: //Filter By Student Type And Status Type
                            sql = "SELECT A.STDID, B.PrefixName + REPLACE(STR(A.StudentID, B.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, A.PhoneNumber, " +
                                " C.School, G.StudentType, A.studentPhoto, A.Surname, A.FirstName, ISNULL(A.MiddleName, ' ') AS MiddleName,  " +
                                "(CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END) + ' ' + F.CATName AS ClassName, " +
                                "A.StudentID, A.Email, " +
                                "ROW_NUMBER() Over (Partition By A.ClassListID ORDER by A.ClassListID) AS SN " +
                                "FROM ADMStudents A " +
                                "INNER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                                "INNER JOIN ADMSchlList AS C ON C.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = A.ClassListID " +
                                "INNER JOIN ADMSchClassList AS E ON E.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassCategory AS F ON F.CATID = E.CATID " +
                                "INNER JOIN ADMStudentType AS G ON G.StudentTypeID = A.StudentTypeID " +
                                "WHERE A.DeleteName = 'False' AND A.StudentTypeID = @StudentTypeID " +
                                "AND A.StatusTypeID = @StatusTypeID ORDER BY A.ClassListID, A.ClassID, Surname;";

                            students = (List<ADMStudents>)await connection.QueryAsync<ADMStudents>(sql,
                                   new
                                   {
                                       _switch.StudentTypeID,
                                       _switch.StatusTypeID
                                   });
                            break;
                        case 7: //Filter By Class - Both Active And InActive
                            sql = "SELECT A.STDID, B.PrefixName + REPLACE(STR(A.StudentID, B.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, A.PhoneNumber, " +
                                " C.School, G.StudentType, A.studentPhoto, A.Surname, A.FirstName, ISNULL(A.MiddleName, ' ') AS MiddleName,  " +
                                "(CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END) + ' ' + F.CATName AS ClassName, " +
                                "A.StudentID, A.Email, " +
                                "ROW_NUMBER() Over (Partition By A.ClassListID ORDER by A.ClassListID, A.Surname) AS SN " +
                                "FROM ADMStudents A " +
                                "INNER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                                "INNER JOIN ADMSchlList AS C ON C.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = A.ClassListID " +
                                "INNER JOIN ADMSchClassList AS E ON E.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassCategory AS F ON F.CATID = E.CATID " +
                                "INNER JOIN ADMStudentType AS G ON G.StudentTypeID = A.StudentTypeID " +
                                "WHERE A.DeleteName = 'False' AND A.ClassID = @ClassID ORDER BY A.Surname;";

                            students = (List<ADMStudents>)await connection.QueryAsync<ADMStudents>(sql,
                                   new
                                   {
                                       _switch.ClassID
                                   });
                            break;
                        case 8: //Filter By Class Group And StatusType
                            sql = "SELECT A.STDID, B.PrefixName + REPLACE(STR(A.StudentID, B.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, A.PhoneNumber, " +
                                " C.School, G.StudentType, A.studentPhoto, A.Surname, A.FirstName, ISNULL(A.MiddleName, ' ') AS MiddleName,  " +
                                "(CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END) + ' ' + F.CATName AS ClassName, " +
                                "A.StudentID, A.Email, " +
                                "ROW_NUMBER() Over (Partition By A.ClassListID ORDER by A.ClassListID) AS SN " +
                                "FROM ADMStudents A " +
                                "INNER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                                "INNER JOIN ADMSchlList AS C ON C.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = A.ClassListID " +
                                "INNER JOIN ADMSchClassList AS E ON E.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassCategory AS F ON F.CATID = E.CATID " +
                                "INNER JOIN ADMStudentType AS G ON G.StudentTypeID = A.StudentTypeID " +
                                "WHERE A.DeleteName = 'False' AND A.ClassListID = @ClassListID AND A.StatusTypeID = @StatusTypeID " +
                                "ORDER BY A.ClassListID, A.ClassID, Surname;";

                            students = (List<ADMStudents>)await connection.QueryAsync<ADMStudents>(sql,
                                    new
                                    {
                                        ClassListID = _switch.ClassID,
                                        _switch.StatusTypeID
                                    });
                            break;
                        case 9:  //Filter By All Students - Both Active And InActive
                            sql = "SELECT A.STDID, A.ClassID, A.Surname, A.FirstName, ISNULL(A.MiddleName, ' ') AS MiddleName, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, A.StudentID, " +
                                "B.PrefixName + REPLACE(STR(A.StudentID, B.PrefixDigits), SPACE(1), '0') AS AdmissionNo, A.StatusTypeID " +
                                "FROM ADMStudents A " +
                                "LEFT OUTER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                                "WHERE A.DeleteName = 'False';";

                            students = (List<ADMStudents>)await connection.QueryAsync<ADMStudents>(sql, new { });
                            break;
                        default:  //Filter By All Students - Both Active And InActive
                            sql = "SELECT A.STDID, B.PrefixName + REPLACE(STR(A.StudentID, B.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, A.PhoneNumber, " +
                                " C.School, G.StudentType, A.studentPhoto, A.Surname, A.FirstName, ISNULL(A.MiddleName, ' ') AS MiddleName,  " +
                                "(CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END) + ' ' + F.CATName AS ClassName, " +
                                "A.StudentID, A.Email, " +
                                "ROW_NUMBER() Over (Partition By A.ClassListID ORDER by A.ClassListID) AS SN " +
                                "FROM ADMStudents A " +
                                "INNER JOIN SETPrefix B ON B.PrefixID = A.PrefixID " +
                                "INNER JOIN ADMSchlList AS C ON C.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = A.ClassListID " +
                                "INNER JOIN ADMSchClassList AS E ON E.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassCategory AS F ON F.CATID = E.CATID " +
                                "INNER JOIN ADMStudentType AS G ON G.StudentTypeID = A.StudentTypeID " +
                                "WHERE A.DeleteName = 'False';";

                            students = (List<ADMStudents>)await connection.QueryAsync<ADMStudents>(sql, new { });
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

            return students;
        }

        public async Task<ADMStudents> GetByIdAsync(int id)
        {
            student = new ADMStudents();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.STDID, A.SchInfoID, A.TermID, A.SchID, A.ClassListID, A.ClassID, A.ClassTeacherID, A.DisciplineID, " +
                "A.PayeeTypeID, A.PayeeID, A.StaffID, A.CountryID, A.StateID, A.LGAID, A.EDUID, A.ReligionID, A.ClubID, A.RoleID," +
                "A.StudentTypeID, A.StatusTypeID, A.ExitID, A.ExitDate, A.GenderID, A.PrefixID, A.StudentID, A.Surname, A.FirstName, " +
                "ISNULL(A.MiddleName, ' ') AS MiddleName, A.Enrollment, A.EnrollmentDate, A.Email, A.DOB, A.Address, A.PhoneNumber, " +
                "A.AltPhoneNumber, A.Memo, A.DateCreated, A.ModifyDate, A.photoStatus, A.studentPhoto, A.Promoted, A.ExamNo, A.ExamNoNECO, " +
                "A.DeleteName, A.ClassPrevious, A.ClassIDPrevious, A.PayStatus, A.ParentPin, A.StudentPin, A.Password, A.CBTLock, " +
                "LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, C.School, " +
                "A.PrefixID, D.PrefixName, D.PrefixDigits, D.PrefixName + REPLACE(STR(A.StudentID, D.PrefixDigits), SPACE(1), '0') AS AdmissionNo,  " +
                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, X.Religion, " +
                "A.Surname + ' ' + A.FirstName + ' ' +  ISNULL(A.MiddleName, ' ') + ' [' + D.PrefixName + REPLACE(STR(A.StudentID, D.PrefixDigits), SPACE(1), '0') + ']'  AS StudentNameWithNo," +
                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' ' + REPLACE(STR(A.StudentID, D.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                "E.StudentType, F.SchClass, G.ClassCode, F.SchClass + ' ' + H.CATName AS ClassName, H.CATName, I.Gender, I.GenderAbrv, " +
                "J.Discipline, K.StatusType, L.PayeeType, Q.ClubName, R.RoleName, S.Country, T.State, U.LGA, V.EDUInstitute, W.ExitType, " +
                "CASE WHEN A.PayeeTypeID = 2 THEN M.ParentTitle + ' ' + M.ParentSurname + " +
                "' [' + N.PrefixName + REPLACE(STR(M.PayeeID, N.PrefixDigits), SPACE(1), '0') + ']' ELSE " +
                "O.Surname + ' ' + O.FirstName + ' ' + ISNULL(O.MiddleName, ' ') + " +
                "' [' + P.PrefixName + REPLACE(STR(O.EmployeeID, P.PrefixDigits), SPACE(1), '0') + ']' END AS ParentName, " +
                "ROW_NUMBER() Over (Partition By A.ClassListID, A.ClassID Order by A.Surname) AS SN, " +
                "ResultTermID, ParentPinCount, ParentPinLock " +
                "FROM ADMStudents A " +
                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                "LEFT OUTER JOIN ADMSchlList AS C ON C.SchID = A.SchID " +
                "LEFT OUTER JOIN SETPrefix D ON D.PrefixID = A.PrefixID " +
                "LEFT OUTER JOIN ADMStudentType AS E ON E.StudentTypeID = A.StudentTypeID " +
                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = A.ClassListID " +
                "LEFT OUTER JOIN ADMSchClassList AS G ON G.ClassID = A.ClassID " +
                "LEFT OUTER JOIN ADMSchClassCategory AS H ON H.CATID = G.CATID " +
                "LEFT OUTER JOIN SETGender AS I ON I.GenderID = A.GenderID " +
                "LEFT OUTER JOIN ADMSchClassDiscipline AS J ON J.DisciplineID = A.DisciplineID " +
                "LEFT OUTER JOIN SETStatusType AS K ON K.StatusTypeID = A.StatusTypeID " +
                "LEFT OUTER JOIN SETPayeeType AS L ON L.PayeeTypeID = A.PayeeTypeID " +
                "LEFT OUTER JOIN ADMSchParents M ON M.PayeeID = A.PayeeID " +
                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                "LEFT OUTER JOIN ADMEmployee O ON O.StaffID = A.StaffID " +
                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = O.PrefixID " +
                "LEFT OUTER JOIN ADMSchClub Q ON Q.ClubID = A.ClubID " +
                "LEFT OUTER JOIN ADMSchClubRole R ON R.RoleID = A.RoleID " +
                "LEFT OUTER JOIN SETCountries S ON S.CountryID = A.CountryID " +
                "LEFT OUTER JOIN SETStates T ON T.StateID = A.StateID " +
                "LEFT OUTER JOIN SETLGA U ON U.LGAID = A.LGAID " +
                "LEFT OUTER JOIN ADMSchEducationInstitute V ON V.EDUID = A.EDUID " +
                "LEFT OUTER JOIN ADMStudentExit W ON W.ExitID = A.ExitID " +
                "LEFT OUTER JOIN SETReligion X ON X.ReligionID = A.ReligionID " +
                "WHERE A.STDID = @STDID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                student = await connection.QuerySingleOrDefaultAsync<ADMStudents>(sql, new { STDID = id });
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

            return student;
        }

        public async Task<ADMStudents> AddAsync(ADMStudents entity)
        {
            entity.DateCreated = DateTime.Now;
            entity.ModifyDate = DateTime.Now;
            entity.ParentPinCount = 0;
            entity.ParentPinLock = false;

            sql = @"INSERT INTO ADMStudents (SchInfoID, TermID, SchID, ClassListID, ClassID, ClassTeacherID, DisciplineID, PayeeTypeID, PayeeID, StaffID, 
                CountryID, StateID, LGAID, EDUID, ReligionID, ClubID, RoleID, StudentTypeID, GenderID, StudentID, Surname, FirstName, 
                MiddleName, Email, DOB, Address, PhoneNumber, AltPhoneNumber, Memo, photoStatus, studentPhoto, ExamNo, ExamNoNECO) 
                OUTPUT INSERTED.STDID  VALUES (@SchInfoID, @TermID, @SchID, @ClassListID, @ClassID, @ClassTeacherID, @DisciplineID, @PayeeTypeID, @PayeeID, 
                @StaffID, @CountryID, @StateID, @LGAID, @EDUID, @ReligionID, @ClubID, @RoleID, @StudentTypeID, @GenderID, @StudentID, 
                @Surname, @FirstName, @MiddleName, @Email, @DOB, @Address, @PhoneNumber, @AltPhoneNumber, @Memo, @photoStatus, 
                @studentPhoto, @ExamNo, @ExamNoNECO);SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.STDID = result;
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

        public async Task<ADMStudents> UpdateAsync(int id, ADMStudents entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            entity.ModifyDate = DateTime.Now;
            
            switch (id)
            {
                case 1: //Update Student Info
                    sql = "UPDATE ADMStudents SET SchInfoID = @SchInfoID, TermID = @TermID, SchID = @SchID, ClassListID = @ClassListID, ClassID = @ClassID, " +
                            "ClassTeacherID = @ClassTeacherID, DisciplineID = @DisciplineID, PayeeTypeID = @PayeeTypeID, PayeeID = @PayeeID, " +
                            "StaffID = @StaffID, CountryID = @CountryID, StateID = @StateID, LGAID = @LGAID, EDUID = @EDUID, ReligionID = @ReligionID, " +
                            "ClubID = @ClubID, RoleID = @RoleID, StudentTypeID = @StudentTypeID, StatusTypeID = @StatusTypeID, ExitID = @ExitID, " +
                            "ExitDate = @ExitDate, GenderID = @GenderID, StudentID = @StudentID, Surname = @Surname, FirstName = @FirstName, " +
                            "MiddleName = @MiddleName, Email = @Email, DOB = @DOB, Address = @Address, PhoneNumber = @PhoneNumber, " +
                            "AltPhoneNumber = @AltPhoneNumber, Memo = @Memo, photoStatus = @photoStatus, studentPhoto = @studentPhoto, ExamNo = @ExamNo, " +
                            "ExamNoNECO = @ExamNoNECO WHERE STDID = @STDID;";
                    break;
                case 2: //Delete Student
                    sql = "UPDATE ADMStudents SET DeleteName = @DeleteName WHERE STDID = @STDID;";
                    break;
                case 3: //Update Student Class
                    sql = "UPDATE ADMStudents SET SchID = @SchID, ClassListID = @ClassListID, ClassID = @ClassID, TermID = @TermID, " +
                            "StatusTypeID = @StatusTypeID WHERE STDID = @STDID;";
                    break;
                case 4: //Batch Update of Student Info
                    sql = "UPDATE ADMStudents SET StudentID = @StudentID, Surname = @Surname, FirstName = @FirstName, MiddleName = @MiddleName, " +
                            "Email = @Email, PhoneNumber = @PhoneNumber WHERE STDID = @STDID;";
                    break;  
                case 5: //Update Student Photo
                    sql = "UPDATE ADMStudents SET  SchID = @SchID, ClassListID = @ClassListID, ClassID = @ClassID, photoStatus = @photoStatus, " +
                            "studentPhoto = @studentPhoto WHERE STDID = @STDID;";
                    break;               
                case 6: //Generate Student PIN
                    sql = "UPDATE ADMStudents SET StudentPin = @StudentPin WHERE STDID = @STDID;";
                    break;
                case 7: //Set Student CBT Password
                    entity.Password = Utilities.Encrypt(entity.Password);
                    sql = "UPDATE ADMStudents SET Password = @Password WHERE STDID = @STDID;";
                    break;
                case 8: //Set Both PIN and CBT Password
                    entity.Password = Utilities.Encrypt(entity.Password);
                    sql = "UPDATE ADMStudents SET StudentPin = @StudentPin, Password = @Password WHERE STDID = @STDID;";
                    break;
                case 9: //CBT Lock Only
                    sql = "UPDATE ADMStudents SET CBTLock = @CBTLock WHERE STDID = @STDID;";
                    break;
                case 10: //Generate Parent PIN
                    entity.ParentPinCount = 0;
                    entity.ParentPinLock = false;
                    sql = "UPDATE ADMStudents SET ParentPin = @ParentPin, ParentPinCount = @ParentPinCount, " +
                        "ParentPinLock = @ParentPinLock WHERE STDID = @STDID;";
                    break;
                case 11: //Update Student Authentication Token
                    sql = "UPDATE ADMStudents SET RefreshToken = @RefreshToken, RefreshTokenExpiryTime = @RefreshTokenExpiryTime WHERE STDID = @STDID;";
                    break;
                case 12: //Update Student Refresh Token
                    sql = "UPDATE ADMStudents SET RefreshToken = @RefreshToken WHERE STDID = @STDID;";
                    break;
                case 13: //Update Student PIN, Parent PIN and CBT Lock
                    sql = "UPDATE ADMStudents SET StudentPin = @StudentPin, ParentPin = @ParentPin, CBTLock =@CBTLock WHERE STDID = @STDID;";
                    break;
                case 14: //Update Parent PIN Counter                   
                    sql = "UPDATE ADMStudents SET ParentPinCount = @ParentPinCount WHERE STDID = @STDID;";
                    break;
                case 15: //Lock Parent PIN
                    entity.ParentPinLock = true;
                    sql = "UPDATE ADMStudents SET ParentPinLock = @ParentPinLock WHERE STDID = @STDID;";
                    break;
                case 16: //Set Result Type For Parents
                    sql = "UPDATE ADMStudents SET ResultTypeID = @ResultTypeID, ResultTermID = @ResultTermID " +
                        "WHERE STDID = @STDID;";
                    break;
                case 17:
                    sql = "UPDATE ADMStudents SET Id = @Id WHERE STDID = @STDID;";
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
            sql = "DELETE FROM ADMStudents WHERE STDID = @STDID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { STDID = id });
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

        public async Task<int> CountAsync(SwitchModel _switch)
        {
            int count = 0;
            
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                switch (_switch.SwitchID)
                {
                    case 1: //Student Count By Class
                        sql = "SELECT COUNT(*) FROM ADMStudents WHERE DeleteName = 'False' AND StatusTypeID = @StatusTypeID " +
                            "AND ClassID = @ClassID";
                        count = await connection.ExecuteScalarAsync<int>(sql, 
                            new 
                            {
                                _switch.StatusTypeID,
                                _switch.ClassID
                            });
                        break;
                    case 2: //Student Count By School
                        sql = "SELECT COUNT(*) FROM ADMStudents WHERE DeleteName = 'False' AND StatusTypeID = @StatusTypeID " +
                            "AND SchID = @SchID";
                        count = await connection.ExecuteScalarAsync<int>(sql,
                            new
                            {
                                _switch.StatusTypeID,
                                _switch.SchID
                            });
                        break;
                    case 3:
                        sql = "SELECT COUNT(*) FROM ADMStudents WHERE DeleteName = 'False' AND StatusTypeID = @StatusTypeID " +
                            "AND ClassListID = @ClassListID";
                        count = await connection.ExecuteScalarAsync<int>(sql,
                            new
                            {
                                _switch.StatusTypeID,
                                ClassListID = _switch.SchID
                            });
                        break;
                    case 4: //Student Count By Status Type
                        sql = "SELECT COUNT(*) FROM ADMStudents WHERE DeleteName = 'False' AND StatusTypeID = @StatusTypeID";
                        count = await connection.ExecuteScalarAsync<int>(sql,
                            new
                            {
                                _switch.StatusTypeID
                            });
                        break;
                    case 5: //Student Count By Student Type
                        sql = "SELECT COUNT(*) FROM ADMStudents WHERE DeleteName = 'False' AND StatusTypeID = @StatusTypeID " +
                            "AND ClassID = @ClassID AND StudentTypeID = @StudentTypeID";
                        count = await connection.ExecuteScalarAsync<int>(sql,
                            new
                            {
                                _switch.StatusTypeID,
                                _switch.ClassID,
                                _switch.StudentTypeID
                            });
                        break;
                    case 6: //Student Count By School and Student Type
                        sql = "SELECT COUNT(*) FROM ADMStudents WHERE DeleteName = 'False' AND StatusTypeID = @StatusTypeID " +
                            "AND SchID = @SchID AND StudentTypeID = @StudentTypeID";
                        count = await connection.ExecuteScalarAsync<int>(sql,
                            new
                            {
                                _switch.StatusTypeID,
                                _switch.SchID,
                                _switch.StudentTypeID
                            });
                        break;
                    case 7: //Student Count By Student Type
                        sql = "SELECT COUNT(*) FROM ADMStudents WHERE DeleteName = 'False' AND StatusTypeID = @StatusTypeID " +
                            "AND StudentTypeID = @StudentTypeID";
                        count = await connection.ExecuteScalarAsync<int>(sql,
                            new
                            {
                                _switch.StatusTypeID,
                                _switch.StudentTypeID
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

            return count;
        }

        public async Task<IReadOnlyList<ADMStudents>> SearchAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: //Search By Student PIN
                            sql = "SELECT A.STDID, A.SchInfoID, A.TermID, A.SchID, A.ClassListID, A.ClassID, A.ClassTeacherID, A.DisciplineID, " +
                                "A.PayeeTypeID, A.PayeeID, A.StaffID, A.CountryID, A.StateID, A.LGAID, A.EDUID, A.ReligionID, A.ClubID, A.RoleID," +
                                "A.StudentTypeID, A.StatusTypeID, A.ExitID, A.ExitDate, A.GenderID, A.PrefixID, A.StudentID, A.Surname, A.FirstName, " +
                                "ISNULL(A.MiddleName, ' ') AS MiddleName, A.Enrollment, A.EnrollmentDate, A.Email, A.DOB, A.Address, A.PhoneNumber, " +
                                "A.AltPhoneNumber, A.Memo, A.DateCreated, A.ModifyDate, A.photoStatus, A.studentPhoto, A.Promoted, A.ExamNo, A.ExamNoNECO, " +
                                "A.DeleteName, A.ClassPrevious, A.ClassIDPrevious, A.PayStatus, A.ParentPin, A.StudentPin, A.Password, A.CBTLock, " +
                                "LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, C.School, " +
                                "A.PrefixID, D.PrefixName, D.PrefixDigits, D.PrefixName + REPLACE(STR(A.StudentID, D.PrefixDigits), SPACE(1), '0') AS AdmissionNo,  " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, X.Religion, " +
                                "A.Surname + ' ' + A.FirstName + ' ' +  ISNULL(A.MiddleName, ' ') + ' [' + D.PrefixName + REPLACE(STR(A.StudentID, D.PrefixDigits), SPACE(1), '0') + ']'  AS StudentNameWithNo," +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' ' + REPLACE(STR(A.StudentID, D.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "E.StudentType, F.SchClass, G.ClassCode, (CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END) + ' ' + H.CATName AS ClassName, " +
                                "H.CATName, I.Gender, I.GenderAbrv, J.Discipline, K.StatusType, L.PayeeType, Q.ClubName, R.RoleName, S.Country, T.State, U.LGA, V.EDUInstitute, W.ExitType, " +
                                "CASE WHEN A.PayeeTypeID = 2 THEN M.ParentTitle + ' ' + M.ParentSurname + " +
                                "' [' + N.PrefixName + REPLACE(STR(M.PayeeID, N.PrefixDigits), SPACE(1), '0') + ']' ELSE " +
                                "O.Surname + ' ' + O.FirstName + ' ' + ISNULL(O.MiddleName, ' ') + " +
                                "' [' + P.PrefixName + REPLACE(STR(O.EmployeeID, P.PrefixDigits), SPACE(1), '0') + ']' END AS ParentName, " +
                                "ROW_NUMBER() Over (Partition By A.ClassListID, A.ClassID Order by A.Surname) AS SN, " +
                                "ResultTermID, ParentPinCount, ParentPinLock " +
                                "FROM ADMStudents A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList AS C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN SETPrefix D ON D.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMStudentType AS E ON E.StudentTypeID = A.StudentTypeID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassList AS G ON G.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory AS H ON H.CATID = G.CATID " +
                                "LEFT OUTER JOIN SETGender AS I ON I.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassDiscipline AS J ON J.DisciplineID = A.DisciplineID " +
                                "LEFT OUTER JOIN SETStatusType AS K ON K.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN SETPayeeType AS L ON L.PayeeTypeID = A.PayeeTypeID " +
                                "LEFT OUTER JOIN ADMSchParents M ON M.PayeeID = A.PayeeID " +
                                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee O ON O.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = O.PrefixID " +
                                "LEFT JOIN ADMSchClub Q ON Q.ClubID = A.ClubID " +
                                "LEFT JOIN ADMSchClubRole R ON R.RoleID = A.RoleID " +
                                "LEFT JOIN SETCountries S ON S.CountryID = A.CountryID " +
                                "LEFT JOIN SETStates T ON T.StateID = A.StateID " +
                                "LEFT JOIN SETLGA U ON U.LGAID = A.LGAID " +
                                "LEFT JOIN ADMSchEducationInstitute V ON V.EDUID = A.EDUID " +
                                "LEFT JOIN ADMStudentExit W ON W.ExitID = A.ExitID " +
                                "LEFT JOIN SETReligion X ON X.ReligionID = A.ReligionID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID AND A.StudentPin = @StudentPin " +
                                "ORDER BY A.ClassListID, A.ClassID, Surname;";

                            students = (List<ADMStudents>)await connection.QueryAsync<ADMStudents>(sql,
                                    new
                                    {
                                        StatusTypeID = _switch.StatusTypeID,
                                        StudentPin = _switch.SearchCriteriaA
                                    });
                            break;
                        case 2: //Search By Student PIN And Password
                            sql = "SELECT A.STDID, A.SchInfoID, A.TermID, A.SchID, A.ClassListID, A.ClassID, A.ClassTeacherID, A.DisciplineID, " +
                                "A.PayeeTypeID, A.PayeeID, A.StaffID, A.CountryID, A.StateID, A.LGAID, A.EDUID, A.ReligionID, A.ClubID, A.RoleID," +
                                "A.StudentTypeID, A.StatusTypeID, A.ExitID, A.ExitDate, A.GenderID, A.PrefixID, A.StudentID, A.Surname, A.FirstName, " +
                                "ISNULL(A.MiddleName, ' ') AS MiddleName, A.Enrollment, A.EnrollmentDate, A.Email, A.DOB, A.Address, A.PhoneNumber, " +
                                "A.AltPhoneNumber, A.Memo, A.DateCreated, A.ModifyDate, A.photoStatus, A.studentPhoto, A.Promoted, A.ExamNo, A.ExamNoNECO, " +
                                "A.DeleteName, A.ClassPrevious, A.ClassIDPrevious, A.PayStatus, A.ParentPin, A.StudentPin, A.Password, A.CBTLock, " +
                                "LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, C.School, " +
                                "A.PrefixID, D.PrefixName, D.PrefixDigits, D.PrefixName + REPLACE(STR(A.StudentID, D.PrefixDigits), SPACE(1), '0') AS AdmissionNo,  " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, X.Religion, " +
                                "A.Surname + ' ' + A.FirstName + ' ' +  ISNULL(A.MiddleName, ' ') + ' [' + D.PrefixName + REPLACE(STR(A.StudentID, D.PrefixDigits), SPACE(1), '0') + ']'  AS StudentNameWithNo," +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' ' + REPLACE(STR(A.StudentID, D.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "E.StudentType, F.SchClass, G.ClassCode, (CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END) + ' ' + H.CATName AS ClassName, " +
                                "H.CATName, I.Gender, I.GenderAbrv, J.Discipline, K.StatusType, L.PayeeType, Q.ClubName, R.RoleName, S.Country, T.State, U.LGA, V.EDUInstitute, W.ExitType, " +
                                "CASE WHEN A.PayeeTypeID = 2 THEN M.ParentTitle + ' ' + M.ParentSurname + " +
                                "' [' + N.PrefixName + REPLACE(STR(M.PayeeID, N.PrefixDigits), SPACE(1), '0') + ']' ELSE " +
                                "O.Surname + ' ' + O.FirstName + ' ' + ISNULL(O.MiddleName, ' ') + " +
                                "' [' + P.PrefixName + REPLACE(STR(O.EmployeeID, P.PrefixDigits), SPACE(1), '0') + ']' END AS ParentName, " +
                                "ROW_NUMBER() Over (Partition By A.ClassListID, A.ClassID Order by A.Surname) AS SN, " +
                                "ResultTermID, ParentPinCount, ParentPinLock " +
                                "FROM ADMStudents A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList AS C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN SETPrefix D ON D.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMStudentType AS E ON E.StudentTypeID = A.StudentTypeID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassList AS G ON G.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory AS H ON H.CATID = G.CATID " +
                                "LEFT OUTER JOIN SETGender AS I ON I.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassDiscipline AS J ON J.DisciplineID = A.DisciplineID " +
                                "LEFT OUTER JOIN SETStatusType AS K ON K.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN SETPayeeType AS L ON L.PayeeTypeID = A.PayeeTypeID " +
                                "LEFT OUTER JOIN ADMSchParents M ON M.PayeeID = A.PayeeID " +
                                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee O ON O.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = O.PrefixID " +
                                "LEFT JOIN ADMSchClub Q ON Q.ClubID = A.ClubID " +
                                "LEFT JOIN ADMSchClubRole R ON R.RoleID = A.RoleID " +
                                "LEFT JOIN SETCountries S ON S.CountryID = A.CountryID " +
                                "LEFT JOIN SETStates T ON T.StateID = A.StateID " +
                                "LEFT JOIN SETLGA U ON U.LGAID = A.LGAID " +
                                "LEFT JOIN ADMSchEducationInstitute V ON V.EDUID = A.EDUID " +
                                "LEFT JOIN ADMStudentExit W ON W.ExitID = A.ExitID " +
                                "LEFT JOIN SETReligion X ON X.ReligionID = A.ReligionID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID AND A.StudentPin = @StudentPin " +
                                "AND A.Password = @Password ORDER BY A.ClassListID, A.ClassID, Surname;";

                            students = (List<ADMStudents>)await connection.QueryAsync<ADMStudents>(sql,
                                    new
                                    {
                                        StatusTypeID = _switch.StatusTypeID,
                                        StudentPin = _switch.SearchCriteriaA,
                                        Password = _switch.SearchCriteriaB
                                    });
                            break;
                        case 3: //Search By Parent PIN
                            sql = "SELECT A.STDID, A.SchInfoID, A.TermID, A.SchID, A.ClassListID, A.ClassID, A.ClassTeacherID, A.DisciplineID, " +
                                "A.PayeeTypeID, A.PayeeID, A.StaffID, A.CountryID, A.StateID, A.LGAID, A.EDUID, A.ReligionID, A.ClubID, A.RoleID," +
                                "A.StudentTypeID, A.StatusTypeID, A.ExitID, A.ExitDate, A.GenderID, A.PrefixID, A.StudentID, A.Surname, A.FirstName, " +
                                "ISNULL(A.MiddleName, ' ') AS MiddleName, A.Enrollment, A.EnrollmentDate, A.Email, A.DOB, A.Address, A.PhoneNumber, " +
                                "A.AltPhoneNumber, A.Memo, A.DateCreated, A.ModifyDate, A.photoStatus, A.studentPhoto, A.Promoted, A.ExamNo, A.ExamNoNECO, " +
                                "A.DeleteName, A.ClassPrevious, A.ClassIDPrevious, A.PayStatus, A.ParentPin, A.StudentPin, A.Password, A.CBTLock, " +
                                "LEFT(B.SchSession, 4) + '/' + RIGHT(B.SchSession, 4) + ' - ' + B.SchTerm  + ' Term' AS AcademicSession, C.School, " +
                                "A.PrefixID, D.PrefixName, D.PrefixDigits, D.PrefixName + REPLACE(STR(A.StudentID, D.PrefixDigits), SPACE(1), '0') AS AdmissionNo,  " +
                                "A.Surname + ' ' + A.FirstName + ' ' + ISNULL(A.MiddleName, ' ') AS StudentName, X.Religion, " +
                                "A.Surname + ' ' + A.FirstName + ' ' +  ISNULL(A.MiddleName, ' ') + ' [' + D.PrefixName + REPLACE(STR(A.StudentID, D.PrefixDigits), SPACE(1), '0') + ']'  AS StudentNameWithNo," +
                                "A.Surname + ' ' + SUBSTRING(A.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(A.MiddleName, 0, 2) + '. ', '') + ' ' + REPLACE(STR(A.StudentID, D.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "E.StudentType, F.SchClass, G.ClassCode, (CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END) + ' ' + H.CATName AS ClassName, " +
                                "H.CATName, I.Gender, I.GenderAbrv, J.Discipline, K.StatusType, L.PayeeType, Q.ClubName, R.RoleName, S.Country, T.State, U.LGA, V.EDUInstitute, W.ExitType, " +
                                "CASE WHEN A.PayeeTypeID = 2 THEN M.ParentTitle + ' ' + M.ParentSurname + " +
                                "' [' + N.PrefixName + REPLACE(STR(M.PayeeID, N.PrefixDigits), SPACE(1), '0') + ']' ELSE " +
                                "O.Surname + ' ' + O.FirstName + ' ' + ISNULL(O.MiddleName, ' ') + " +
                                "' [' + P.PrefixName + REPLACE(STR(O.EmployeeID, P.PrefixDigits), SPACE(1), '0') + ']' END AS ParentName, " +
                                "ROW_NUMBER() Over (Partition By A.ClassListID, A.ClassID Order by A.Surname) AS SN, " +
                                "ResultTermID, ParentPinCount, ParentPinLock " +
                                "FROM ADMStudents A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList AS C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN SETPrefix D ON D.PrefixID = A.PrefixID " +
                                "LEFT OUTER JOIN ADMStudentType AS E ON E.StudentTypeID = A.StudentTypeID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassList AS G ON G.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory AS H ON H.CATID = G.CATID " +
                                "LEFT OUTER JOIN SETGender AS I ON I.GenderID = A.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassDiscipline AS J ON J.DisciplineID = A.DisciplineID " +
                                "LEFT OUTER JOIN SETStatusType AS K ON K.StatusTypeID = A.StatusTypeID " +
                                "LEFT OUTER JOIN SETPayeeType AS L ON L.PayeeTypeID = A.PayeeTypeID " +
                                "LEFT OUTER JOIN ADMSchParents M ON M.PayeeID = A.PayeeID " +
                                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee O ON O.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = O.PrefixID " +
                                "LEFT JOIN ADMSchClub Q ON Q.ClubID = A.ClubID " +
                                "LEFT JOIN ADMSchClubRole R ON R.RoleID = A.RoleID " +
                                "LEFT JOIN SETCountries S ON S.CountryID = A.CountryID " +
                                "LEFT JOIN SETStates T ON T.StateID = A.StateID " +
                                "LEFT JOIN SETLGA U ON U.LGAID = A.LGAID " +
                                "LEFT JOIN ADMSchEducationInstitute V ON V.EDUID = A.EDUID " +
                                "LEFT JOIN ADMStudentExit W ON W.ExitID = A.ExitID " +
                                "LEFT JOIN SETReligion X ON X.ReligionID = A.ReligionID " +
                                "WHERE A.DeleteName = 'False' AND A.StatusTypeID = @StatusTypeID AND A.ParentPin = @ParentPin " +
                                "ORDER BY A.ClassListID, A.ClassID, Surname;";

                            students = (List<ADMStudents>)await connection.QueryAsync<ADMStudents>(sql,
                                    new
                                    {
                                        StatusTypeID = _switch.StatusTypeID,
                                        ParentPin = _switch.SearchCriteriaA
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

            return students;
        }
    }

    public class ADMStudentTypeRepository : IADMStudentTypeRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMStudentType> _list { get; set; }
        ADMStudentType _details = new();

        public ADMStudentTypeRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ADMStudentType>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ADMStudentType;";

                            _list = (List<ADMStudentType>)await connection.QueryAsync<ADMStudentType>(sql, new { });
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

        public Task<ADMStudentType> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ADMStudentType> AddAsync(ADMStudentType entity)
        {
            throw new NotImplementedException();
        }

        public Task<ADMStudentType> UpdateAsync(int id, ADMStudentType entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(SwitchModel _switchid)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ADMStudentType>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ADMStudentMEDHistoryRepository : IADMStudentMEDHistoryRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMStudentMEDHistory> _list { get; set; }
        ADMStudentMEDHistory _details = new();

        public ADMStudentMEDHistoryRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ADMStudentMEDHistory>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT A.MEDHistoryID, A.SchInfoID, A.STDID, A.MEDID, A.MEDValue, A.MEDTextValue, B.MEDName " +
                                "FROM ADMStudentMEDHistory AS A " +
                                "INNER JOIN SETMedical AS B ON A.MEDID = B.MEDID " +
                                "WHERE A.STDID = @STDID;";

                            _list = (List<ADMStudentMEDHistory>)await connection.QueryAsync<ADMStudentMEDHistory>(sql, 
                                new 
                                {
                                    STDID = _switch.STDID
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

            return _list;
        }

        public async Task<ADMStudentMEDHistory> GetByIdAsync(int id)
        {
            _details = new ADMStudentMEDHistory();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.MEDHistoryID, A.SchInfoID, A.STDID, A.MEDID, A.MEDValue, A.MEDTextValue, B.MEDName " +
                "FROM ADMStudentMEDHistory AS A " +
                "INNER JOIN SETMedical AS B ON A.MEDID = B.MEDID " +
                "WHERE A.MEDHistoryID = @MEDHistoryID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ADMStudentMEDHistory>(sql, new { MEDHistoryID = id });
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

        public async Task<ADMStudentMEDHistory> AddAsync(ADMStudentMEDHistory entity)
        {
            sql = @"INSERT INTO ADMStudentMEDHistory (SchInfoID, STDID, MEDID, MEDValue, MEDTextValue) OUTPUT INSERTED.MEDHistoryID
                    VALUES (@SchInfoID, @STDID, @MEDID, @MEDValue, @MEDTextValue)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.MEDHistoryID = result;
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

        public async Task<ADMStudentMEDHistory> UpdateAsync(int id, ADMStudentMEDHistory entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMStudentMEDHistory SET MEDValue = @MEDValue, MEDTextValue = @MEDTextValue WHERE MEDHistoryID = @MEDHistoryID;";
                    break;
                case 2:
                    sql = "UPDATE ADMStudentMEDHistory SET Id = @Id WHERE MEDHistoryID = @MEDHistoryID;";
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
            sql = "DELETE FROM ADMStudentMEDHistory WHERE MEDHistoryID = @MEDHistoryID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { MEDHistoryID = id });
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

        public Task<IReadOnlyList<ADMStudentMEDHistory>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ADMStudentExitRepository : IADMStudentExitRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMStudentExit> _list { get; set; }
        ADMStudentExit _details = new();

        public ADMStudentExitRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ADMStudentExit>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT ExitID, ExitType FROM ADMStudentExit;";

                            _list = (List<ADMStudentExit>)await connection.QueryAsync<ADMStudentExit>(sql, new { });
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

        public async Task<ADMStudentExit> GetByIdAsync(int id)
        {
            _details = new ADMStudentExit();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT ExitType FROM ADMStudentExit WHERE ExitID =@ExitID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ADMStudentExit>(sql, new { ExitID = id });
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

        public async Task<ADMStudentExit> AddAsync(ADMStudentExit entity)
        {
            sql = @"INSERT INTO ADMStudentExit (ExitType) OUTPUT INSERTED.ExitID VALUES (@ExitType)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.ExitID = result;
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

        public async Task<ADMStudentExit> UpdateAsync(int id, ADMStudentExit entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMStudentExit SET ExitType = @ExitType WHERE ExitID = @ExitID;";
                    break;
                case 2:
                    sql = "UPDATE ADMStudentExit SET Id = @Id WHERE ExitID = @ExitID;";
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
            sql = "DELETE FROM ADMStudentExit WHERE ExitID = @ExitID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { ExitID = id });
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

        public Task<IReadOnlyList<ADMStudentExit>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ADMSchClubRepository : IADMSchClubRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMSchClub> _list { get; set; }
        ADMSchClub _details = new();

        public ADMSchClubRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ADMSchClub>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT ClubID, ClubName FROM ADMSchClub;";

                            _list = (List<ADMSchClub>)await connection.QueryAsync<ADMSchClub>(sql, new { });
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

        public async Task<ADMSchClub> GetByIdAsync(int id)
        {
            _details = new ADMSchClub();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT ClubName FROM ADMSchClub WHERE ClubID =@ClubID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ADMSchClub>(sql, new { ClubID = id });
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

        public async Task<ADMSchClub> AddAsync(ADMSchClub entity)
        {
            sql = @"INSERT INTO ADMSchClub (ClubName) OUTPUT INSERTED.ClubID VALUES (@ClubName)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.ClubID = result;
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

        public async Task<ADMSchClub> UpdateAsync(int id, ADMSchClub entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMSchClub SET ClubName = @ClubName WHERE ClubID = @ClubID;";
                    break;
                case 2:
                    sql = "UPDATE ADMSchClub SET Id = @Id WHERE ClubID = @ClubID;";
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
            sql = "DELETE FROM ADMSchClub WHERE ClubID = @ClubID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { ClubID = id });
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

        public Task<IReadOnlyList<ADMSchClub>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ADMSchClubRoleRepository : IADMSchClubRoleRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMSchClubRole> _list { get; set; }
        ADMSchClubRole _details = new();

        public ADMSchClubRoleRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ADMSchClubRole>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT RoleID, RoleName FROM ADMSchClubRole;";

                            _list = (List<ADMSchClubRole>)await connection.QueryAsync<ADMSchClubRole>(sql, new { });
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

        public async Task<ADMSchClubRole> GetByIdAsync(int id)
        {
            _details = new ADMSchClubRole();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT RoleName FROM ADMSchClubRole WHERE RoleID =@RoleID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ADMSchClubRole>(sql, new { RoleID = id });
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

        public async Task<ADMSchClubRole> AddAsync(ADMSchClubRole entity)
        {
            sql = @"INSERT INTO ADMSchClubRole (RoleName) OUTPUT INSERTED.RoleID VALUES (@RoleName)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.RoleID = result;
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

        public async Task<ADMSchClubRole> UpdateAsync(int id, ADMSchClubRole entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMSchClubRole SET RoleName = @RoleName WHERE RoleID = @RoleID;";
                    break;
                case 2:
                    sql = "UPDATE ADMSchClubRole SET Id = @Id WHERE RoleID = @RoleID;";
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
            sql = "DELETE FROM ADMSchClubRole WHERE RoleID = @RoleID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { RoleID = id });
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

        public Task<IReadOnlyList<ADMSchClubRole>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ADMSchParentsRepository : IADMSchParentsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMSchParents> parents { get; set; }
        ADMSchParents parent = new();

        public ADMSchParentsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ADMSchParents>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT A.PayeeID, A.PayeeID AS ParentID, A.SchInfoID, A.StatusTypeID, A.StudentCount, A.ParentTitle, A.ParentSurname, " +
                                "A.FatherName, A.FatherPhones, A.FatherPhonesAlternate, A.FatherOccupation, A.FatherAddrHome, A.FatherAddrWork, " +
                                "A.FatherEmail, A.photoStatusFather, A.fatherPhoto, A.MotherName, A.MotherPhones, A.MotherPhonesAlternate, " +
                                "A.MotherOccupation, A.MotherAddrHome, A.MotherAddrWork, A.MotherEmail, A.photoStatusMother, A.motherPhoto, " +
                                "A.RegistrationDate, A.Memo, A.DeletePayee, A.PrefixID, B.StatusType, " +
                                "C.PrefixName + REPLACE(STR(A.PayeeID, C.PrefixDigits), SPACE(1), '0') AS ParentNo, " +
                                "A.ParentTitle + ' ' + A.ParentSurname + ' [' + C.PrefixName + REPLACE(STR(A.PayeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName " +
                                "FROM ADMSchParents A " +
                                "INNER JOIN SETStatusType B ON B.StatusTypeID = A.StatusTypeID " +
                                "INNER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                                "WHERE A.DeletePayee = 'False' AND A.StatusTypeID = @StatusTypeID ORDER BY A.ParentSurname;";

                            parents = (List<ADMSchParents>)await connection.QueryAsync<ADMSchParents>(sql, 
                                new 
                                {
                                    StatusTypeID = _switch.StatusTypeID
                                });
                            break;
                        case 2:
                            sql = "SELECT A.PayeeID, A.PayeeID AS ParentID, A.SchInfoID, A.StatusTypeID, A.StudentCount, A.ParentTitle, A.ParentSurname, " +
                                "A.FatherName, A.FatherPhones, A.FatherPhonesAlternate, A.FatherOccupation, A.FatherAddrHome, A.FatherAddrWork, " +
                                "A.FatherEmail, A.photoStatusFather, A.fatherPhoto, A.MotherName, A.MotherPhones, A.MotherPhonesAlternate, " +
                                "A.MotherOccupation, A.MotherAddrHome, A.MotherAddrWork, A.MotherEmail, A.photoStatusMother, A.motherPhoto, " +
                                "A.RegistrationDate, A.Memo, A.DeletePayee, A.PrefixID, B.StatusType, " +
                                "C.PrefixName + REPLACE(STR(A.PayeeID, C.PrefixDigits), SPACE(1), '0') AS ParentNo, " +
                                "A.ParentTitle + ' ' + A.ParentSurname + ' [' + C.PrefixName + REPLACE(STR(A.PayeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName " +
                                "FROM ADMSchParents A " +
                                "INNER JOIN SETStatusType B ON B.StatusTypeID = A.StatusTypeID " +
                                "INNER JOIN SETPrefix C ON C.PrefixID = A.PrefixID ORDER BY A.ParentSurname" +
                                "WHERE A.DeletePayee = 'False';";

                            parents = (List<ADMSchParents>)await connection.QueryAsync<ADMSchParents>(sql,
                                new
                                {
                                    
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

            return parents;
        }

        public async Task<ADMSchParents> GetByIdAsync(int id)
        {
            parent = new ADMSchParents();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.PayeeID, A.PayeeID AS ParentID, A.SchInfoID, A.StatusTypeID, A.StudentCount, A.ParentTitle, A.ParentSurname, " +
                "A.FatherName, A.FatherPhones, A.FatherPhonesAlternate, A.FatherOccupation, A.FatherAddrHome, A.FatherAddrWork, " +
                "A.FatherEmail, A.photoStatusFather, A.fatherPhoto, A.MotherName, A.MotherPhones, A.MotherPhonesAlternate, " +
                "A.MotherOccupation, A.MotherAddrHome, A.MotherAddrWork, A.MotherEmail, A.photoStatusMother, A.motherPhoto, " +
                "A.RegistrationDate, A.Memo, A.DeletePayee, A.PrefixID, B.StatusType, " +
                "A.ParentTitle + ' ' + A.ParentSurname + ' [' + C.PrefixName + REPLACE(STR(A.PayeeID, C.PrefixDigits), SPACE(1), '0') + ']' AS ParentName " +
                "FROM ADMSchParents A " +
                "INNER JOIN SETStatusType B ON B.StatusTypeID = A.StatusTypeID " +
                "INNER JOIN SETPrefix C ON C.PrefixID = A.PrefixID " +
                "WHERE A.PayeeID = @PayeeID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                parent = await connection.QueryFirstOrDefaultAsync<ADMSchParents>(sql, new { PayeeID = id });
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

            return parent;
        }

        public async Task<ADMSchParents> AddAsync(ADMSchParents entity)
        {
            entity.RegistrationDate = DateTime.Now;
            entity.ModifiedDate = DateTime.Now; 
            entity.DeletePayee = false;

            sql = @"INSERT INTO ADMSchParents (SchInfoID, StatusTypeID, StudentCount, ParentTitle, ParentSurname, FatherName, FatherPhones, 
                FatherPhonesAlternate, FatherOccupation, FatherAddrHome, FatherAddrWork, FatherEmail, photoStatusFather, fatherPhoto, 
                MotherName, MotherPhones, MotherPhonesAlternate, MotherOccupation, MotherAddrHome, MotherAddrWork, MotherEmail, photoStatusMother, 
                motherPhoto, RegistrationDate, Memo, PrefixID, ModifiedDate, DeletePayee) OUTPUT INSERTED.PayeeID VALUES (@SchInfoID, @StatusTypeID, 
                @StudentCount, @ParentTitle, @ParentSurname, @FatherName, @FatherPhones, @FatherPhonesAlternate, @FatherOccupation, @FatherAddrHome, 
                @FatherAddrWork, @FatherEmail, @photoStatusFather, @fatherPhoto, @MotherName, @MotherPhones, @MotherPhonesAlternate, @MotherOccupation, 
                @MotherAddrHome, @MotherAddrWork, @MotherEmail, @photoStatusMother, @motherPhoto, @RegistrationDate, @Memo, @PrefixID, @ModifiedDate, 
                @DeletePayee)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.PayeeID = result;
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

        public async Task<ADMSchParents> UpdateAsync(int id, ADMSchParents entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMSchParents SET SchInfoID = @SchInfoID, StatusTypeID = @StatusTypeID, ParentTitle = @ParentTitle, " +
                        "ParentSurname = @ParentSurname, FatherName = @FatherName, FatherPhones = @FatherPhones, " +
                        "FatherPhonesAlternate = @FatherPhonesAlternate, FatherOccupation = @FatherOccupation, FatherAddrHome = @FatherAddrHome, " +
                        "FatherAddrWork = @FatherAddrWork, FatherEmail = @FatherEmail, photoStatusFather = @photoStatusFather, fatherPhoto = @fatherPhoto, " +
                        "MotherName = @MotherName, MotherPhones = @MotherPhones, MotherPhonesAlternate = @MotherPhonesAlternate, " +
                        "MotherOccupation = @MotherOccupation, MotherAddrHome = @MotherAddrHome, MotherAddrWork = @MotherAddrWork, MotherEmail = @MotherEmail, " +
                        "photoStatusMother = @photoStatusMother, motherPhoto = @motherPhoto, Memo = @Memo, ModifiedDate = @ModifiedDate " +
                        "WHERE PayeeID = @PayeeID;";
                    break;
                case 2: //Delete Parent
                    sql = "UPDATE ADMSchParents SET DeletePayee = @DeletePayee WHERE  PayeeID = @PayeeID;";
                    break;
                case 3: //Student Count
                    sql = "UPDATE ADMSchParents SET StudentCount = @StudentCount WHERE  PayeeID = @PayeeID;";
                    break;
                case 4:
                    sql = "UPDATE ADMSchParents SET Id = @Id WHERE PayeeID = @PayeeID;";
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
            sql = "DELETE FROM ADMSchParents WHERE PayeeID = @PayeeID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { PayeeID = id });
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

        public Task<int> CountAsync(SwitchModel _switcht)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ADMSchParents>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }
}
