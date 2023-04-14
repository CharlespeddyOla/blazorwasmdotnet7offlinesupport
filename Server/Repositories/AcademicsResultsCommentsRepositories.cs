using WebAppAcademics.Server.Interfaces.Academics.Exam;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;
using System.Data;
using System.Data.SqlClient;

namespace WebAppAcademics.Server.Repositories
{
    public class ACDResultsMidTermCommentsRepository : IACDResultsMidTermCommentsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDReportCommentMidTerm> _list { get; set; }
        ACDReportCommentMidTerm _details = new();

        public ACDResultsMidTermCommentsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDReportCommentMidTerm>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: //Retrieve Commenst For Selected Class
                            sql = "SELECT A.CommentID, A.TermID, A.SchSession, A.ClassID, A.ClassTeacherID, A.STDID, A.MarkObtainable, " +
                                "A.MarkObtained, A.Comments_Teacher, A.AVGPer, A.Position, A.Grade, A.Id, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                " E.Surname + ' ' + E.FirstName + ' ' + ISNULL(E.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, " +
                                "ROW_NUMBER() Over (PARTITION BY A.ClassID Order by Position) AS SN  " +
                                "FROM ACDReportCommentMidTerm A " +
                                "INNER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "INNER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "INNER JOIN ADMEmployee E ON E.StaffID = A.ClassTeacherID " +
                                "INNER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "INNER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "INNER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE A.TermID = @TermID AND A.ClassID = @ClassID ORDER BY Position;";

                            _list = (List<ACDReportCommentMidTerm>)await connection.QueryAsync<ACDReportCommentMidTerm>(sql,
                                       new
                                       {
                                           _switch.TermID,
                                           _switch.ClassID
                                       });
                            break;
                        case 2: //Retrieve Student Comment
                            sql = "SELECT A.CommentID, A.TermID, A.SchSession, A.ClassID, A.ClassTeacherID, A.STDID, A.MarkObtainable, " +
                                "A.MarkObtained, A.Comments_Teacher, A.AVGPer, A.Position, A.Grade, A.Id, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                " E.Surname + ' ' + E.FirstName + ' ' + ISNULL(E.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, " +
                                "ROW_NUMBER() Over (PARTITION BY A.ClassID Order by Position) AS SN  " +
                                "FROM ACDReportCommentMidTerm A " +
                                "INNER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "INNER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "INNER JOIN ADMEmployee E ON E.StaffID = A.ClassTeacherID " +
                                "INNER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "INNER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "INNER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE A.TermID = @TermID AND A.STDID = @STDID ORDER BY Position;";

                            _list = (List<ACDReportCommentMidTerm>)await connection.QueryAsync<ACDReportCommentMidTerm>(sql,
                                       new
                                       {
                                           _switch.TermID,
                                           _switch.STDID
                                       });
                            break;
                        default:
                            sql = "SELECT A.CommentID, A.TermID, A.SchSession, A.ClassID, A.ClassTeacherID, A.STDID, A.MarkObtainable, " +
                                "A.MarkObtained, A.Comments_Teacher, A.AVGPer, A.Position, A.Grade, A.Id, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                " E.Surname + ' ' + E.FirstName + ' ' + ISNULL(E.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, " +
                                "ROW_NUMBER() Over (PARTITION BY A.ClassID Order by Position) AS SN  " +
                                "FROM ACDReportCommentMidTerm A " +
                                "INNER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "INNER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "INNER JOIN ADMEmployee E ON E.StaffID = A.ClassTeacherID " +
                                "INNER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "INNER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "INNER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE A.TermID = @TermID ORDER BY Position;";

                            _list = (List<ACDReportCommentMidTerm>)await connection.QueryAsync<ACDReportCommentMidTerm>(sql,
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

        public async Task<ACDReportCommentMidTerm> GetByIdAsync(int id)
        {
            _details = new ACDReportCommentMidTerm();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.CommentID, A.TermID, A.SchSession, A.ClassID, A.ClassTeacherID, A.STDID, A.MarkObtainable, " +
                    "A.MarkObtained, A.Comments_Teacher, A.AVGPer, A.Position, A.Grade, " +
                    "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                    "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                    " E.Surname + ' ' + E.FirstName + ' ' + ISNULL(E.MiddleName, ' ') AS ClassTeacher, " +
                    "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                    "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName " +
                    "FROM ACDReportCommentMidTerm A " +
                    "INNER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                    "INNER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                    "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                    "INNER JOIN ADMEmployee E ON E.StaffID = A.ClassTeacherID " +
                    "INNER JOIN ADMStudents S ON S.STDID = A.STDID " +
                    "INNER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                    "INNER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                    "WHERE A.CommentID = @CommentID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDReportCommentMidTerm>(sql, new { CommentID = id });
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

        public async Task<ACDReportCommentMidTerm> AddAsync(ACDReportCommentMidTerm entity)
        {            
            sql = @"INSERT INTO ACDReportCommentMidTerm (TermID, SchSession, ClassID, ClassTeacherID, STDID, MarkObtainable, MarkObtained, 
                    Comments_Teacher, AVGPer, Position, Grade) OUTPUT INSERTED.CommentID VALUES (@TermID, @SchSession, @ClassID, 
                    @ClassTeacherID, @STDID, @MarkObtainable, @MarkObtained, @Comments_Teacher, @AVGPer, @Position, @Grade);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.CommentID = result;
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

        public async Task<ACDReportCommentMidTerm> UpdateAsync(int id, ACDReportCommentMidTerm entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDReportCommentMidTerm SET MarkObtainable = @MarkObtainable, MarkObtained = @MarkObtained, " +
                        "Comments_Teacher = @Comments_Teacher, AVGPer = @AVGPer, Position = @Position, Grade = @Grade " +
                        "WHERE CommentID = @CommentID";
                    break;
                case 2://Update Comment Only
                    sql = "UPDATE ACDReportCommentMidTerm SET Comments_Teacher = @Comments_Teacher WHERE CommentID = @CommentID";
                    break;
                case 3:
                    sql = "UPDATE ACDReportCommentMidTerm SET ClassTeacherID = @ClassTeacherID WHERE ClassID = @ClassID";
                    break;
                case 4:
                    sql = "UPDATE ACDReportCommentMidTerm SET Id = @Id WHERE CommentID = @CommentID;";
                    break;
                case 5: //Update Class And Class Teacher On Student Class Change
                    sql = "UPDATE ACDReportCommentMidTerm SET ClassID = @ClassID, ClassTeacherID = @ClassTeacherID " +
                        "WHERE STDID = @STDID";
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
            sql = "DELETE FROM ACDReportCommentMidTerm WHERE CommentID = @CommentID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { CommentID = id });
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

        public Task<IReadOnlyList<ACDReportCommentMidTerm>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDResultsTermEndCommentsRepository : IACDResultsTermEndCommentsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDReportCommentsTerminal> _list { get; set; }
        ACDReportCommentsTerminal _details = new();

        public ACDResultsTermEndCommentsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDReportCommentsTerminal>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: //Retrieve Commenst For Selected Class
                            sql = "SELECT A.CommentID, A.TermID, A.SchSession, A.ClassID, A.ClassTeacherID, A.STDID, A.Attendance, " +
                                "A.DaysAbsent, A.Comments_Teacher, A.Comments_Principal, AVGPer, Position, Grade, A.MarkObtainable, " +
                                "A.MarkObtained, A.Id, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "A.ClassTeacherID, E.Surname + ' ' + E.FirstName + ' ' + ISNULL(E.MiddleName, ' ') AS ClassTeacher, " +
                                " P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo,  " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName,  " +
                                "ROW_NUMBER() Over (PARTITION BY A.ClassID Order by Position) AS SN " +
                                "FROM ACDReportCommentTerminal A " +
                                "INNER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "INNER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "INNER JOIN ADMSchClassCategory AS F ON F.CATID = C.CATID " +
                                "INNER JOIN ADMEmployee E ON E.StaffID = A.ClassTeacherID " +
                                "INNER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "INNER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "INNER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE A.TermID = @TermID AND A.ClassID = @ClassID;";

                            _list = (List<ACDReportCommentsTerminal>)await connection.QueryAsync<ACDReportCommentsTerminal>(sql,
                                       new
                                       {
                                           TermID = _switch.TermID,
                                           _switch.ClassID
                                       });
                            break;
                        case 2:  //Retrieve Student Comment
                            sql = "SELECT A.CommentID, A.TermID, A.SchSession, A.ClassID, A.ClassTeacherID, A.STDID, A.Attendance, " +
                                "A.DaysAbsent, A.Comments_Teacher, A.Comments_Principal, AVGPer, Position, Grade, A.MarkObtainable, " +
                                "A.MarkObtained, A.Id, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "A.ClassTeacherID, E.Surname + ' ' + E.FirstName + ' ' + ISNULL(E.MiddleName, ' ') AS ClassTeacher, " +
                                " P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo,  " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName,  " +
                                "ROW_NUMBER() Over (PARTITION BY A.ClassID Order by Position) AS SN " +
                                "FROM ACDReportCommentTerminal A " +
                                "INNER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "INNER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "INNER JOIN ADMSchClassCategory AS F ON F.CATID = C.CATID " +
                                "INNER JOIN ADMEmployee E ON E.StaffID = A.ClassTeacherID " +
                                "INNER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "INNER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "INNER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE A.TermID = @TermID AND A.STDID = @STDID;";

                            _list = (List<ACDReportCommentsTerminal>)await connection.QueryAsync<ACDReportCommentsTerminal>(sql,
                                       new
                                       {
                                           TermID = _switch.TermID,
                                           STDID = _switch.STDID
                                       });
                            break;
                        default:
                            sql = "SELECT A.CommentID, A.TermID, A.SchSession, A.ClassID, A.ClassTeacherID, A.STDID, A.Attendance, " +
                                "A.DaysAbsent, A.Comments_Teacher, A.Comments_Principal, AVGPer, Position, Grade, A.MarkObtainable, " +
                                "A.MarkObtained, A.Id, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "A.ClassTeacherID, E.Surname + ' ' + E.FirstName + ' ' + ISNULL(E.MiddleName, ' ') AS ClassTeacher, " +
                                " P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo,  " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName,  " +
                                "ROW_NUMBER() Over (PARTITION BY A.ClassID Order by Position) AS SN " +
                                "FROM ACDReportCommentTerminal A " +
                                "LEFT OUTER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "LEFT OUTER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassCategory AS F ON F.CATID = C.CATID " +
                                "LEFT OUTER JOIN ADMEmployee E ON E.StaffID = A.ClassTeacherID " +
                                "LEFT OUTER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE A.TermID = @TermID;";

                            _list = (List<ACDReportCommentsTerminal>)await connection.QueryAsync<ACDReportCommentsTerminal>(sql,
                                       new
                                       {
                                           TermID = _switch.TermID
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

        public async Task<ACDReportCommentsTerminal> GetByIdAsync(int id)
        {
            _details = new ACDReportCommentsTerminal();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.CommentID, A.TermID, A.SchSession, A.ClassID, A.ClassTeacherID, A.STDID, A.Attendance, " +
                "A.DaysAbsent, A.Comments_Teacher, A.Comments_Principal, AVGPer, Position, Grade, A.MarkObtainable, " +
                "A.MarkObtained, " +
                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                "A.ClassTeacherID, E.Surname + ' ' + E.FirstName + ' ' + ISNULL(E.MiddleName, ' ') AS ClassTeacher, " +
                " P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo,  " +
                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName,  " +
                "ROW_NUMBER() Over (PARTITION BY A.ClassID Order by Position) AS SN " +
                "FROM ACDReportCommentTerminal A " +
                "INNER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                "INNER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                "INNER JOIN ADMSchClassCategory AS F ON F.CATID = C.CATID " +
                "INNER JOIN ADMEmployee E ON E.StaffID = A.ClassTeacherID " +
                "INNER JOIN ADMStudents S ON S.STDID = A.STDID " +
                "INNER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                "INNER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                "WHERE A.CommentID = @CommentID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDReportCommentsTerminal>(sql, new { CommentID = id });
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

        public async Task<ACDReportCommentsTerminal> AddAsync(ACDReportCommentsTerminal entity)
        {
            sql = @"INSERT INTO ACDReportCommentTerminal (TermID, SchSession, ClassID, ClassTeacherID, STDID, Attendance, DaysAbsent, 
                Comments_Teacher, Comments_Principal, AVGPer, Position, Grade, MarkObtainable, MarkObtained)  OUTPUT INSERTED.CommentID VALUES 
                (@TermID, @SchSession, @ClassID, @ClassTeacherID, @STDID, @Attendance, @DaysAbsent, @Comments_Teacher, @Comments_Principal, 
                @AVGPer, @Position, @Grade, @MarkObtainable, @MarkObtained);SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.CommentID = result;
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

        public async Task<ACDReportCommentsTerminal> UpdateAsync(int id, ACDReportCommentsTerminal entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDReportCommentTerminal SET MarkObtainable = @MarkObtainable, MarkObtained = @MarkObtained, " +
                        "Comments_Teacher = @Comments_Teacher, Comments_Principal = @Comments_Principal, AVGPer = @AVGPer, " +
                        "Position = @Position, Grade = @Grade, Attendance = @Attendance, DaysAbsent =  @DaysAbsent " +
                        "WHERE CommentID = @CommentID";
                    break;
                case 2: //Update Comments Only
                    sql = "UPDATE ACDReportCommentTerminal SET Comments_Teacher = @Comments_Teacher, Comments_Principal = @Comments_Principal, " +
                        "DaysAbsent =  @DaysAbsent WHERE CommentID = @CommentID";
                    break;
                case 3:
                    sql = "UPDATE ACDReportCommentTerminal SET ClassTeacherID = @ClassTeacherID WHERE ClassID = @ClassID";
                    break;
                case 4:
                    sql = "UPDATE ACDReportCommentTerminal SET Id = @Id WHERE CommentID = @CommentID;";
                    break;
                case 5: //Update Class And Class Teacher On Student Class Change
                    sql = "UPDATE ACDReportCommentTerminal SET ClassID = @ClassID, ClassTeacherID = @ClassTeacherID " +
                        "WHERE STDID = @STDID";
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
            sql = "DELETE FROM ACDReportCommentTerminal WHERE CommentID = @CommentID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { CommentID = id });
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

        public Task<IReadOnlyList<ACDReportCommentsTerminal>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDResultsCheckPointIGCSECommentsRepository : IACDResultsCheckPointIGCSECommentsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDReportCommentCheckPointIGCSE> _list { get; set; }
        ACDReportCommentCheckPointIGCSE _details = new();

        public ACDResultsCheckPointIGCSECommentsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDReportCommentCheckPointIGCSE>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: //Retrieve Commenst For Selected Class
                            sql = "SELECT A.CommentID, A.TermID, A.SchSession, A.ClassID, A.ClassTeacherID, A.STDID, A.Comments, AVGPer, " +
                                "Position, A.Grade, A.Id, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "E.Surname + ' ' + E.FirstName + ' ' + ISNULL(E.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, " +
                                "ROW_NUMBER() Over (PARTITION BY A.ClassID Order by Position) AS SN " +
                                "FROM ACDReportCommentCheckPointIGCSE A " +
                                "INNER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "INNER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "INNER JOIN ADMEmployee E ON E.StaffID = A.ClassTeacherID " +
                                "INNER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "INNER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "INNER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE A.TermID = @TermID AND A.ClassID = @ClassID ORDER BY Position;";

                            _list = (List<ACDReportCommentCheckPointIGCSE>)await connection.QueryAsync<ACDReportCommentCheckPointIGCSE>(sql,
                                       new
                                       {
                                           _switch.TermID,
                                           _switch.ClassID
                                       });
                            break;
                        case 2:  //Retrieve Student Comment
                            sql = "SELECT A.CommentID, A.TermID, A.SchSession, A.ClassID, A.ClassTeacherID, A.STDID, A.Comments, AVGPer, " +
                                "Position, A.Grade, A.Id, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "E.Surname + ' ' + E.FirstName + ' ' + ISNULL(E.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, " +
                                "ROW_NUMBER() Over (PARTITION BY A.ClassID Order by Position) AS SN " +
                                "FROM ACDReportCommentCheckPointIGCSE A " +
                                "INNER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "INNER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "INNER JOIN ADMEmployee E ON E.StaffID = A.ClassTeacherID " +
                                "INNER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "INNER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "INNER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE A.TermID = @TermID AND A.STDID = @STDID ORDER BY Position;";

                            _list = (List<ACDReportCommentCheckPointIGCSE>)await connection.QueryAsync<ACDReportCommentCheckPointIGCSE>(sql,
                                       new
                                       {
                                           _switch.TermID,
                                           _switch.STDID
                                       });
                            break;
                        default:
                            sql = "SELECT A.CommentID, A.TermID, A.SchSession, A.ClassID, A.ClassTeacherID, A.STDID, A.Comments, AVGPer, " +
                                "Position, A.Grade, A.Id, " +
                                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "E.Surname + ' ' + E.FirstName + ' ' + ISNULL(E.MiddleName, ' ') AS ClassTeacher, " +
                                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName, " +
                                "ROW_NUMBER() Over (PARTITION BY A.ClassID Order by Position) AS SN " +
                                "FROM ACDReportCommentCheckPointIGCSE A " +
                                "INNER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                                "INNER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                                "INNER JOIN ADMEmployee E ON E.StaffID = A.ClassTeacherID " +
                                "INNER JOIN ADMStudents S ON S.STDID = A.STDID " +
                                "INNER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                                "INNER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                                "WHERE A.TermID = @TermID ORDER BY Position;";

                            _list = (List<ACDReportCommentCheckPointIGCSE>)await connection.QueryAsync<ACDReportCommentCheckPointIGCSE>(sql,
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

        public async Task<ACDReportCommentCheckPointIGCSE> GetByIdAsync(int id)
        {
            _details = new ACDReportCommentCheckPointIGCSE();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.CommentID, A.TermID, A.SchSession, A.ClassID, A.ClassTeacherID, A.STDID, A.Comments, AVGPer, " +
                "Position, A.Grade, " +
                "SUBSTRING(CAST(B.SchSession AS varchar(14)), 0, 5) + '/' + SUBSTRING(CAST(B.SchSession AS varchar(14)), 5, 6) + ' - ' + B.SchTerm + ' Term' AS AcademicSession, " +
                "CASE WHEN D.UseConvension = 'True' THEN D.ConvensionalName ELSE D.SchClass END + ' ' + R.CATName AS ClassName, " +
                "E.Surname + ' ' + E.FirstName + ' ' + ISNULL(E.MiddleName, ' ') AS ClassTeacher, " +
                "P.PrefixName + REPLACE(STR(S.StudentID, P.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                "S.Surname + ' ' + S.FirstName + ' ' + " + "ISNULL(S.MiddleName, ' ') AS StudentName " +
                "FROM ACDReportCommentCheckPointIGCSE A " +
                "INNER JOIN SETSchSessions B ON B.TermID = A.TermID " +
                "INNER JOIN ADMSchClassList C ON C.ClassID = A.ClassID " +
                "INNER JOIN ADMSchClassGroup AS D ON D.ClassListID = C.ClassListID " +
                "INNER JOIN ADMEmployee E ON E.StaffID = A.ClassTeacherID " +
                "INNER JOIN ADMStudents S ON S.STDID = A.STDID " +
                "INNER JOIN SETPrefix P ON P.PrefixID = S.PrefixID " +
                "INNER JOIN ADMSchClassCategory R ON R.CATID = C.CATID " +
                "WHERE A.CommentID = @CommentID ORDER BY Position;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDReportCommentCheckPointIGCSE>(sql, new { CommentID = id });
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

        public async Task<ACDReportCommentCheckPointIGCSE> AddAsync(ACDReportCommentCheckPointIGCSE entity)
        {
            sql = @"INSERT INTO ACDReportCommentCheckPointIGCSE (TermID, SchSession, ClassID, ClassTeacherID, STDID, Comments, AVGPer, 
                    Position, Grade) OUTPUT INSERTED.CommentID VALUES (@TermID, @SchSession, @ClassID, @ClassTeacherID, @STDID, @Comments,
                    @AVGPer, @Position, @Grade);SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.CommentID = result;
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

        public async Task<ACDReportCommentCheckPointIGCSE> UpdateAsync(int id, ACDReportCommentCheckPointIGCSE entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDReportCommentCheckPointIGCSE SET Comments = @Comments, AVGPer = @AVGPer, Position = @Position, " +
                        "Grade = @Grade WHERE CommentID = @CommentID";
                    break;
                case 2://Update Comment Only
                    sql = "UPDATE ACDReportCommentCheckPointIGCSE SET Comments = @Comments WHERE CommentID = @CommentID";
                    break;
                case 3:
                    sql = "UPDATE ACDReportCommentCheckPointIGCSE SET ClassTeacherID = @ClassTeacherID WHERE ClassID = @ClassID";
                    break;
                case 4:
                    sql = "UPDATE ACDReportCommentCheckPointIGCSE SET Id = @Id WHERE CommentID = @CommentID;";
                    break;
                case 5: //Update Class And Class Teacher On Student Class Change
                    sql = "UPDATE ACDReportCommentCheckPointIGCSE SET ClassID = @ClassID, ClassTeacherID = @ClassTeacherID " +
                        "WHERE STDID = @STDID";
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
            sql = "DELETE FROM ACDReportCommentCheckPointIGCSE WHERE CommentID = @CommentID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { CommentID = id });
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

        public Task<IReadOnlyList<ACDReportCommentCheckPointIGCSE>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }
}
