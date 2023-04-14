using WebAppAcademics.Server.Interfaces.Academics.Exam;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Marks;
using System.Data;
using System.Data.SqlClient;

namespace WebAppAcademics.Server.Repositories
{
    public class ACDGradeSettingsRepository : IACDGradeSettingsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDSettingsGrade> _list { get; set; }
        ACDSettingsGrade _details = new();

        public ACDGradeSettingsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
       
        public async Task<IReadOnlyList<ACDSettingsGrade>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: 
                            sql = "SELECT * FROM ACDSettingsGrade;";

                            _list = (List<ACDSettingsGrade>)await connection.QueryAsync<ACDSettingsGrade>(sql, new { });
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

        public async Task<ACDSettingsGrade> GetByIdAsync(int id)
        {
            _details = new ACDSettingsGrade();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ACDSettingsGrade WHERE GradeID = @GradeID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDSettingsGrade>(sql, new { GradeID = id });
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

        public async Task<ACDSettingsGrade> AddAsync(ACDSettingsGrade entity)
        {
            sql = @"INSERT INTO ACDSettingsGrade (LowerGrade, HigherGrade, GradeLetter, GradeRemark, GradeComments, 
                    TeachersComment, PrincipalComment) OUTPUT INSERTED.GradeID VALUES (@LowerGrade, @HigherGrade, 
                    @GradeLetter, @GradeRemark, @GradeComments, @TeachersComment, @PrincipalComment);SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.GradeID = result;
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

        public async Task<ACDSettingsGrade> UpdateAsync(int id, ACDSettingsGrade entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1: 
                    sql = "UPDATE ACDSettingsGrade SET LowerGrade = @LowerGrade, HigherGrade = @HigherGrade, " +
                        "GradeLetter = @GradeLetter, GradeRemark = @GradeRemark, TeachersComment = @TeachersComment, " +
                        "PrincipalComment = @PrincipalComment WHERE GradeID = @GradeID;";
                    break;
                case 2:
                    sql = "UPDATE ACDSettingsGrade SET Id = @Id WHERE GradeID = @GradeID;";
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
            sql = "DELETE FROM ACDSettingsGrade WHERE GradeID = @GradeID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { GradeID = id });
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

        public Task<IReadOnlyList<ACDSettingsGrade>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDMockGradeSettingRepository : IACDMockGradeSettingRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDSettingsGradeMock> _list { get; set; }
        ACDSettingsGradeMock _details = new();

        public ACDMockGradeSettingRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDSettingsGradeMock>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ACDSettingsGradeMock;";

                            _list = (List<ACDSettingsGradeMock>)await connection.QueryAsync<ACDSettingsGradeMock>(sql, new { });
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

        public async Task<ACDSettingsGradeMock> GetByIdAsync(int id)
        {
            _details = new ACDSettingsGradeMock();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ACDSettingsGradeMock WHERE GradeID = @GradeID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDSettingsGradeMock>(sql, new { GradeID = id });
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

        public async Task<ACDSettingsGradeMock> AddAsync(ACDSettingsGradeMock entity)
        {
            sql = @"INSERT INTO ACDSettingsGradeMock (LowerGrade, HigherGrade, GradeLetter, GradeRemark) 
                    OUTPUT INSERTED.GradeID VALUES (@LowerGrade, @HigherGrade, @GradeLetter, @GradeRemark);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.GradeID = result;
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

        public async Task<ACDSettingsGradeMock> UpdateAsync(int id, ACDSettingsGradeMock entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDSettingsGradeMock SET LowerGrade = @LowerGrade, HigherGrade = @HigherGrade, GradeLetter = @GradeLetter, " +
                            "GradeRemark = @GradeRemark WHERE GradeID = @GradeID;";
                    break;
                case 2:
                    sql = "UPDATE ACDSettingsGradeMock SET Id = @Id WHERE GradeID = @GradeID;";
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
            sql = "DELETE FROM ACDSettingsGradeMock WHERE GradeID = @GradeID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { GradeID = id });
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

        public Task<IReadOnlyList<ACDSettingsGradeMock>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDCheckPointGradeSettingRepository : IACDCheckPointGradeSettingRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDSettingsGradeCheckPoint> _list { get; set; }
        ACDSettingsGradeCheckPoint _details = new();

        public ACDCheckPointGradeSettingRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDSettingsGradeCheckPoint>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ACDSettingsGradeCheckPoint;";

                            _list = (List<ACDSettingsGradeCheckPoint>)await connection.QueryAsync<ACDSettingsGradeCheckPoint>(sql, new { });
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

        public async Task<ACDSettingsGradeCheckPoint> GetByIdAsync(int id)
        {
            _details = new ACDSettingsGradeCheckPoint();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ACDSettingsGradeCheckPoint WHERE GradeID = @GradeID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDSettingsGradeCheckPoint>(sql, new { GradeID = id });
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

        public async Task<ACDSettingsGradeCheckPoint> AddAsync(ACDSettingsGradeCheckPoint entity)
        {
            sql = @"INSERT INTO ACDSettingsGradeCheckPoint (LowerGrade, HigherGrade, GradeLetter, GradeRemark, AutoComments) 
                    OUTPUT INSERTED.GradeID VALUES (@LowerGrade, @HigherGrade, @GradeLetter, @GradeRemark, @AutoComments);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.GradeID = result;
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

        public async Task<ACDSettingsGradeCheckPoint> UpdateAsync(int id, ACDSettingsGradeCheckPoint entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDSettingsGradeCheckPoint SET HigherGrade = @HigherGrade, LowerGrade = @LowerGrade,  " +
                        "HigherRating = @HigherRating, LowerRating = @LowerRating, GradeLetter = @GradeLetter, " +
                        "GradeRemark = @GradeRemark, AutoComments = @AutoComments WHERE GradeID = @GradeID;";
                    break;
                case 2:
                    sql = "UPDATE ACDSettingsGradeCheckPoint SET Id = @Id WHERE GradeID = @GradeID;";
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
            sql = "DELETE FROM ACDSettingsGradeCheckPoint WHERE GradeID = @GradeID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { GradeID = id });
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

        public Task<IReadOnlyList<ACDSettingsGradeCheckPoint>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDIGCSEGradeSettingsRepository : IACDIGCSEGradeSettingsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDSettingsGradeIGCSE> _list { get; set; }
        ACDSettingsGradeIGCSE _details = new();

        public ACDIGCSEGradeSettingsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDSettingsGradeIGCSE>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ACDSettingsGradeIGCSE;";

                            _list = (List<ACDSettingsGradeIGCSE>)await connection.QueryAsync<ACDSettingsGradeIGCSE>(sql, new { });
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

        public async Task<ACDSettingsGradeIGCSE> GetByIdAsync(int id)
        {
            _details = new ACDSettingsGradeIGCSE();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ACDSettingsGradeIGCSE WHERE GradeID = @GradeID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDSettingsGradeIGCSE>(sql, new { GradeID = id });
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

        public async Task<ACDSettingsGradeIGCSE> AddAsync(ACDSettingsGradeIGCSE entity)
        {
            sql = @"INSERT INTO ACDSettingsGradeIGCSE (LowerGrade, HigherGrade, GradeLetter, GradeRemark, AutoComments) 
                    OUTPUT INSERTED.GradeID VALUES (@LowerGrade, @HigherGrade, @GradeLetter, @GradeRemark, @AutoComments);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.GradeID = result;
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

        public async Task<ACDSettingsGradeIGCSE> UpdateAsync(int id, ACDSettingsGradeIGCSE entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDSettingsGradeIGCSE SET LowerGrade = @LowerGrade, HigherGrade = @HigherGrade, " +
                        "GradeLetter = @GradeLetter, GradeRemark = @GradeRemark, AutoComments = @AutoComments " +
                        "WHERE GradeID = @GradeID;";
                    break;
                case 2:
                    sql = "UPDATE ACDSettingsGradeIGCSE SET Id = @Id WHERE GradeID = @GradeID;";
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
            sql = "DELETE FROM ACDSettingsGradeIGCSE WHERE GradeID = @GradeID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { GradeID = id });
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

        public Task<IReadOnlyList<ACDSettingsGradeIGCSE>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDMarkSettingsRepository : IACDMarkSettingsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDSettingsMarks> _list { get; set; }
        ACDSettingsMarks _details = new();

        public ACDMarkSettingsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDSettingsMarks>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ACDSettingsMarks;";

                            _list = (List<ACDSettingsMarks>)await connection.QueryAsync<ACDSettingsMarks>(sql, new { });
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

        public async Task<ACDSettingsMarks> GetByIdAsync(int id)
        {
            _details = new ACDSettingsMarks();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ACDSettingsMarks WHERE MarkID = @MarkID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDSettingsMarks>(sql, new { MarkID = id });
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

        public async Task<ACDSettingsMarks> AddAsync(ACDSettingsMarks entity)
        {
            sql = @"INSERT INTO ACDSettingsMarks (MarkType, Mark, PassMark, ApplyPassMark, PassMarkColor, FailMarkColor, 
                    ApplyCBT) OUTPUT INSERTED.MarkID VALUES (@MarkType, @Mark, @PassMark, @ApplyPassMark, @PassMarkColor, 
                    @FailMarkColor, @ApplyCBT);SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.MarkID = result;
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

        public async Task<ACDSettingsMarks> UpdateAsync(int id, ACDSettingsMarks entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDSettingsMarks SET MarkType = @MarkType, Mark = @Mark, PassMark = @PassMark, " +
                            "ApplyPassMark = @ApplyPassMark, ApplyCBT = @ApplyCBT WHERE MarkID = @MarkID;";
                    break;
                case 2:
                    sql = "UPDATE ACDSettingsMarks SET Id = @Id WHERE MarkID = @MarkID;";
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
            sql = "DELETE FROM ACDSettingsMarks WHERE MarkID = @MarkID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { MarkID = id });
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

        public Task<int> CountAsync(SwitchModel _switchdt)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ACDSettingsMarks>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDRatingSettingsRepository : IACDRatingSettingsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDSettingsRating> _list { get; set; }
        ACDSettingsRating _details = new();

        public ACDRatingSettingsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDSettingsRating>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ACDSettingsRating;";

                            _list = (List<ACDSettingsRating>)await connection.QueryAsync<ACDSettingsRating>(sql, new { });
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

        public async Task<ACDSettingsRating> GetByIdAsync(int id)
        {
            _details = new ACDSettingsRating();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ACDSettingsRating WHERE RatingID = @RatingID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDSettingsRating>(sql, new { RatingID = id });
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

        public async Task<ACDSettingsRating> AddAsync(ACDSettingsRating entity)
        {
            sql = @"INSERT INTO ACDSettingsRating (Rating, LowScore, HighScore, GradeLetter, RatingLevel, RatingKey, ShownCol) 
                    OUTPUT INSERTED.RatingID VALUES (@Rating, @LowScore, @HighScore, @GradeLetter, @RatingLevel, @RatingKey, 
                    @ShownCol);SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.RatingID = result;
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

        public async Task<ACDSettingsRating> UpdateAsync(int id, ACDSettingsRating entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDSettingsRating SET Rating = @Rating, LowScore = @LowScore, HighScore = @HighScore, " +
                        "GradeLetter = @GradeLetter, RatingLevel = @RatingLevel, RatingKey = @RatingKey " +
                        "WHERE RatingID = @RatingID;";
                    break;
                case 2:
                    sql = "UPDATE ACDSettingsRating SET Id = @Id WHERE RatingID = @RatingID;";
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
            sql = "DELETE FROM ACDSettingsRating WHERE RatingID = @RatingID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { RatingID = id });
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

        public Task<IReadOnlyList<ACDSettingsRating>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDRatingOptionSettingsRepository : IACDRatingOptionSettingsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDSettingsRatingOptions> _list { get; set; }
        ACDSettingsRatingOptions _details = new();

        public ACDRatingOptionSettingsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDSettingsRatingOptions>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ACDSettingsRatingOptions;";

                            _list = (List<ACDSettingsRatingOptions>)await connection.QueryAsync<ACDSettingsRatingOptions>(sql, new { });
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

        public async Task<ACDSettingsRatingOptions> GetByIdAsync(int id)
        {
            _details = new ACDSettingsRatingOptions();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ACDSettingsRatingOptions WHERE OptionID = @OptionID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDSettingsRatingOptions>(sql, new { OptionID = id });
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

        public async Task<ACDSettingsRatingOptions> AddAsync(ACDSettingsRatingOptions entity)
        {
            sql = @"INSERT INTO ACDSettingsRatingOptions (RatingOption, UsedOption) OUTPUT INSERTED.OptionID 
                    VALUES (@RatingOption, @UsedOption);SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.OptionID = result;
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

        public async Task<ACDSettingsRatingOptions> UpdateAsync(int id, ACDSettingsRatingOptions entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDSettingsRatingOptions SET UsedOption = @UsedOption WHERE OptionID = @OptionID;";
                    break;
                case 2:
                    sql = "UPDATE ACDSettingsRatingOptions SET Id = @Id WHERE OptionID = @OptionID;";
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
            sql = "DELETE FROM ACDSettingsRatingOptions WHERE OptionID = @OptionID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { OptionID = id });
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

        public Task<int> CountAsync(SwitchModel _switchdnt)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ACDSettingsRatingOptions>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDRatingTextSettingsRepository : IACDRatingTextSettingsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDSettingsRatingText> _list { get; set; }
        ACDSettingsRatingText _details = new();

        public ACDRatingTextSettingsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDSettingsRatingText>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ACDSettingsRatingText;";

                            _list = (List<ACDSettingsRatingText>)await connection.QueryAsync<ACDSettingsRatingText>(sql, new { });
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

        public async Task<ACDSettingsRatingText> GetByIdAsync(int id)
        {
            _details = new ACDSettingsRatingText();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ACDSettingsRatingText WHERE TextID = @TextID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDSettingsRatingText>(sql, new { TextID = id });
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

        public async Task<ACDSettingsRatingText> AddAsync(ACDSettingsRatingText entity)
        {
            sql = @"INSERT INTO ACDSettingsRatingText (RatingText, UsedText) OUTPUT INSERTED.TextID 
                    VALUES (@RatingText, @UsedText);SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.TextID = result;
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

        public async Task<ACDSettingsRatingText> UpdateAsync(int id, ACDSettingsRatingText entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDSettingsRatingText SET UsedText = @UsedText WHERE TextID = @TextID;";
                    break;
                case 2:
                    sql = "UPDATE ACDSettingsRatingText SET Id = @Id WHERE TextID = @TextID;";
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
            sql = "DELETE FROM ACDSettingsRatingText WHERE TextID = @TextID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { TextID = id });
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

        public Task<int> CountAsync(SwitchModel _switchdnt)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ACDSettingsRatingText>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDOtherSettingsRepository : IACDOtherSettingsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDSettingsOthers> _list { get; set; }
        ACDSettingsOthers _details = new();

        public ACDOtherSettingsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDSettingsOthers>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ACDSettingsOthers;";

                            _list = (List<ACDSettingsOthers>)await connection.QueryAsync<ACDSettingsOthers>(sql, new { });
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

        public async Task<ACDSettingsOthers> GetByIdAsync(int id)
        {
            _details = new ACDSettingsOthers();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ACDSettingsOthers WHERE OtherSettingID = @OtherSettingID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDSettingsOthers>(sql, new { OtherSettingID = id });
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

        public async Task<ACDSettingsOthers> AddAsync(ACDSettingsOthers entity)
        {
            sql = @"INSERT INTO ACDSettingsOthers (Description, BoolValue, TextValue) OUTPUT INSERTED.OtherSettingID 
                    VALUES (@Description, @BoolValue, @TextValue);SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.OtherSettingID = result;
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

        public async Task<ACDSettingsOthers> UpdateAsync(int id, ACDSettingsOthers entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDSettingsOthers SET BoolValue = @BoolValue WHERE OtherSettingID = @OtherSettingID;";
                    break;
                case 2:
                    sql = "UPDATE ACDSettingsOthers SET Id = @Id WHERE OtherSettingID = @OtherSettingID;";
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
            sql = "DELETE FROM ACDSettingsOthers WHERE OtherSettingID = @OtherSettingID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { OtherSettingID = id });
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

        public Task<int> CountAsync(SwitchModel _switchdint)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ACDSettingsOthers>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDOtherGradeSettingsRepository : IACDOtherGradeSettingsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDSettingsGradeOthers> _list { get; set; }
        ACDSettingsGradeOthers _details = new();

        public ACDOtherGradeSettingsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDSettingsGradeOthers>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ACDSettingsGradeOthers;";

                            _list = (List<ACDSettingsGradeOthers>)await connection.QueryAsync<ACDSettingsGradeOthers>(sql, new { });
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

        public async Task<ACDSettingsGradeOthers> GetByIdAsync(int id)
        {
            _details = new ACDSettingsGradeOthers();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ACDSettingsGradeOthers WHERE GradeID = @GradeID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDSettingsGradeOthers>(sql, new { GradeID = id });
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

        public async Task<ACDSettingsGradeOthers> AddAsync(ACDSettingsGradeOthers entity)
        {
            sql = @"INSERT INTO ACDSettingsGradeOthers (LowerGrade, HigherGrade, GradeLetter, GradeRemark) 
                    OUTPUT INSERTED.GradeID VALUES (@LowerGrade, @HigherGrade, @GradeLetter, @GradeRemark);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.GradeID = result;
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

        public async Task<ACDSettingsGradeOthers> UpdateAsync(int id, ACDSettingsGradeOthers entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDSettingsGradeOthers SET LowerGrade = @LowerGrade, HigherGrade = @HigherGrade, " +
                        "GradeLetter = @GradeLetter, GradeRemark = @GradeRemark WHERE GradeID = @GradeID;";
                    break;
                case 2:
                    sql = "UPDATE ACDSettingsGradeOthers SET Id = @Id WHERE GradeID = @GradeID;";
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
            sql = "DELETE FROM ACDSettingsGradeOthers WHERE GradeID = @GradeID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { GradeID = id });
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

        public Task<IReadOnlyList<ACDSettingsGradeOthers>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDResultTypeSettingsRepository : IACDResultTypeSettingsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDReportType> _list { get; set; }
        ACDReportType _details = new();

        public ACDResultTypeSettingsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDReportType>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ACDReportType WHERE ReportTypeID < 3;";

                            _list = (List<ACDReportType>)await connection.QueryAsync<ACDReportType>(sql, new { });
                            break;
                        case 2:
                            sql = "SELECT * FROM ACDReportType;";

                            _list = (List<ACDReportType>)await connection.QueryAsync<ACDReportType>(sql, new { });
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

        public async Task<ACDReportType> GetByIdAsync(int id)
        {
            _details = new ACDReportType();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ACDReportType WHERE ReportTypeID = @ReportTypeID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDReportType>(sql, new { ReportTypeID = id });
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

        public async Task<ACDReportType> AddAsync(ACDReportType entity)
        {
            sql = @"INSERT INTO ACDReportType (ReportType, SelectedExam) OUTPUT INSERTED.ReportTypeID 
                    VALUES (@ReportType, @SelectedExam);SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.ReportTypeID = result;
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

        public async Task<ACDReportType> UpdateAsync(int id, ACDReportType entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDReportType SET SelectedExam = @SelectedExam WHERE ReportTypeID = @ReportTypeID;";
                    break;
                case 2:
                    sql = "UPDATE ACDReportType SET Id = @Id WHERE ReportTypeID = @ReportTypeID;";
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
            sql = "DELETE FROM ACDReportType WHERE ReportTypeID = @ReportTypeID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { ReportTypeID = id });
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

        public Task<IReadOnlyList<ACDReportType>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDResultHeaderFooterSettingsRepository : IACDResultHeaderFooterSettingsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDReportFooter> _list { get; set; }
        ACDReportFooter _details = new();

        public ACDResultHeaderFooterSettingsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDReportFooter>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ACDReportFooter;";

                            _list = (List<ACDReportFooter>)await connection.QueryAsync<ACDReportFooter>(sql, new { });
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

        public async Task<ACDReportFooter> GetByIdAsync(int id)
        {
            _details = new ACDReportFooter();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ACDReportFooter WHERE FooterID = @FooterID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDReportFooter>(sql, new { FooterID = id });
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

        public async Task<ACDReportFooter> AddAsync(ACDReportFooter entity)
        {
            sql = @"INSERT INTO ACDReportFooter (HeaderCA, HeaderExam, Footer) OUTPUT INSERTED.FooterID 
                    VALUES (@HeaderCA, @HeaderExam, @Footer);SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.FooterID = result;
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

        public async Task<ACDReportFooter> UpdateAsync(int id, ACDReportFooter entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDReportFooter SET Footer = @Footer, HeaderCA = @HeaderCA, HeaderExam = @HeaderExam " +
                        "WHERE FooterID = @FooterID;";
                    break;
                case 2:
                    sql = "UPDATE ACDReportFooter SET Id = @Id WHERE FooterID = @FooterID;";
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
            sql = "DELETE FROM ACDReportFooter WHERE FooterID = @FooterID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { FooterID = id });
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

        public Task<IReadOnlyList<ACDReportFooter>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDFlagsRepository : IACDFlagsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDFlags> _list { get; set; }
        ACDFlags _details = new();

        public ACDFlagsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
                
        public async Task<IReadOnlyList<ACDFlags>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM ACDFlags;";

                            _list = (List<ACDFlags>)await connection.QueryAsync<ACDFlags>(sql, new { });
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

        public async Task<ACDFlags> UpdateAsync(int id, ACDFlags entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDFlags SET Flag = @Flag WHERE FlagID = @FlagID;";
                    break;
                case 2:
                    sql = "UPDATE ACDFlags SET Id = @Id WHERE FlagID = @FlagID;";
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

        public Task<ACDFlags> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<ACDFlags>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }

        public Task<ACDFlags> AddAsync(ACDFlags entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(SwitchModel _switchd)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

    }
}
