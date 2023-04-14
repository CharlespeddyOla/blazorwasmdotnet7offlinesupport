using WebAppAcademics.Server.Interfaces.Academics.Exam;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;
using System.Data;
using System.Data.SqlClient;

namespace WebAppAcademics.Server.Repositories
{
    public class ACDResultsCognitiveRepository : IACDResultsCognitiveRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDStudentsResultCognitive> _list { get; set; }
        ACDStudentsResultCognitive _details = new();

        public ACDResultsCognitiveRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDStudentsResultCognitive>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ACDStudentsResultCognitive;";

                            _list = (List<ACDStudentsResultCognitive>)await connection.QueryAsync<ACDStudentsResultCognitive>(sql,
                                       new
                                       {                                          
                                       });
                            break;
                        default:
                            sql = "SELECT * FROM ACDStudentsResultCognitive WHERE STDID = @STDID;";

                            _list = (List<ACDStudentsResultCognitive>)await connection.QueryAsync<ACDStudentsResultCognitive>(sql,
                                    new
                                    {
                                        STDID = _switch.SwitchID
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

        public async Task<ACDStudentsResultCognitive> GetByIdAsync(int id)
        {
            _details = new ACDStudentsResultCognitive();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ACDStudentsResultCognitive WHERE STDID = @STDID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDStudentsResultCognitive>(sql, new { STDID = id });
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

        public async Task<ACDStudentsResultCognitive> AddAsync(ACDStudentsResultCognitive entity)
        {
            sql = @"INSERT INTO ACDStudentsResultCognitive (STDID, ClassID, SubjectID, SubjectCode, Subject, CA1, CA2, CA3, CA, Exam, 
                TotalMark, Grade, Remarks, POS, MaxMark, MinMark, ClassAvg, FTerm, STerm, TTerm, StudentPhoto, StudentNo, FullName, 
                ClassTeacher, YouthClub, YouthRole, Attendance, DaysAbsent, NextTermBegins, NextTermEnds, ClassName, No_In_Class, No_Of_Sbj, 
                AVGPer, Position, Age, Gender, OverAllScore, Comments_Teacher, ClassTeacherSign, Comments_Principal, PrincipalSign, 
                AcademicSession, CurrentTerm, AlphabetID, SchName, SchSlogan, SchAddress, SchAddressLine2, SchPhones, SchEmails, SchWebsites, 
                SchLogo, PMark_Exam, SortID, SubjectTeacher) VALUES (@STDID, @ClassID, @SubjectID, @SubjectCode, @Subject, @CA1, @CA2, @CA3, @CA, @Exam, 
                @TotalMark, @Grade, @Remarks, @POS, @MaxMark, @MinMark, @ClassAvg, @FTerm, @STerm, @TTerm, @StudentPhoto, @StudentNo, 
                @FullName, @ClassTeacher, @YouthClub, @YouthRole, @Attendance, @DaysAbsent, @NextTermBegins, @NextTermEnds, @ClassName, 
                @No_In_Class, @No_Of_Sbj, @AVGPer, @Position, @Age, @Gender, @OverAllScore, @Comments_Teacher, @ClassTeacherSign, 
                @Comments_Principal, @PrincipalSign, @AcademicSession, @CurrentTerm, @AlphabetID, @SchName, @SchSlogan, @SchAddress,
                @SchAddressLine2, @SchPhones, @SchEmails, @SchWebsites, @SchLogo, @PMark_Exam, @SortID, @SubjectTeacher);SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
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

        public Task<ACDStudentsResultCognitive> UpdateAsync(int id, ACDStudentsResultCognitive entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            sql = "DELETE FROM ACDStudentsResultCognitive";

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

        public async Task<int> CountAsync(SwitchModel _switchd)
        {
            int recordCount = 0;

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                switch(_switchd.SwitchID)
                {
                    case 1:
                        sql = "SELECT COUNT(*) FROM ACDStudentsResultCognitive;";
                        var resultMany = await connection.ExecuteScalarAsync<int>(sql, new { });
                        recordCount = resultMany;
                        break;
                    default:
                        sql = "SELECT COUNT(*) FROM ACDStudentsResultCognitive WHERE STDID = @STDID;";
                        var resultSingle = await connection.ExecuteScalarAsync<int>(sql, 
                            new 
                            {
                                _switchd.STDID
                            });
                        recordCount = resultSingle;
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

            return recordCount;
        }

        public Task<IReadOnlyList<ACDStudentsResultCognitive>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDResultsOtherMarksRepository : IACDResultsOtherMarksRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDStudentsResultAssessmentBool> _list { get; set; }
        ACDStudentsResultAssessmentBool _details = new();

        public ACDResultsOtherMarksRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDStudentsResultAssessmentBool>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ACDStudentsResultAssessmentBool;";

                            _list = (List<ACDStudentsResultAssessmentBool>)await connection.QueryAsync<ACDStudentsResultAssessmentBool>(sql,
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

            return _list;
        }

        public Task<ACDStudentsResultAssessmentBool> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ACDStudentsResultAssessmentBool> AddAsync(ACDStudentsResultAssessmentBool entity)
        {
            sql = @"INSERT INTO ACDStudentsResultAssessmentBool (STDID, ClassID, SubjectID, SbjClassID, SbjClassification, SubjectCode, 
                    Subject, RatingValue, Five, Four, Three, Two, One, Zero) VALUES (@STDID, @ClassID, @SubjectID, @SbjClassID, 
                    @SbjClassification, @SubjectCode, @Subject, @RatingValue, @Five, @Four, @Three, @Two, @One, @Zero);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
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

        public Task<ACDStudentsResultAssessmentBool> UpdateAsync(int id, ACDStudentsResultAssessmentBool entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            sql = "DELETE FROM ACDStudentsResultAssessmentBool";

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

        public Task<IReadOnlyList<ACDStudentsResultAssessmentBool>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDResultsBroadSheetRepository : IACDResultsBroadSheetRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        List<string> fieldNames;
        List<dynamic> broadsheets;
        
        public ACDResultsBroadSheetRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<List<dynamic>> GetAllAsync()
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    sql = "SELECT * FROM ACDBroadsheet;";
                    broadsheets = (List<dynamic>)await connection.QueryAsync<dynamic>(sql);
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

            return broadsheets;
        }

        public async Task<List<string>> GetFieldNamesAsync()
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    sql = "SELECT name FROM sys.columns WHERE object_id = OBJECT_ID('ACDBroadsheet');";
                    fieldNames = (List<string>)await connection.QueryAsync<string>(sql);
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

            return fieldNames;
        }

        public async Task<ACDBroadSheet> ExecuteScriptAsync(ACDBroadSheet model)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    FileInfo file = new FileInfo(model.scriptFilePath);
                    string script = file.OpenText().ReadToEnd();

                    await connection.ExecuteAsync(script);
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

            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            switch (id)
            {
                case 1: //Delete Table
                    sql = "DROP TABLE IF EXISTS dbo.ACDBroadsheet";
                    break;
                case 2: //Add Colums Subject Count, MarkObtained, Average, Position
                    sql = "ALTER TABLE dbo.ACDBroadsheet ADD SubjectCount INT NULL, MarkObtained INT NULL, AverageMark DECIMAL(18, 2) NULL, Position INT NULL";
                    break;
            }
           

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { });
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

        public async Task<ACDBroadSheet> UpdateAsync(ACDBroadSheet model)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "UPDATE ACDBroadSheet SET SubjectCount = @SubjectCount, MarkObtained = @MarkObtained, Position = @Position, AverageMark = @AverageMark " +
                "WHERE STDID = @STDID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, model);
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

            return model;
        }
    }       
}
