using WebAppAcademics.Server.Interfaces.Academics.Exam;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;
using System.Data;
using System.Data.SqlClient;

namespace WebAppAcademics.Server.Repositories
{
    public class ACDCognitiveMarkEntryRepository : IACDCognitiveMarkEntryRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDStudentsMarksCognitive> _list { get; set; }
        ACDStudentsMarksCognitive _details = new();

        public ACDCognitiveMarkEntryRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDStudentsMarksCognitive>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: //Mid-Term Exam Mark Entry Filter For Results
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND A.ClassID = @ClassID AND EntryStatus_MidTerm = @EntryStatus_MidTerm " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassID,
                                    SbjSelection = true, //SbjSelection Is True Means Subject Is Allocated To Selected Students
                                    EntryStatus_MidTerm = true //This Indicates Marks Entry for the Term Is Completed
                                });
                            break;
                        case 2: //End of Term Exam Mark Entry Filter  For Results
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND A.ClassID = @ClassID AND EntryStatus_TermEnd = @EntryStatus_TermEnd " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassID,
                                    SbjSelection = true, //SbjSelection Is True Means Subject Is Allocated To Selected Students
                                    EntryStatus_TermEnd = true //This Indicates Marks Entry for the Term Is Completed
                                });
                            break;
                        case 3:  //ICGCS Exam Mark Entry Filter For Results
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND A.ClassID = @ClassID AND EntryStatus_ICGCS = @EntryStatus_ICGCS " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassID,
                                    SbjSelection = true, //SbjSelection Is True Means Subject Is Allocated To Selected Students
                                    EntryStatus_ICGCS = true //This Indicates Marks Entry for the Term Is Completed
                                });
                            break;
                        case 4: //Mark Entry Filter Per Student For A Selected Class
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND A.ClassID = @ClassID AND A.STDID = @STDID " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                               new
                               {
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassID,
                                    SbjSelection = true, //SbjSelection Is True Means Subject Is Allocated To Selected Students
                                    _switch.STDID 
                               });
                            break;
                        case 5: //Mark Entry Filter By School, Class, Teacher And Subject
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND A.ClassID = @ClassID AND A.StaffID = @StaffID AND A.SubjectID = @SubjectID " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassID,
                                    SbjSelection = true, //SbjSelection Is True Means Subject Is Allocated To Selected Students
                                    _switch.StaffID, 
                                    _switch.SubjectID
                                });
                            break;
                        case 6: 
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND A.ClassID = @ClassID " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassID,
                                    SbjSelection = true
                                });
                            break;
                        case 7: //Mark Entry Filter By School, Class And Subject
                            //sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                            //    "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                            //    "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                            //    "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                            //    "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                            //    "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                            //    "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, A.Id, " +
                            //    "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                            //    "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                            //    "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                            //    "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                            //    "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                            //    "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                            //    "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                            //    "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                            //    "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                            //    "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID) AS SN " +
                            //    "FROM ACDStudentsMarksCognitive A " +
                            //    "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                            //    "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                            //    "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                            //    "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                            //    "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                            //    "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                            //    "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                            //    "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                            //    "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                            //    "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                            //    "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                            //    "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                            //    "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                            //    "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                            //    "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                            //    "AND A.SchID = @SchID AND A.ClassID = @ClassID AND A.SubjectID = @SubjectID " +
                            //    "ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID;";
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, A.Id, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND A.ClassID = @ClassID AND A.SubjectID = @SubjectID " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassID,
                                    SbjSelection = true, //SbjSelection Is True Means Subject Is Allocated To Selected Students
                                    _switch.SubjectID
                                });
                            break;
                        case 8: //Retrieve Selected Student Marks
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.STDID = @STDID " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.STDID,
                                    SbjSelection = true, //SbjSelection Is True Means Subject Is Allocated To Selected Students
                                });
                            break;
                        case 9://Filter Marks By Term And School
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID ORDER BY A.SchID, E.ClassListID, E.CATID, J.SortID, S.Surname) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, J.SortID, S.Surname;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SchID
                                });
                            break;
                        case 10://Filter Marks By Term, School And Class
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID, A.STDID ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SortID) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND A.ClassID = @ClassID " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SortID;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassID
                                });
                            break;
                        case 11://Filter Marks By Term And Student
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID ORDER BY A.SchID, E.ClassListID, E.CATID, J.SortID) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.TermID = @TermID AND A.STDID = @STDID " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, J.SortID;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.STDID
                                });
                            break;
                        case 12://Filter Marks By Term, Student and Subject
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY J.Subject ORDER BY J.Subject) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.TermID = @TermID " +
                                "AND A.STDID = @STDID AND A.SubjectID = @SubjectID " +
                                "ORDER BY J.Subject;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.STDID,
                                    _switch.SubjectID
                                });
                            break;
                        case 13://Filter Marks By Term, School And Subject
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND A.SubjectID = @SubjectID " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.SubjectID
                                });
                            break;
                        case 14://Filter Marks By Term, School, Class and Subject
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID, J.Subject ORDER BY A.SchID, E.ClassListID, E.CATID, J.SbjClassID, J.Subject, J.SortID, S.Surname ) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND A.ClassID = @ClassID AND A.SubjectID = @SubjectID " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, J.SbjClassID, J.Subject, J.SortID, S.Surname;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassID,
                                    _switch.SubjectID
                                });
                            break;                       
                        case 15://Filter Marks By Term And Teacher
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID, J.Subject ORDER BY A.SchID, E.ClassListID, E.CATID, J.SbjClassID, J.Subject, J.SortID, S.Surname ) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.TermID = @TermID " +
                                "AND A.StaffID = @StaffID " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, J.SbjClassID, J.Subject, J.SortID, S.Surname;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,                                    
                                    _switch.StaffID
                                });
                            break;
                        case 16://Filter Marks By Term, School And Teacher
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID,  " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID, J.Subject ORDER BY A.SchID, E.ClassListID, E.CATID, J.SbjClassID, J.Subject, J.SortID, S.Surname ) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND A.StaffID = @StaffID " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, J.SbjClassID, J.Subject, J.SortID, S.Surname;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.StaffID
                                });
                            break;
                        case 17://Filter Marks By Term, School, Subject And Teacher
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID, J.Subject ORDER BY A.SchID, E.ClassListID, E.CATID, J.SbjClassID, J.Subject, J.SortID, S.Surname ) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND A.SubjectID = @SubjectID " +
                                "AND A.StaffID = @StaffID " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, J.SbjClassID, J.Subject, J.SortID, S.Surname;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.SubjectID,
                                    _switch.StaffID
                                });
                            break;
                        case 18://Filter Marks By Term, School, Class and Teacher
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID, J.Subject ORDER BY A.SchID, E.ClassListID, E.CATID, J.SbjClassID, J.Subject, J.SortID, S.Surname ) AS SN " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND  A.ClassID = @ClassID " +
                                "AND A.StaffID = @StaffID " +
                                "ORDER BY A.SchID, E.ClassListID, E.CATID, J.SbjClassID, J.Subject, J.SortID, S.Surname;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassID,
                                    _switch.StaffID
                                });
                            break;
                        case 19: //Mid-Term Exam Mark - Distinct Student STDID
                            sql = "SELECT DISTINCT A.STDID, S.Surname + ' ' + S.FirstName + ' ' + ISNULL(S.MiddleName, ' ') AS StudentName, " +
                                "S.Surname + ' ' + S.FirstName + ' ' +  ISNULL(S.MiddleName, ' ') + ' [' + D.PrefixName + REPLACE(STR(S.StudentID, D.PrefixDigits), SPACE(1), '0') + ']'  AS StudentNameWithNo " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix D ON D.PrefixID = S.PrefixID " +
                                "WHERE Mark_Delete = 'False' AND SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND A.ClassID = @ClassID;"; // AND EntryStatus_MidTerm = @EntryStatus_MidTerm;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassID,
                                    SbjSelection = true, //SbjSelection Is True Means Subject Is Allocated To Selected Students
                                    //EntryStatus_MidTerm = true //This Indicates Marks Entry for the Term Is Completed
                                });
                            break;
                        case 20: //End of Term Exam Mark  - Distinct Student STDID
                            sql = "SELECT DISTINCT A.STDID, S.Surname + ' ' + S.FirstName + ' ' + ISNULL(S.MiddleName, ' ') AS StudentName, " +
                                "S.Surname + ' ' + S.FirstName + ' ' +  ISNULL(S.MiddleName, ' ') + ' [' + D.PrefixName + REPLACE(STR(S.StudentID, D.PrefixDigits), SPACE(1), '0') + ']'  AS StudentNameWithNo " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix D ON D.PrefixID = S.PrefixID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND A.ClassID = @ClassID;"; // AND A.EntryStatus_TermEnd = @EntryStatus_TermEnd;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    TermID = _switch.TermID,
                                    SchID = _switch.SchID,
                                    ClassID = _switch.ClassID,
                                    SbjSelection = true, //SbjSelection Is True Means Subject Is Allocated To Selected Students
                                    //EntryStatus_TermEnd = true //This Indicates Marks Entry for the Term Is Completed
                                });
                            break;
                        case 21:  //ICGCS Exam Mark  - Distinct Student STDID
                            sql = "SELECT DISTINCT A.STDID, S.Surname + ' ' + S.FirstName + ' ' + ISNULL(S.MiddleName, ' ') AS StudentName, " +
                                "S.Surname + ' ' + S.FirstName + ' ' +  ISNULL(S.MiddleName, ' ') + ' [' + D.PrefixName + REPLACE(STR(S.StudentID, D.PrefixDigits), SPACE(1), '0') + ']'  AS StudentNameWithNo " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix D ON D.PrefixID = S.PrefixID " +
                                "WHERE Mark_Delete = 'False' AND SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND A.ClassID = @ClassID;";// AND EntryStatus_ICGCS = @EntryStatus_ICGCS;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassID,
                                    SbjSelection = true, //SbjSelection Is True Means Subject Is Allocated To Selected Students
                                    //EntryStatus_ICGCS = true //This Indicates Marks Entry for the Term Is Completed
                                });
                            break;
                        case 22: //Exam Mark Entry Filter By School Term
                            sql = "SELECT StudentMarkID, TermID, SchID, ClassID, StaffID, ClassTeacherID, SchoolPrincipalID, STDID, SubjectID, " +
                                "Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, Mark_CA3, Mark_CBT, Mark_Exam, EntryStatus_MidTerm, " +
                                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                "FROM ACDStudentsMarksCognitive WHERE Mark_Delete = 'False' AND TermID = @TermID;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID
                                });
                            break;
                        case 23://Filter Marks By Term and Teacher                               
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                   "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                   "Mark_CA3, Mark_CBT, Mark_Exam, EntryStatus_MidTerm, EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, " +
                                   "Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +
                                   "ROW_NUMBER() OVER (PARTITION BY A.SchID ORDER BY A.SchID ) AS SN, A.Id " +
                                   "FROM ACDStudentsMarksCognitive A " +
                                   "WHERE Mark_Delete = 'False' AND A.SbjSelection = 1 AND A.TermID = @TermID " +
                                   "AND A.StaffID = @StaffID ORDER BY A.SchID;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.StaffID
                                });
                            break;
                        case 24: //Marks for Comments - Administrator 
                            sql = "SELECT StudentMarkID, TermID, SchID, ClassID, StaffID, ClassTeacherID, SchoolPrincipalID, STDID, " +
                                "SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, Mark_CA3, Mark_CBT, Mark_Exam, " +
                                "EntryStatus_MidTerm, EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, A.Id " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = 'True' AND A.TermID = @TermID;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID
                                });
                            break;
                        case 25: //Marks for Comments - Staff 
                            sql = "SELECT StudentMarkID, TermID, SchID, ClassID, StaffID, ClassTeacherID, SchoolPrincipalID, STDID, " +
                                "SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, Mark_CA3, Mark_CBT, Mark_Exam, " +
                                "EntryStatus_MidTerm, EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, A.Id " +
                                "FROM ACDStudentsMarksCognitive A " +
                                "WHERE Mark_Delete = 'False' AND SbjSelection = 'True' AND TermID = @TermID AND StaffID = @StaffID;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.StaffID
                                });
                            break;
                        default: //Exam Mark Entry Filter By School Term     
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                                    "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                                    "Mark_CA3, Mark_CBT, Mark_Exam, EntryStatus_MidTerm, EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, " +
                                    "Mark_CA1 + Mark_CA2 + Mark_CA3 + Mark_CBT + Mark_Exam AS Total_Exam, " +                                
                                    "ROW_NUMBER() OVER (PARTITION BY A.SchID ORDER BY A.SchID ) AS SN, A.Id " +
                                    "FROM ACDStudentsMarksCognitive A " +
                                    "WHERE Mark_Delete = 'False' AND A.SbjSelection = 'True' AND A.TermID = @TermID ORDER BY A.SchID;";

                            _list = (List<ACDStudentsMarksCognitive>)await connection.QueryAsync<ACDStudentsMarksCognitive>(sql,
                                new
                                {
                                    _switch.TermID
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

        public async Task<ACDStudentsMarksCognitive> GetByIdAsync(int id)
        {
            _details = new ACDStudentsMarksCognitive();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.ClassTeacherID, " +
                "A.SchoolPrincipalID, A.STDID, A.SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, " +
                "Mark_CA3, Mark_CBT, Mark_Exam, GradeID, GradeID_Mid, GradeID_ICGC, EntryStatus_MidTerm, " +
                "EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, " +
                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' Academic Year' AS AcademicSession, " +
                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                "B.SchTerm, B.SchTerm + ' Term' AS CurrentTerm, C.School, J.SortID, " +
                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(D.MiddleName, ' ') AS Principal, D.signPicture AS signPrincipal, " +
                "G.Surname + ' ' + G.FirstName + ' ' + ISNULL(G.MiddleName, ' ') AS ClassTeacher, G.signPicture AS signClassTeacher, " +
                "H.Surname + ' ' + H.FirstName + ' ' + ISNULL(H.MiddleName, ' ') AS Teacher, S.Email, " +
                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, S.studentPhoto, " +
                "J.SubjectCode, J.Subject, N.Gender, ISNULL(DATEDIFF(year, S.DOB, GETDATE()), 0) AS Age, J.SortID, " +
                "L.ClubName AS YouthClub, M.RoleName AS YouthRole, J.SbjMerge, J.SbjMergeID, J.SbjMergeName, " +
                "S.Surname + ' ' + SUBSTRING(S.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(S.MiddleName, 0, 2) + '. ', '') + ' ' + " +
                "REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0')  AS StudentInitials, " +
                "ROW_NUMBER() OVER (PARTITION BY A.SchID, E.ClassListID, E.CATID ORDER BY A.SchID, E.ClassListID, E.CATID, S.Surname, J.SbjClassID, J.SortID) AS SN " +
                "FROM ACDStudentsMarksCognitive A " +
                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                "LEFT OUTER JOIN ADMSchlList C ON C.SchID = A.SchID " +
                "LEFT OUTER JOIN ADMEmployee D ON D.StaffID = A.SchoolPrincipalID " +
                "LEFT OUTER JOIN ADMSchClassList E ON E.ClassID = A.ClassID " +
                "LEFT OUTER JOIN ADMSchClassGroup AS F ON F.ClassListID = E.ClassListID " +
                "LEFT OUTER JOIN ADMEmployee G ON G.StaffID = A.ClassTeacherID " +
                "LEFT OUTER JOIN ADMEmployee H ON H.StaffID = A.StaffID " +
                "LEFT OUTER JOIN ACDSubjects J ON J.SubjectID = A.SubjectID " +
                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                "LEFT OUTER JOIN ADMSchClub L ON L.ClubID = S.ClubID " +
                "LEFT OUTER JOIN ADMSchClubRole M ON M.RoleID = S.RoleID " +
                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                "LEFT OUTER JOIN SETGender N ON N.GenderID = S.GenderID " +
                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = E.CATID " +
                "WHERE Mark_Delete = 'False' AND A.StudentMarkID = @StudentMarkID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDStudentsMarksCognitive>(sql, new { StudentMarkID = id });
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

        public async Task<ACDStudentsMarksCognitive> AddAsync(ACDStudentsMarksCognitive entity)
        {
            entity.EntryStatus_MidTerm = false;
            entity.EntryStatus_TermEnd = false;
            entity.EntryStatus_ICGCS = false;
            entity.SbjSelection = true;
            entity.Mark_Delete = false;

            sql = @"INSERT INTO ACDStudentsMarksCognitive (TermID, SchSession, SchID, ClassID, StaffID, ClassTeacherID, SchoolPrincipalID, 
                    STDID, SubjectID, Mark_ICGC, Mark_Mid, Mark_MidCBT, Mark_CA1, Mark_CA2, Mark_CA3, Mark_CBT, Mark_Exam, GradeID, 
                    GradeID_ICGC, GradeID_Mid, EntryStatus_MidTerm, EntryStatus_TermEnd, EntryStatus_ICGCS, SbjSelection, Mark_Delete) 
                    OUTPUT INSERTED.StudentMarkID VALUES (@TermID, @SchSession, @SchID, @ClassID, @StaffID, @ClassTeacherID, 
                    @SchoolPrincipalID, @STDID, @SubjectID, @Mark_ICGC, @Mark_Mid, @Mark_MidCBT, @Mark_CA1, @Mark_CA2, @Mark_CA3, 
                    @Mark_CBT, @Mark_Exam, @GradeID, @GradeID_ICGC, @GradeID_Mid, @EntryStatus_MidTerm, @EntryStatus_TermEnd, 
                    @EntryStatus_ICGCS, @SbjSelection, @Mark_Delete);SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.StudentMarkID = result;
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

        public async Task<ACDStudentsMarksCognitive> UpdateAsync(int id, ACDStudentsMarksCognitive entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1: //Update Mid-Term Mark Entry
                    sql = "UPDATE ACDStudentsMarksCognitive SET Mark_Mid = @Mark_Mid, Mark_MidCBT = @Mark_MidCBT, GradeID_Mid = @GradeID_Mid, " +
                        "EntryStatus_MidTerm = @EntryStatus_MidTerm WHERE StudentMarkID = @StudentMarkID;";
                    break;
                case 2: //Update End of Term Mark Entry
                    sql = "UPDATE ACDStudentsMarksCognitive SET Mark_CA1 = @Mark_CA1, Mark_CA2 = @Mark_CA2, Mark_CA3 = @Mark_CA3, " +
                        "Mark_CBT = @Mark_CBT, Mark_Exam = @Mark_Exam, GradeID = @GradeID, EntryStatus_TermEnd = @EntryStatus_TermEnd " +
                        "WHERE StudentMarkID = @StudentMarkID;";
                    break;
                case 3: //Update IGSCE Mark Entry
                    sql = "UPDATE ACDStudentsMarksCognitive SET Mark_ICGC = @Mark_ICGC, GradeID_ICGC = @GradeID_ICGC, " +
                        "EntryStatus_ICGCS = @EntryStatus_ICGCS WHERE StudentMarkID = @StudentMarkID;";
                    break;
                case 4: //Update Mark Entry Status For Mid-Term Exam
                    sql = "UPDATE ACDStudentsMarksCognitive SET EntryStatus_MidTerm = @EntryStatus_MidTerm " +
                            "WHERE StudentMarkID = @StudentMarkID;";
                    break;
                case 5: //Update Mark Entry Status For End of Term Exam
                    sql = "UPDATE ACDStudentsMarksCognitive SET EntryStatus_TermEnd = @EntryStatus_TermEnd " +
                            "WHERE StudentMarkID = @StudentMarkID;";
                    break;
                case 6: //Update Mark Entry Status For IGSCE Exam
                    sql = "UPDATE ACDStudentsMarksCognitive SET EntryStatus_ICGCS = @EntryStatus_ICGCS " +
                           "WHERE StudentMarkID = @StudentMarkID;";
                    break;
                case 7: //Delete A Mark Entry
                    sql = "UPDATE ACDStudentsMarksCognitive SET Mark_Delete = @Mark_Delete WHERE StudentMarkID = @StudentMarkID;";
                    break;
                case 8: //Set SbjSelection Field To True/False IF Subject Allocation Is Set/Removed From A Student
                    sql = "UPDATE ACDStudentsMarksCognitive SET SbjSelection = @SbjSelection " +
                        "WHERE TermID = @TermID AND ClassID = @ClassID AND SubjectID = @SubjectID AND STDID = @STDID;";
                    break;
                case 9: //Update Staff ID On Change of Subject Allocation To Another Teacher
                    sql = "UPDATE ACDStudentsMarksCognitive SET StaffID = @StaffID " +
                        "WHERE TermID = @TermID AND SchID = @SchID AND ClassID = @ClassID AND SubjectID = @SubjectID;";
                    break;
                case 10: //Update Class Teacher ID On Change of Class Teacher
                    sql = "UPDATE ACDStudentsMarksCognitive SET ClassTeacherID = @ClassTeacherID " +
                        "WHERE TermID = @TermID AND SchID = @SchID AND ClassID = @ClassID;";
                    break;
                case 11: //Update Principal ID On Change of School Principal
                    sql = "UPDATE ACDStudentsMarksCognitive SET SchoolPrincipalID = @SchoolPrincipalID " +
                        "WHERE TermID = @TermID AND SchID = @SchID;";
                    break;
                case 12: //Update Mark Entry Per Student
                    sql = "UPDATE ACDStudentsMarksCognitive SET  Mark_Mid = @Mark_Mid, Mark_MidCBT = @Mark_MidCBT, Mark_ICGC = @Mark_ICGC, " +
                        "Mark_CA1 = @Mark_CA1, Mark_CA2 = @Mark_CA2, Mark_CA3 = @Mark_CA3, Mark_CBT = @Mark_CBT, Mark_Exam = @Mark_Exam, " +
                        "EntryStatus_MidTerm = @EntryStatus_MidTerm, EntryStatus_TermEnd = @EntryStatus_TermEnd, " +
                        "EntryStatus_ICGCS = @EntryStatus_ICGCS WHERE StudentMarkID = @StudentMarkID;";
                    break;
                case 13:
                    sql = "UPDATE ACDStudentsMarksCognitive SET Id = @Id WHERE StudentMarkID = @StudentMarkID;";
                    break;
                case 14: //Offline Synchronization
                    sql = "UPDATE ACDStudentsMarksCognitive SET Mark_ICGC = @Mark_ICGC, Mark_Mid = @Mark_Mid, " +
                        "Mark_MidCBT = @Mark_MidCBT, Mark_CA1 = @Mark_CA1, Mark_CA2 = @Mark_CA2, Mark_CA3 = @Mark_CA3, " +
                        "Mark_CBT = @Mark_CBT, Mark_Exam = @Mark_Exam, EntryStatus_ICGCS = @EntryStatus_ICGCS, " +
                        "EntryStatus_MidTerm = @EntryStatus_MidTerm, EntryStatus_TermEnd = @EntryStatus_TermEnd " +
                        "WHERE StudentMarkID = @StudentMarkID;";
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
            sql = "DELETE FROM ACDStudentsMarksCognitive WHERE StudentMarkID = @StudentMarkID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { StudentMarkID = id });
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

        public Task<IReadOnlyList<ACDStudentsMarksCognitive>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDOtherMarksEntryRepository : IACDOtherMarksEntryRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDStudentsMarksAssessment> _list { get; set; }
        ACDStudentsMarksAssessment _details = new();

        public ACDOtherMarksEntryRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDStudentsMarksAssessment>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.STDID, A.SubjectID, " +
                                "Rating, OptionID, TextID, RatingID, A.SbjSelection, E.School, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, I.SubjectCode, " +
                                "I.Subject, I.SbjClassID, J.SbjClassification AS SubjectClassification, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, C.ClassListID, C.CATID ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID) AS SN " +
                                "FROM ACDStudentsMarksAssessment A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = C.SchID " +
                                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects I ON I.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification J ON J.SbjClassID = I.SbjClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND  A.ClassID = @ClassID " +
                                "ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID;";

                            _list = (List<ACDStudentsMarksAssessment>)await connection.QueryAsync<ACDStudentsMarksAssessment>(sql,
                               new
                               {
                                   SbjSelection = true,
                                   _switch.TermID,
                                   _switch.SchID,
                                   _switch.ClassID
                               });
                            break;
                        case 2:
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.STDID, A.SubjectID, " +
                                "Rating, OptionID, TextID, RatingID, A.SbjSelection, E.School, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, I.SubjectCode, " +
                                "I.Subject, I.SbjClassID, J.SbjClassification AS SubjectClassification, " +
                                "ROW_NUMBER() Over (PARTITION BY A.SchID, C.ClassListID, C.CATID ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID) AS SN " +
                                "FROM ACDStudentsMarksAssessment A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = C.SchID " +
                                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects I ON I.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification J ON J.SbjClassID = I.SbjClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND  A.ClassID = @ClassID AND I.SbjClassID = @SbjClassID " +
                                "ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID;";

                            _list = (List<ACDStudentsMarksAssessment>)await connection.QueryAsync<ACDStudentsMarksAssessment>(sql,
                               new
                               {
                                   SbjSelection = true,
                                   _switch.TermID,
                                   _switch.SchID,
                                   _switch.ClassID,
                                   _switch.SbjClassID
                               });
                            break;
                        case 3:
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.STDID, A.SubjectID, " +
                                "Rating, OptionID, TextID, RatingID, A.SbjSelection, E.School, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, I.SubjectCode, " +
                                "I.Subject, I.SbjClassID, J.SbjClassification AS SubjectClassification, " +
                                "ROW_NUMBER() Over (PARTITION BY A.SchID, C.ClassListID, C.CATID ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID) AS SN " +
                                "FROM ACDStudentsMarksAssessment A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = C.SchID " +
                                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects I ON I.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification J ON J.SbjClassID = I.SbjClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND  A.ClassID = @ClassID AND I.SbjClassID = @SbjClassID AND A.STDID = @STDID " +
                                "ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID;";

                            _list = (List<ACDStudentsMarksAssessment>)await connection.QueryAsync<ACDStudentsMarksAssessment>(sql,
                               new
                               {
                                   SbjSelection = true,
                                   _switch.TermID,
                                   _switch.SchID,
                                   _switch.ClassID,
                                   _switch.SbjClassID,
                                   _switch.STDID
                               });
                            break;
                        case 4: //Filter By TermID and School
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.STDID, A.SubjectID, " +
                                "Rating, OptionID, TextID, RatingID, A.SbjSelection, E.School, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, I.SubjectCode, " +
                                "I.Subject, I.SbjClassID, J.SbjClassification AS SubjectClassification, " +
                                "ROW_NUMBER() Over (PARTITION BY A.SchID, C.ClassListID, C.CATID, A.STDID ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID) AS SN " +
                                "FROM ACDStudentsMarksAssessment A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = C.SchID " +
                                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects I ON I.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification J ON J.SbjClassID = I.SbjClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.SchID = @SchID " +
                                "ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID;";

                            _list = (List<ACDStudentsMarksAssessment>)await connection.QueryAsync<ACDStudentsMarksAssessment>(sql,
                               new
                               {
                                   SbjSelection = true,
                                   _switch.TermID,
                                   _switch.SchID
                               });
                            break;
                        case 5: //Filter By Class
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.STDID, A.SubjectID, " +
                                "Rating, OptionID, TextID, RatingID, A.SbjSelection, E.School, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, I.SubjectCode, " +
                                "I.Subject, I.SbjClassID, J.SbjClassification AS SubjectClassification, " +
                                "ROW_NUMBER() Over (PARTITION BY A.SchID, C.ClassListID, C.CATID, A.STDID ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID) AS SN " +
                                "FROM ACDStudentsMarksAssessment A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = C.SchID " +
                                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects I ON I.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification J ON J.SbjClassID = I.SbjClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.ClassID = @ClassID " +
                                "ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID;";

                            _list = (List<ACDStudentsMarksAssessment>)await connection.QueryAsync<ACDStudentsMarksAssessment>(sql,
                               new
                               {
                                   SbjSelection = true,
                                   _switch.TermID,
                                   _switch.ClassID
                               });
                            break;
                        case 6: //Filter By  Student
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.STDID, A.SubjectID, " +
                                "Rating, OptionID, TextID, RatingID, A.SbjSelection, E.School, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, I.SubjectCode, " +
                                "I.Subject, I.SbjClassID, J.SbjClassification AS SubjectClassification, " +
                                "ROW_NUMBER() Over (PARTITION BY A.SchID, C.ClassListID, C.CATID, I.SbjClassID ORDER BY A.SchID, C.ClassListID, C.CATID, I.SbjClassID, I.SortID) AS SN " +
                                "FROM ACDStudentsMarksAssessment A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = C.SchID " +
                                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects I ON I.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification J ON J.SbjClassID = I.SbjClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.STDID = @STDID " +
                                "ORDER BY A.SchID, C.ClassListID, C.CATID, I.SbjClassID, I.SortID;";

                            _list = (List<ACDStudentsMarksAssessment>)await connection.QueryAsync<ACDStudentsMarksAssessment>(sql,
                               new
                               {
                                   SbjSelection = true,
                                   _switch.TermID,
                                   _switch.STDID
                               });
                            break;
                        case 7: //Filter By Student and Accessment Type
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.STDID, A.SubjectID, " +
                                "Rating, OptionID, TextID, RatingID, A.SbjSelection, E.School, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, I.SubjectCode, " +
                                "I.Subject, I.SbjClassID, J.SbjClassification AS SubjectClassification, " +
                                "ROW_NUMBER() Over (PARTITION BY A.SchID, C.ClassListID, C.CATID, I.SbjClassID ORDER BY A.SchID, C.ClassListID, C.CATID, I.SbjClassID, I.SortID) AS SN " +
                                "FROM ACDStudentsMarksAssessment A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = C.SchID " +
                                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects I ON I.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification J ON J.SbjClassID = I.SbjClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.STDID = @STDID AND I.SbjClassID = @SbjClassID " +
                                "ORDER BY A.SchID, C.ClassListID, C.CATID, I.SbjClassID, I.SortID;";

                            _list = (List<ACDStudentsMarksAssessment>)await connection.QueryAsync<ACDStudentsMarksAssessment>(sql,
                               new
                               {
                                   SbjSelection = true,
                                   _switch.TermID,
                                   _switch.STDID,
                                   _switch.SbjClassID,
                               });
                            break;
                        case 8:  //Filter By School and Accessment Type
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.STDID, A.SubjectID, " +
                                "Rating, OptionID, TextID, RatingID, A.SbjSelection, E.School, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, I.SubjectCode, " +
                                "I.Subject, I.SbjClassID, J.SbjClassification AS SubjectClassification, " +
                                "ROW_NUMBER() Over (PARTITION BY A.SchID, C.ClassListID, C.CATID, A.STDID, I.SbjClassID ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID) AS SN " +
                                "FROM ACDStudentsMarksAssessment A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = C.SchID " +
                                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects I ON I.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification J ON J.SbjClassID = I.SbjClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.SchID = @SchID AND I.SbjClassID = @SbjClassID " +
                                "ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID;";

                            _list = (List<ACDStudentsMarksAssessment>)await connection.QueryAsync<ACDStudentsMarksAssessment>(sql,
                               new
                               {
                                   SbjSelection = true,
                                   _switch.TermID,
                                   _switch.SchID,
                                   _switch.SbjClassID,
                               });
                            break;
                        case 9:  //Filter By Class and Accessment Type
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.STDID, A.SubjectID, " +
                                "Rating, OptionID, TextID, RatingID, A.SbjSelection, E.School, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, I.SubjectCode, " +
                                "I.Subject, I.SbjClassID, J.SbjClassification AS SubjectClassification, " +
                                "ROW_NUMBER() Over (PARTITION BY A.SchID, C.ClassListID, C.CATID, A.STDID, I.SbjClassID ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID) AS SN " +
                                "FROM ACDStudentsMarksAssessment A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = C.SchID " +
                                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects I ON I.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification J ON J.SbjClassID = I.SbjClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.ClassID = @ClassID AND I.SbjClassID = @SbjClassID " +
                                "ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID;";

                            _list = (List<ACDStudentsMarksAssessment>)await connection.QueryAsync<ACDStudentsMarksAssessment>(sql,
                               new
                               {
                                   SbjSelection = true,
                                   _switch.TermID,
                                   _switch.ClassID,
                                   _switch.SbjClassID,
                               });
                            break;
                        case 10:
                            sql = "SELECT DISTINCT A.STDID, P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo," +
                                "S.Surname + ' ' + S.FirstName + ' ' + ISNULL(S.MiddleName, ' ') AS StudentName " +
                                "FROM ACDStudentsMarksAssessment A " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                                "AND A.SchID = @SchID AND  A.ClassID = @ClassID;";

                            _list = (List<ACDStudentsMarksAssessment>)await connection.QueryAsync<ACDStudentsMarksAssessment>(sql,
                               new
                               {
                                   SbjSelection = true,
                                   _switch.TermID,
                                   _switch.SchID,
                                   _switch.ClassID
                               });
                            break;
                        case 11:
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.STDID, A.SubjectID, " +
                                "Rating, OptionID, TextID, RatingID, A.SbjSelection, E.School, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, I.SubjectCode, " +
                                "I.Subject, I.SbjClassID, J.SbjClassification AS SubjectClassification, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, C.ClassListID, C.CATID ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID) AS SN " +
                                "FROM ACDStudentsMarksAssessment A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = C.SchID " +
                                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects I ON I.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification J ON J.SbjClassID = I.SbjClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.SbjSelection = @SbjSelection AND A.TermID = @TermID " +
                                "ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID;";

                            _list = (List<ACDStudentsMarksAssessment>)await connection.QueryAsync<ACDStudentsMarksAssessment>(sql,
                               new
                               {
                                   SbjSelection = true,
                                   _switch.TermID
                               });
                            break;
                        default:
                            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.STDID, A.SubjectID, " +
                                "Rating, OptionID, TextID, RatingID, A.SbjSelection, E.School, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, I.SubjectCode, " +
                                "I.Subject, I.SbjClassID, J.SbjClassification AS SubjectClassification, " +
                                "ROW_NUMBER() Over (PARTITION BY A.SchID, C.ClassListID, C.CATID, A.STDID ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID) AS SN " +
                                "FROM ACDStudentsMarksAssessment A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = C.SchID " +
                                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects I ON I.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification J ON J.SbjClassID = I.SbjClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE Mark_Delete = 'False' AND A.TermID = @TermID " +
                                "ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID;";

                            _list = (List<ACDStudentsMarksAssessment>)await connection.QueryAsync<ACDStudentsMarksAssessment>(sql,
                               new
                               {
                                   _switch.TermID
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

        public async Task<ACDStudentsMarksAssessment> GetByIdAsync(int id)
        {
            _details = new ACDStudentsMarksAssessment();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT StudentMarkID, A.TermID, A.SchSession, A.SchID, A.ClassID, A.StaffID, A.STDID, A.SubjectID, " +
                "Rating, OptionID, TextID, RatingID, A.SbjSelection, E.School, " +
                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, " +
                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, S.StudentID, " +
                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, I.SubjectCode, " +
                "I.Subject, I.SbjClassID, J.SbjClassification AS SubjectClassification, " +
                "ROW_NUMBER() Over (PARTITION BY A.SchID, C.ClassListID, C.CATID ORDER BY A.SchID, C.ClassListID, C.CATID, S.Surname, I.SbjClassID, I.SortID) AS SN " +
                "FROM ACDStudentsMarksAssessment A " +
                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                "LEFT OUTER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                "LEFT OUTER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = C.SchID " +
                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                "LEFT OUTER JOIN ACDSubjects I ON I.SubjectID = A.SubjectID " +
                "LEFT OUTER JOIN ACDSbjClassification J ON J.SbjClassID = I.SbjClassID " +
                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                "WHERE Mark_Delete = 'False' AND A.StudentMarkID = @StudentMarkID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDStudentsMarksAssessment>(sql, new { StudentMarkID = id });
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

        public async Task<ACDStudentsMarksAssessment> AddAsync(ACDStudentsMarksAssessment entity)
        {
            sql = @"INSERT INTO ACDStudentsMarksAssessment (TermID, SchSession, SchID, ClassID, StaffID, STDID, SubjectID, Rating, 
                    OptionID, TextID, RatingID, SbjSelection, Mark_Delete) OUTPUT INSERTED.StudentMarkID VALUES (@TermID, @SchSession,
                    @SchID, @ClassID, @StaffID, @STDID, @SubjectID, @Rating, @OptionID, @TextID, @RatingID, @SbjSelection, @Mark_Delete);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.StudentMarkID = result;
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

        public async Task<ACDStudentsMarksAssessment> UpdateAsync(int id, ACDStudentsMarksAssessment entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:  //Update Mark Entry
                    sql = "UPDATE ACDStudentsMarksAssessment SET Rating = @Rating, OptionID = @OptionID, TextID = @TextID, " +
                            "RatingID = @RatingID WHERE StudentMarkID = @StudentMarkID;";
                    break;
                case 2: //Delete A Mark Entry
                    sql = "UPDATE ACDStudentsMarksAssessment SET Mark_Delete = @Mark_Delete WHERE StudentMarkID = @StudentMarkID;";
                    break;
                case 3: //Update Class Teacher ID On Change of Class Teacher
                    sql = "UPDATE ACDStudentsMarksAssessment SET StaffID = @StaffID " +
                        "WHERE TermID = @TermID AND SchID = @SchID AND ClassID = @ClassID;";
                    break;
                case 4:
                    sql = "UPDATE ACDStudentsMarksAssessment SET Id = @Id WHERE StudentMarkID = @StudentMarkID;";
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
            sql = "DELETE FROM ACDStudentsMarksAssessment WHERE StudentMarkID = @StudentMarkID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { StudentMarkID = id });
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

        public Task<IReadOnlyList<ACDStudentsMarksAssessment>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

}
