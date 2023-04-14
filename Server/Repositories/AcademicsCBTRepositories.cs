using WebAppAcademics.Server.Interfaces.Academics.CBT;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.CBT;
using System.Data;
using System.Data.SqlClient;

namespace WebAppAcademics.Server.Repositories
{
    public class ACDCBTExamTypeRepository : IACDCBTExamTypeRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<CBTExamType> _list { get; set; }
        CBTExamType _details = new();

        public ACDCBTExamTypeRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<CBTExamType>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM CBTExamType;";

                            _list = (List<CBTExamType>)await connection.QueryAsync<CBTExamType>(sql, new { });
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

        public async Task<CBTExamType> GetByIdAsync(int id)
        {
            _details = new CBTExamType();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM CBTExamType WHERE ExamTypeID = @ExamTypeID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<CBTExamType>(sql, new { ExamTypeID = id });
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

        public async Task<CBTExamType> AddAsync(CBTExamType entity)
        {
            sql = @"INSERT INTO CBTExamType (ExamType) OUTPUT INSERTED.ExamTypeID VALUES (@ExamType);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.ExamTypeID = result;
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
                
        public async Task<CBTExamType> UpdateAsync(int id, CBTExamType entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE CBTExamType SET ExamType = @ExamType WHERE ExamTypeID = @ExamTypeID;";
                    break;
                case 2:
                    sql = "UPDATE ACDFlags SET Id = @Id WHERE ExamTypeID = @ExamTypeID;";
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
            sql = "DELETE FROM CBTExamType WHERE ExamTypeID = @ExamTypeID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { ExamTypeID = id });
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

        public Task<IReadOnlyList<CBTExamType>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDCBTExamsRepository : IACDCBTExamsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<CBTExams> _list { get; set; }
        CBTExams _details = new();

        public ACDCBTExamsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<CBTExams>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: // Exam List By Term ID
                            sql = "SELECT A.ExamID, A.TermID, A.SchID, A.ClassListID, A.SubjectID, A.ReportTypeID, A.ExamDate, " +
                                "A.ExamTypeID, A.ExamCode, A.ExamName, A.ExamInstruction, A.PassingPercentage, A.FixExamTime, " +
                                "A.ExamTime, A.QTimer, A.ExamTimer, A.AllowCalc, A.ExamPassword, A.ExamDefault, A.StaffID, " +
                                "B.School, C.SchClass, " +
                                "CASE WHEN C.UseConvension = 'True' THEN C.ConvensionalName ELSE C.SchClass END AS ClassName, " +
                                "D.Subject, D.SortID, E.ReportType, F.ExamType, " +
                                "G.Surname + ' ' + SUBSTRING(G.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(G.MiddleName, 0, 2) + '. ', '') AS SubjectTeacher, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.TermID, A.ReportTypeID ORDER BY A.TermID, A.ReportTypeID, D.SortID) AS SN " +
                                "FROM CBTExams A " +
                                "INNER JOIN ADMSchlList B ON B.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup C ON C.ClassListID = A.ClassListID " +
                                "INNER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "INNER JOIN ACDReportType E ON E.ReportTypeID = A.ReportTypeID " +
                                "INNER JOIN CBTExamType F ON F.ExamTypeID = A.ExamTypeID " +
                                "INNER JOIN ADMEmployee G ON G.StaffID = A.StaffID " +
                                "WHERE A.TermID = @TermID ORDER BY A.TermID, D.SortID;";

                            _list = (List<CBTExams>)await connection.QueryAsync<CBTExams>(sql,
                            new
                            {
                                TermID = _switch.TermID
                            });
                            break;
                        case 2: //Exam List For Student CBT Examination Selection
                            sql = "SELECT A.ExamID, A.TermID, A.SchID, A.ClassListID, A.SubjectID, A.ReportTypeID, A.ExamDate, " +
                                "A.ExamTypeID, A.ExamCode, A.ExamName, A.ExamInstruction, A.PassingPercentage, A.FixExamTime, " +
                                "A.ExamTime, A.QTimer, A.ExamTimer, A.AllowCalc, A.ExamPassword, A.ExamDefault, A.StaffID, " +
                                "B.School, C.SchClass, " +
                                "CASE WHEN C.UseConvension = 'True' THEN C.ConvensionalName ELSE C.SchClass END AS ClassName, " +
                                "D.Subject, D.SortID, E.ReportType, F.ExamType, " +
                                "G.Surname + ' ' + SUBSTRING(G.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(G.MiddleName, 0, 2) + '. ', '') AS SubjectTeacher, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.TermID, A.ReportTypeID ORDER BY A.TermID, A.ReportTypeID, D.SortID) AS SN " +
                                "FROM CBTExams A " +
                                "INNER JOIN ADMSchlList B ON B.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup C ON C.ClassListID = A.ClassListID " +
                                "INNER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "INNER JOIN ACDReportType E ON E.ReportTypeID = A.ReportTypeID " +
                                "INNER JOIN CBTExamType F ON F.ExamTypeID = A.ExamTypeID " +
                                "INNER JOIN ADMEmployee G ON G.StaffID = A.StaffID " +
                                "WHERE A.ClassListID = @ClassListID AND A.ExamDefault = @ExamDefault " +
                                "ORDER BY A.TermID, D.SortID;";

                            _list = (List<CBTExams>)await connection.QueryAsync<CBTExams>(sql,
                            new
                            {
                                _switch.ClassListID,
                                ExamDefault = true
                            });
                            break;
                        case 3: //Exam List By Term ID And Active Exam
                            sql = "SELECT A.ExamID, A.TermID, A.SchID, A.ClassListID, A.SubjectID, A.ReportTypeID, A.ExamDate, " +
                                "A.ExamTypeID, A.ExamCode, A.ExamName, A.ExamInstruction, A.PassingPercentage, A.FixExamTime, " +
                                "A.ExamTime, A.QTimer, A.ExamTimer, A.AllowCalc, A.ExamPassword, A.ExamDefault, A.StaffID, " +
                                "B.School, C.SchClass, " +
                                "CASE WHEN C.UseConvension = 'True' THEN C.ConvensionalName ELSE C.SchClass END AS ClassName, " +
                                "D.Subject, D.SortID, E.ReportType, F.ExamType, " +
                                "G.Surname + ' ' + SUBSTRING(G.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(G.MiddleName, 0, 2) + '. ', '') AS SubjectTeacher, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.TermID, A.ReportTypeID ORDER BY A.TermID, A.ReportTypeID, D.SortID) AS SN " +
                                "FROM CBTExams A " +
                                "INNER JOIN ADMSchlList B ON B.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup C ON C.ClassListID = A.ClassListID " +
                                "INNER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "INNER JOIN ACDReportType E ON E.ReportTypeID = A.ReportTypeID " +
                                "INNER JOIN CBTExamType F ON F.ExamTypeID = A.ExamTypeID " +
                                "INNER JOIN ADMEmployee G ON G.StaffID = A.StaffID " +
                                "WHERE A.TermID = @TermID AND A.ExamDefault = @ExamDefault " +
                                "ORDER BY A.TermID, D.SortID;";

                            _list = (List<CBTExams>)await connection.QueryAsync<CBTExams>(sql,
                            new
                            {
                                TermID = _switch.TermID,
                                ExamDefault = true
                            });
                            break;
                        case 4: //Exam List By Term ID And Staff ID
                            sql = "SELECT A.ExamID, A.TermID, A.SchID, A.ClassListID, A.SubjectID, A.ReportTypeID, A.ExamDate, " +
                                "A.ExamTypeID, A.ExamCode, A.ExamName, A.ExamInstruction, A.PassingPercentage, A.FixExamTime, " +
                                "A.ExamTime, A.QTimer, A.ExamTimer, A.AllowCalc, A.ExamPassword, A.ExamDefault, A.StaffID, " +
                                "B.School, C.SchClass, " +
                                "CASE WHEN C.UseConvension = 'True' THEN C.ConvensionalName ELSE C.SchClass END AS ClassName, " +
                                "D.Subject, D.SortID, E.ReportType, F.ExamType, " +
                                "G.Surname + ' ' + SUBSTRING(G.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(G.MiddleName, 0, 2) + '. ', '') AS SubjectTeacher, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.TermID, A.ReportTypeID ORDER BY A.TermID, A.ReportTypeID, D.SortID) AS SN " +
                                "FROM CBTExams A " +
                                "INNER JOIN ADMSchlList B ON B.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup C ON C.ClassListID = A.ClassListID " +
                                "INNER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "INNER JOIN ACDReportType E ON E.ReportTypeID = A.ReportTypeID " +
                                "INNER JOIN CBTExamType F ON F.ExamTypeID = A.ExamTypeID " +
                                "INNER JOIN ADMEmployee G ON G.StaffID = A.StaffID " +
                                "WHERE A.TermID = @TermID AND A.StaffID = @StaffID " +
                                "ORDER BY A.TermID, D.SortID;";

                            _list = (List<CBTExams>)await connection.QueryAsync<CBTExams>(sql,
                            new
                            {
                                TermID = _switch.TermID,
                                StaffID = _switch.StaffID
                            });
                            break;
                        case 5: //Exam List By Term ID, Class List  ID AND Report Type ID
                            sql = "SELECT A.ExamID, A.TermID, A.SchID, A.ClassListID, A.SubjectID, A.ReportTypeID, A.ExamDate, " +
                                "A.ExamTypeID, A.ExamCode, A.ExamName, A.ExamInstruction, A.PassingPercentage, A.FixExamTime, " +
                                "A.ExamTime, A.QTimer, A.ExamTimer, A.AllowCalc, A.ExamPassword, A.ExamDefault, A.StaffID, " +
                                "B.School, C.SchClass, " +
                                "CASE WHEN C.UseConvension = 'True' THEN C.ConvensionalName ELSE C.SchClass END AS ClassName, " +
                                "D.Subject, D.SortID, E.ReportType, F.ExamType, " +
                                "G.Surname + ' ' + SUBSTRING(G.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(G.MiddleName, 0, 2) + '. ', '') AS SubjectTeacher, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.TermID, A.ReportTypeID ORDER BY A.TermID, A.ReportTypeID, D.SortID) AS SN " +
                                "FROM CBTExams A " +
                                "INNER JOIN ADMSchlList B ON B.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup C ON C.ClassListID = A.ClassListID " +
                                "INNER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "INNER JOIN ACDReportType E ON E.ReportTypeID = A.ReportTypeID " +
                                "INNER JOIN CBTExamType F ON F.ExamTypeID = A.ExamTypeID " +
                                "INNER JOIN ADMEmployee G ON G.StaffID = A.StaffID " +
                                "WHERE A.TermID = @TermID AND A.ClassListID = @ClassListID AND " +
                                "A.ReportTypeID = @ReportTypeID " +
                                "ORDER BY A.TermID, D.SortID;";

                            _list = (List<CBTExams>)await connection.QueryAsync<CBTExams>(sql,
                            new
                            {
                                TermID = _switch.TermID,
                                ClassListID = _switch.ClassListID,
                                ReportTypeID = _switch.StaffID
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

        public async Task<CBTExams> GetByIdAsync(int id)
        {
            _details = new CBTExams();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.ExamID, A.TermID, A.SchID, A.ClassListID, A.SubjectID, A.ReportTypeID, A.ExamDate, " +
                "A.ExamTypeID, A.ExamCode, A.ExamName, A.ExamInstruction, A.PassingPercentage, A.FixExamTime, " +
                "A.ExamTime, A.QTimer, A.ExamTimer, A.AllowCalc, A.ExamPassword, A.ExamDefault, A.StaffID, " +
                "B.School, C.SchClass, " +
                "CASE WHEN C.UseConvension = 'True' THEN C.ConvensionalName ELSE C.SchClass END AS ClassName, " +
                "D.Subject, D.SortID, E.ReportType, F.ExamType, " +
                "G.Surname + ' ' + SUBSTRING(G.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(G.MiddleName, 0, 2) + '. ', '') AS SubjectTeacher, " +
                "ROW_NUMBER() OVER (PARTITION BY A.TermID ORDER BY A.TermID, D.SortID) AS SN " +
                "FROM CBTExams A " +
                "INNER JOIN ADMSchlList B ON B.SchID = A.SchID " +
                "INNER JOIN ADMSchClassGroup C ON C.ClassListID = A.ClassListID " +
                "INNER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                "INNER JOIN ACDReportType E ON E.ReportTypeID = A.ReportTypeID " +
                "INNER JOIN CBTExamType F ON F.ExamTypeID = A.ExamTypeID " +
                "INNER JOIN ADMEmployee G ON G.StaffID = A.StaffID " +
                "WHERE A.ExamID = @ExamID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<CBTExams>(sql, new { ExamID = id });
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

        public async Task<CBTExams> AddAsync(CBTExams entity)
        {
            sql = @"INSERT INTO CBTExams (TermID, SchID, ClassListID, SubjectID, ExamDate, ReportTypeID, ExamTypeID, 
                    ExamCode, ExamName, ExamInstruction, PassingPercentage, FixExamTime, ExamTime, QTimer, ExamTimer, 
                    ExamPassword, ExamDefault, StaffID) OUTPUT INSERTED.ExamID VALUES (@TermID, @SchID, @ClassListID, 
                    @SubjectID, @ExamDate, @ReportTypeID, @ExamTypeID, @ExamCode, @ExamName, @ExamInstruction, 
                    @PassingPercentage, @FixExamTime, @ExamTime, @QTimer, @ExamTimer, @ExamPassword, @ExamDefault, @StaffID);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.ExamID = result;
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

        public async Task<CBTExams> UpdateAsync(int id, CBTExams entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1: //Update Exam
                    sql = "UPDATE CBTExams SET ExamDate = @ExamDate, ExamCode = @ExamCode, ExamName = @ExamName, " +
                        "ExamInstruction = @ExamInstruction, PassingPercentage = @PassingPercentage, " +
                        "ExamTime = @ExamTime, ExamDefault = @ExamDefault, SchID = @SchID, ClassListID = @ClassListID WHERE ExamID = @ExamID;";
                    break;
                case 2: //Set Default Exam
                    sql = "UPDATE CBTExams SET ExamDefault = @ExamDefault WHERE ExamID = @ExamID;";
                    break;
                case 3: //Update Staff ID When Subject Teacher Change
                    sql = "UPDATE CBTExams SET StaffID = @StaffID WHERE TermID = @TermID AND SchID = @SchID AND SubjectID = @SubjectID;";
                    break;
                case 4: //Update Exam
                    sql = "UPDATE CBTExams SET ExamCode = @ExamCode, ExamName = @ExamName, PassingPercentage = @PassingPercentage, " +
                        "ExamTime = @ExamTime, ExamDefault = @ExamDefault WHERE ExamID = @ExamID;";
                    break;
                case 5:
                    sql = "UPDATE ACDFlags SET Id = @Id WHERE ExamID = @ExamID;";
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
            sql = "DELETE FROM CBTExams WHERE ExamID = @ExamID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { ExamID = id });
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

        public Task<int> CountAsync(SwitchModel _switchdid)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<CBTExams>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDCBTExamQuestionTypeRepository : IACDCBTExamQuestionTypeRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<CBTQuestionType> _list { get; set; }
        CBTQuestionType _details = new();

        public ACDCBTExamQuestionTypeRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<CBTQuestionType>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: //All Question Type
                            sql = "SELECT * FROM CBTQuestionType;";

                            _list = (List<CBTQuestionType>)await connection.QueryAsync<CBTQuestionType>(sql, new { });
                            break;
                        case 2: //Selected Question Type
                            sql = "SELECT * FROM CBTQuestionType WHERE QTypeStatus = @QTypeStatus;";

                            _list = (List<CBTQuestionType>)await connection.QueryAsync<CBTQuestionType>(sql, 
                                new 
                                {
                                    QTypeStatus = true
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

        public async Task<CBTQuestionType> GetByIdAsync(int id)
        {
            _details = new CBTQuestionType();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM CBTQuestionType WHERE QTypeID = @QTypeID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<CBTQuestionType>(sql, new { QTypeID = id });
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

        public async Task<CBTQuestionType> AddAsync(CBTQuestionType entity)
        {
            entity.QTypeStatus = true;

            sql = @"INSERT INTO CBTQuestionType (QType, QTypeStatus) OUTPUT INSERTED.QTypeID VALUES (@QType, @QTypeStatus);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.QTypeID = result;
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

        public async Task<CBTQuestionType> UpdateAsync(int id, CBTQuestionType entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE CBTQuestionType SET QType = @QType WHERE QTypeID = @QTypeID;";
                    break;
                case 2:
                    sql = "UPDATE CBTQuestionType SET QTypeStatus = @QTypeStatus WHERE QTypeID = @QTypeID;";
                    break;
                case 3:
                    sql = "UPDATE CBTQuestionType SET Id = @Id WHERE QTypeID = @QTypeID;";
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
            sql = "DELETE FROM CBTQuestionType WHERE QTypeID = @QTypeID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { QTypeID = id });
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

        public Task<int> CountAsync(SwitchModel _switchdid)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<CBTQuestionType>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDCBTExamQuestionsRepository : IACDCBTExamQuestionsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<CBTQuestions> _list { get; set; }
        CBTQuestions _details = new();

        public ACDCBTExamQuestionsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<CBTQuestions>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: 
                            sql = "SELECT A.QID, A.ExamID, A.QTypeID, A.QNo, A.Section, A.Question, A.Equation, A.QPoints, " +
                                "A.QTime, A.SImage, A.QImage, A.NAns, B.ExamCode, B.ExamName, B.ExamInstruction, C.QType, " +
                                "CAST(A.QNo AS nvarchar(5)) + '.     ' + A.Question AS CurrentQuestion " +
                                "FROM CBTQuestions A INNER JOIN CBTExams B ON B.ExamID = A.ExamID " +
                                "INNER JOIN CBTQuestionType C ON C.QTypeID = A.QTypeID " +
                                "WHERE DeleteQuestion = 'False' AND A.ExamID = @ExamID;";

                            _list = (List<CBTQuestions>)await connection.QueryAsync<CBTQuestions>(sql,
                            new
                            {
                                ExamID = _switch.ExamID
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

        public async Task<CBTQuestions> GetByIdAsync(int id)
        {
            _details = new CBTQuestions();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.QID, A.ExamID, A.QTypeID, A.QNo, A.Section, A.Question, A.Equation, A.QPoints, " +
                    "A.QTime, A.SImage, A.QImage, A.NAns, B.ExamCode, B.ExamName, B.ExamInstruction, C.QType, " +
                    "CAST(A.QNo AS nvarchar(5)) + '.     ' + A.Question AS CurrentQuestion " +
                    "FROM CBTQuestions A INNER JOIN CBTExams B ON B.ExamID = A.ExamID " +
                    "INNER JOIN CBTQuestionType C ON C.QTypeID = A.QTypeID " +
                    "WHERE A.QID = @QID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<CBTQuestions>(sql, new { QID = id });
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

        public async Task<CBTQuestions> AddAsync(CBTQuestions entity)
        {
            entity.DeleteQuestion = false;
           
            sql = @"INSERT INTO CBTQuestions (ExamID, QTypeID, QNo, Section, Question, Equation, QPoints, QTime, SImage, QImage, NAns) 
                    OUTPUT INSERTED.QID VALUES (@ExamID, @QTypeID, @QNo, @Section, @Question, @Equation, @QPoints, @QTime, @SImage,
                    @QImage, @NAns);SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.QID = result;
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

        public async Task<CBTQuestions> UpdateAsync(int id, CBTQuestions entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE CBTQuestions SET Section = @Section, Question = @Question, Equation = @Equation, " +
                        "SImage = @SImage, QImage = @QImage WHERE QID = @QID;";
                    break;
                case 2: //Delete
                    sql = "UPDATE CBTQuestions SET DeleteQuestion = @DeleteQuestion WHERE QID = @QID";
                    break;
                case 3:
                    sql = "UPDATE CBTQuestions SET Id = @Id WHERE QID = @QID;";
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
            sql = "DELETE FROM CBTQuestions WHERE QID = @QID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { QID = id });
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

        public Task<int> CountAsync(SwitchModel _switchdid)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<CBTQuestions>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDCBTExamAnswersRepository : IACDCBTExamAnswersRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<CBTAnswers> _list { get; set; }
        CBTAnswers _details = new();

        public ACDCBTExamAnswersRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<CBTAnswers>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT A.AnsID, A.ExamID, A.QTypeID, A.QID, A.AnsLetter, A.Answers, A.Equation, " +
                                "A.AnsImage, A.CorrectAns,  A.AnsLetter + '.     ' +  A.Answers AS Choices " +
                                "FROM CBTAnswers A INNER JOIN CBTQuestions B ON B.QID = A.QID " +
                                "WHERE DeleteAnswers = 'False' AND A.ExamID = @ExamID;";

                            _list = (List<CBTAnswers>)await connection.QueryAsync<CBTAnswers>(sql,
                                new
                                {
                                    ExamID = _switch.ExamID
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

        public async Task<CBTAnswers> GetByIdAsync(int id)
        {
            _details = new CBTAnswers();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.AnsID, A.ExamID, A.QTypeID, A.QID, A.AnsLetter, A.Answers, A.Equation, " +
                    "A.AnsImage, A.CorrectAns,  A.AnsLetter + '.     ' +  A.Answers AS Choices " +
                    "FROM CBTAnswers A INNER JOIN CBTQuestions B ON B.QID = A.QID " +
                    "WHERE DeleteAnswers = 'False' AND A.AnsID = @AnsID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<CBTAnswers>(sql, new { AnsID = id });
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

        public async Task<CBTAnswers> AddAsync(CBTAnswers entity)
        {
            entity.DeleteAnswers = false;

            sql = @"INSERT INTO CBTAnswers (ExamID, QTypeID, QID, AnsLetter, Answers, Equation, AnsImage, CorrectAns) 
                    OUTPUT INSERTED.AnsID VALUES (@ExamID, @QTypeID, @QID, @AnsLetter, @Answers, @Equation, @AnsImage, 
                    @CorrectAns);";


            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.AnsID = result;
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

        public async Task<CBTAnswers> UpdateAsync(int id, CBTAnswers entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE CBTAnswers SET Answers = @Answers, Equation = @Equation, AnsImage = @AnsImage, " +
                        "CorrectAns = @CorrectAns WHERE AnsID = @AnsID;";
                    break;
                case 2: //Delete
                    sql = "UPDATE CBTAnswers SET DeleteAnswers = @DeleteAnswers WHERE AnsID = @AnsID;";
                    break;
                case 3:
                    sql = "UPDATE CBTAnswers SET Id = @Id WHERE AnsID = @AnsID;";
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
            sql = "DELETE FROM CBTAnswers WHERE AnsID = @AnsID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { AnsID = id });
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

        public Task<IReadOnlyList<CBTAnswers>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDCBTExamStudentAnswersRepository : IACDCBTExamStudentAnswersRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<CBTStudentAnswers> _list { get; set; }
        CBTStudentAnswers _details = new();

        public ACDCBTExamStudentAnswersRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<CBTStudentAnswers>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT A.AnsID, A.StudentAnswerID, A.STDID, A.ExamID, A.QTypeID, A.QID, A.QNo, A.Answer, " +
                                "A.MultipleAnswer, A.QAnswered, A.Correct, A.QPoints, A.StudentScoreID, A.CBTToUse, " +
                                "B.Question FROM CBTStudentAnswers A " +
                                "INNER JOIN CBTQuestions B ON B.QID = A.QID " +
                                "WHERE A.ExamID = @ExamID AND A.STDID = @STDID AND A.CBTToUse  = @CBTToUse;";

                            _list = (List<CBTStudentAnswers>)await connection.QueryAsync<CBTStudentAnswers>(sql,
                            new
                            {
                                ExamID = _switch.ExamID,
                                STDID = _switch.STDID,                                
                                CBTToUse = _switch.CBTToUse
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

        public async Task<CBTStudentAnswers> GetByIdAsync(int id)
        {
            _details = new CBTStudentAnswers();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.StudentAnswerID, A.STDID, A.ExamID, A.QTypeID, A.QID, A.QNo, A.Answer, " +
                    "A.MultipleAnswer, A.QAnswered, A.Correct, A.QPoints, A.StudentScoreID, A.CBTToUse, " +
                    "B.Question FROM CBTStudentAnswers A " +
                    "INNER JOIN CBTQuestions B ON B.QID = A.QID " +
                    "WHERE A.StudentAnswerID = @StudentAnswerID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<CBTStudentAnswers>(sql, new { StudentAnswerID = id });
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

        public async Task<CBTStudentAnswers> AddAsync(CBTStudentAnswers entity)
        {
            sql = @"INSERT INTO CBTStudentAnswers (STDID, ExamID, QTypeID, QID, QNo, Answer, QAnswered, Correct, QPoints, 
                    StudentScoreID, AnsID, CBTToUse) OUTPUT INSERTED.StudentAnswerID VALUES (@STDID, @ExamID, @QTypeID, 
                    @QID, @QNo, @Answer, @QAnswered, @Correct, @QPoints, @StudentScoreID, @AnsID, @CBTToUse);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.StudentAnswerID = result;
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

        public async Task<CBTStudentAnswers> UpdateAsync(int id, CBTStudentAnswers entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1: //Reset CBT To Use
                    sql = "UPDATE CBTStudentAnswers SET CBTToUse = @CBTToUse WHERE STDID = @STDID AND ExamID = @ExamID;";
                    break;
                case 2: //Update Student Answers Score ID
                    sql = "UPDATE CBTStudentAnswers SET StudentScoreID = @StudentScoreID " +
                            "WHERE STDID = @STDID AND ExamID = @ExamID AND CBTToUse = @CBTToUse;";
                    break;
                case 3: //Reset Selected Student Answers
                    sql = "UPDATE CBTStudentAnswers SET Correct = @Correct " +
                            "WHERE STDID = @STDID AND ExamID = @ExamID AND CBTToUse = @CBTToUse;";
                    break;
                case 4: //Update Selected Student Answers
                    sql = "UPDATE CBTStudentAnswers SET Correct = @Correct WHERE StudentAnswerID = @StudentAnswerID;";
                    break;
                case 5: //Update Student NAswer ID
                    sql = "UPDATE CBTStudentAnswers SET AnsID = @AnsID WHERE StudentAnswerID = @StudentAnswerID;";
                    break;
                case 6:
                    sql = "UPDATE CBTStudentAnswers SET Id = @Id WHERE StudentAnswerID = @StudentAnswerID;";
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
            sql = "DELETE FROM CBTStudentAnswers WHERE StudentAnswerID = @StudentAnswerID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { StudentAnswerID = id });
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

        public Task<IReadOnlyList<CBTStudentAnswers>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDCBTExamStudentScoreRepository : IACDCBTExamStudentScoreRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<CBTStudentScores> _list { get; set; }
        CBTStudentScores _details = new();

        public ACDCBTExamStudentScoreRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<CBTStudentScores>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT StudentScoreID, A.ExamID, A.STDID, B.ExamDate, A.NQuestions, A.NUnAnsQuestions, " +
                                "A.NWrongAns, A.NCorrectAns, A.ScorePercentage, A.QTimer, A.ExamTimer, A.TimeAllocated, " +
                                "A.TimeUsed, A.CBTToUse, A.TimeStamp, B.PassingPercentage, B.SubjectID, C.Subject, " +
                                "E.PrefixName + REPLACE(STR(D.StudentID, E.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(SUBSTRING(D.MiddleName, 0, 2) + '. ', '') AS StudentName, " +
                                "D.studentPhoto, B.TermID, B.ReportTypeID, D.ClassID, " +
                                "(CASE WHEN G.UseConvension = 'True' THEN G.ConvensionalName ELSE G.SchClass END) + ' ' + H.CATName AS ClassName " +
                                "FROM CBTStudentScores A " +
                                "LEFT OUTER JOIN CBTExams B ON B.ExamID = A.ExamID " +
                                "LEFT OUTER JOIN ACDSubjects C ON C.SubjectID = B.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents D ON D.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix E ON E.PrefixID = D.PrefixID " +
                                "LEFT OUTER JOIN ADMSchClassList AS F ON F.ClassID = D.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS G ON G.ClassListID = F.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassCategory AS H ON H.CATID = F.CATID " +
                                "WHERE A.ExamID = @ExamID AND A.CBTToUse = @CBTToUse;";

                            _list = (List<CBTStudentScores>)await connection.QueryAsync<CBTStudentScores>(sql,
                               new
                               {
                                   ExamID = _switch.ExamID,
                                   CBTToUse = _switch.CBTToUse
                               });
                            break;
                        case 2:
                            sql = "SELECT StudentScoreID, A.ExamID, A.STDID, B.ExamDate, A.NQuestions, A.NUnAnsQuestions, " +
                                "A.NWrongAns, A.NCorrectAns, A.ScorePercentage, A.QTimer, A.ExamTimer, A.TimeAllocated, " +
                                "A.TimeUsed, A.CBTToUse, A.TimeStamp, B.PassingPercentage, B.SubjectID, C.Subject, " +
                                "E.PrefixName + REPLACE(STR(D.StudentID, E.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "E.PrefixName + REPLACE(STR(D.StudentID, E.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(SUBSTRING(D.MiddleName, 0, 2) + '. ', '') AS StudentName, " +
                                "D.studentPhoto, B.TermID, B.ReportTypeID, D.ClassID, " +
                                "(CASE WHEN G.UseConvension = 'True' THEN G.ConvensionalName ELSE G.SchClass END) + ' ' + H.CATName AS ClassName " +
                                "FROM CBTStudentScores A " +
                                "LEFT OUTER JOIN CBTExams B ON B.ExamID = A.ExamID " +
                                "LEFT OUTER JOIN ACDSubjects C ON C.SubjectID = B.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents D ON D.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix E ON E.PrefixID = D.PrefixID " +
                                "LEFT OUTER JOIN ADMSchClassList AS F ON F.ClassID = D.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassGroup AS G ON G.ClassListID = F.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassCategory AS H ON H.CATID = F.CATID " +
                                "WHERE A.ExamID = @ExamID AND A.STDID = @STDID AND A.CBTToUse = @CBTToUse;";

                            _list = (List<CBTStudentScores>)await connection.QueryAsync<CBTStudentScores>(sql,
                              new
                              {
                                  ExamID = _switch.ExamID,
                                  STDID = _switch.STDID,
                                  CBTToUse = _switch.CBTToUse
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

        public async Task<CBTStudentScores> GetByIdAsync(int id)
        {
            _details = new CBTStudentScores();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT StudentScoreID, A.ExamID, A.STDID, B.ExamDate, A.NQuestions, A.NUnAnsQuestions, " +
                "A.NWrongAns, A.NCorrectAns, A.ScorePercentage, A.QTimer, A.ExamTimer, A.TimeAllocated, " +
                "A.TimeUsed, A.CBTToUse, A.TimeStamp, B.PassingPercentage, B.SubjectID, C.Subject, " +
                "E.PrefixName + REPLACE(STR(D.StudentID, E.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                "D.Surname + ' ' + D.FirstName + ISNULL(D.MiddleName, ' ') AS StudentName, D.studentPhoto, " +
                "B.TermID, B.ReportTypeID, D.ClassID " +
                "FROM CBTStudentScores A " +
                "INNER JOIN CBTExams B ON B.ExamID = A.ExamID " +
                "LEFT OUTER JOIN ACDSubjects C ON C.SubjectID = B.SubjectID " +
                "INNER JOIN ADMStudents D ON D.STDID = A.STDID " +
                "LEFT OUTER JOIN SETPrefix E ON E.PrefixID = D.PrefixID " +
                "WHERE A.StudentScoreID = @StudentScoreID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<CBTStudentScores>(sql, new { StudentScoreID = id });
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

        public async Task<CBTStudentScores> AddAsync(CBTStudentScores entity)
        {
            entity.CBTToUse = true;

            sql = @"INSERT CBTStudentScores (ExamID, STDID, ExamDate, NQuestions, NUnAnsQuestions, NWrongAns, NCorrectAns, 
                    ScorePercentage, QTimer, ExamTimer, TimeAllocated, TimeUsed, CBTToUse) OUTPUT INSERTED.StudentScoreID 
                    VALUES (@ExamID, @STDID, @ExamDate, @NQuestions, @NUnAnsQuestions, @NWrongAns, @NCorrectAns, 
                    @ScorePercentage, @QTimer, @ExamTimer, @TimeAllocated, @TimeUsed, @CBTToUse);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.StudentScoreID = result;
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

        public async Task<CBTStudentScores> UpdateAsync(int id, CBTStudentScores entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1: //Reset CBT To Use
                    sql = "UPDATE CBTStudentScores SET CBTToUse = @CBTToUse WHERE ExamID = @ExamID AND STDID = @STDID";
                    break;
                case 2: //Update Student Score
                    sql = "UPDATE CBTStudentScores SET NWrongAns = @NWrongAns, NCorrectAns = @NCorrectAns, ScorePercentage = @ScorePercentage " +
                        "WHERE StudentScoreID = @StudentScoreID";
                    break;
                case 3:
                    sql = "UPDATE CBTStudentScores SET Id = @Id WHERE StudentScoreID = @StudentScoreID;";
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
            sql = "DELETE FROM CBTStudentScores WHERE StudentScoreID = @StudentScoreID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { StudentScoreID = id });
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

        public Task<IReadOnlyList<CBTStudentScores>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDCBTExamLatexRepository : IACDCBTExamLatexRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<CBTLatex> _list { get; set; }
        CBTLatex _details = new();

        public ACDCBTExamLatexRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<CBTLatex>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM CBTLatex;";

                            _list = (List<CBTLatex>)await connection.QueryAsync<CBTLatex>(sql, new { });
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

        public async Task<CBTLatex> GetByIdAsync(int id)
        {
            _details = new CBTLatex();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM CBTLatex WHERE LatexID = @LatexID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<CBTLatex>(sql, new { LatexID = id });
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

        public async Task<CBTLatex> AddAsync(CBTLatex entity)
        {
            sql = @"INSERT CBTLatex (LatexGroup, LatexSymbol, Description, Package) OUTPUT INSERTED.LatexID 
                    VALUES (@LatexGroup, @LatexSymbol, @Description, @Package);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.LatexID = result;
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

        public async Task<CBTLatex> UpdateAsync(int id, CBTLatex entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1: //Reset CBT To Use
                    sql = "UPDATE CBTLatex SET LatexGroup = @LatexGroup, LatexSymbol = @LatexSymbol, " +
                        "Description = @Description, Package = @Package WHERE LatexID = @LatexID";
                    break;
                case 2:
                    sql = "UPDATE CBTLatex SET Id = @Id WHERE LatexID = @LatexID;";
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
            sql = "DELETE FROM CBTLatex WHERE LatexID = @LatexID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { LatexID = id });
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

        public Task<IReadOnlyList<CBTLatex>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class CBTConnectionInfoRepository : ICBTConnectionInfoRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<CBTConnectionInfo> _list { get; set; }
        CBTConnectionInfo _details = new();

        public CBTConnectionInfoRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<CBTConnectionInfo>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM CBTConnectionInfo;";

                            _list = (List<CBTConnectionInfo>)await connection.QueryAsync<CBTConnectionInfo>(sql, new { });
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

        public async Task<CBTConnectionInfo> GetByIdAsync(int id)
        {
            _details = new CBTConnectionInfo();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM CBTConnectionInfo WHERE ConnectionID = @ConnectionID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<CBTConnectionInfo>(sql, new { ConnectionID = id });
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

        public Task<CBTConnectionInfo> AddAsync(CBTConnectionInfo entity)
        {
            throw new NotImplementedException();
        }

        public async Task<CBTConnectionInfo> UpdateAsync(int id, CBTConnectionInfo entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE CBTConnectionInfo SET ConnectionValue = @ConnectionValue WHERE ConnectionID = @ConnectionID;";
                    break;
                case 2:
                    sql = "UPDATE CBTConnectionInfo SET Id = @Id WHERE ConnectionID = @ConnectionID;";
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

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(SwitchModel _switchd)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<CBTConnectionInfo>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class CBTExamTakenFlagsRepository : ICBTExamTakenFlagsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<CBTExamTakenFlags> _list { get; set; }
        CBTExamTakenFlags _details = new();

        public CBTExamTakenFlagsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<CBTExamTakenFlags>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT A.FlagID, A.STDID, A.ExamID, A.TermID, A.Flag, " +
                                "E.PrefixName + REPLACE(STR(D.StudentID, E.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(SUBSTRING(D.MiddleName, 0, 2) + '. ', '') AS StudentName, B.SubjectID, C.Subject " +
                                "FROM CBTExamTakenFlags A " +
                                "LEFT OUTER JOIN CBTExams B ON B.ExamID = A.ExamID " +
                                "LEFT OUTER JOIN ACDSubjects C ON C.SubjectID = B.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents D ON D.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix E ON E.PrefixID = D.PrefixID " +
                                "LEFT OUTER JOIN ADMSchClassList AS F ON F.ClassID = D.ClassID " +
                                "WHERE A.TermID = @TermID AND D.ClassID = @ClassID;";

                            _list = (List<CBTExamTakenFlags>)await connection.QueryAsync<CBTExamTakenFlags>(sql,
                               new
                               {
                                   TermID = _switch.TermID,
                                   ClassID = _switch.ClassID
                               });
                            break;
                        case 2:
                            sql = "SELECT A.FlagID, A.STDID, A.ExamID, A.Flag, E.PrefixName + REPLACE(STR(D.StudentID, E.PrefixDigits), SPACE(1), '0') AS AdmissionNo, " +
                                "D.Surname + ' ' + D.FirstName + ' ' + ISNULL(SUBSTRING(D.MiddleName, 0, 2) + '. ', '') AS StudentName, B.SubjectID, C.Subject " +
                                "FROM CBTExamTakenFlags A " +
                                "LEFT OUTER JOIN CBTExams B ON B.ExamID = A.ExamID " +
                                "LEFT OUTER JOIN ACDSubjects C ON C.SubjectID = B.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents D ON D.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix E ON E.PrefixID = D.PrefixID " +
                                "LEFT OUTER JOIN ADMSchClassList AS F ON F.ClassID = D.ClassID " +
                                "WHERE A.TermID = @TermID AND A.STDID = @STDID;";

                            _list = (List<CBTExamTakenFlags>)await connection.QueryAsync<CBTExamTakenFlags>(sql,
                               new
                               {
                                   TermID = _switch.TermID,
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

        public async Task<CBTExamTakenFlags> GetByIdAsync(int id)
        {
            _details = new CBTExamTakenFlags();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM CBTExamTakenFlags WHERE FlagID = @FlagID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<CBTExamTakenFlags>(sql, new { FlagID = id });
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

        public async Task<CBTExamTakenFlags> AddAsync(CBTExamTakenFlags entity)
        {
            sql = @"INSERT INTO CBTExamTakenFlags (STDID, ExamID, TermID, Flag) OUTPUT INSERTED.FlagID VALUES (@STDID, @ExamID, @TermID, @Flag);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.FlagID = result;
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

        public async Task<CBTExamTakenFlags> UpdateAsync(int id, CBTExamTakenFlags entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1: //Update Exam
                    sql = "UPDATE CBTExamTakenFlags SET Flag = @Flag WHERE FlagID = @FlagID;";
                    break;
                case 2: //Update Exam
                    sql = "UPDATE CBTExamTakenFlags SET Flag = @Flag WHERE FlagID = @FlagID;";
                    break;
                case 3:
                    sql = "UPDATE CBTExamTakenFlags SET Id = @Id WHERE FlagID = @FlagID;";
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

        public Task<int> CountAsync(SwitchModel _switchd)
        {
            throw new NotImplementedException();
        }


        public async Task<IReadOnlyList<CBTExamTakenFlags>> SearchAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM CBTExamTakenFlags WHERE TermID = @TermID AND STDID = @STDID AND ExamID = @ExamID;";

                            _list = (List<CBTExamTakenFlags>)await connection.QueryAsync<CBTExamTakenFlags>(sql,
                                    new
                                    {
                                        TermID = _switch.TermID,
                                        STDID = _switch.STDID,
                                        ExamID = _switch.ExamID
                                    });
                            break;
                        case 2:
                            sql = "SELECT * FROM CBTExamTakenFlags WHERE TermID = @TermID AND STDID = @STDID AND Flag = @Flag;";

                            _list = (List<CBTExamTakenFlags>)await connection.QueryAsync<CBTExamTakenFlags>(sql,
                                    new
                                    {
                                        TermID = _switch.TermID,
                                        STDID = _switch.STDID,
                                        Flag = _switch.Flag
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


        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

    }

}
