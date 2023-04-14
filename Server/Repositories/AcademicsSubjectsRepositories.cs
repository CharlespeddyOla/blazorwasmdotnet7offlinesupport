using WebAppAcademics.Server.Interfaces.Academics.Subjects;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Academics.Subjects;
using System.Data;
using System.Data.SqlClient;

namespace WebAppAcademics.Server.Repositories
{
    public class ACDSubjectsRepository : IACDSubjectsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDSubjects> _list { get; set; }
        ACDSubjects _details = new();

        public ACDSubjectsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDSubjects>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: //Filter By StatusTye
                            sql = "SELECT A.SubjectID, A.SbjDeptID, A.SbjClassID, A.SchID, A.SubjectCode, A.Subject, A.SubjectStatus, A.SbjMerge, " +
                                "A.SbjMergeID, A.SbjMergeName, A.SortID, B.SbjDept AS SubjectDepartment, C.SbjClassification AS SubjectClassification, " +
                                "C.Status AS SubjectClassificationStatus, D.School, D.StaffID " +
                                "FROM ACDSubjects A " +
                                "INNER JOIN ACDSbjDept B ON B.SbjDeptID = A.SbjDeptID " +
                                "INNER JOIN ACDSbjClassification C ON C.SbjClassID = A.SbjClassID " +
                                "INNER JOIN ADMSchlList D ON D.SchID = A.SchID " +
                                "WHERE A.SubjectStatus = @SubjectStatus ORDER BY A.SchID, A.SbjDeptID, A.SbjClassID, A.Subject;";

                            _list = (List<ACDSubjects>)await connection.QueryAsync<ACDSubjects>(sql,
                                new
                                {
                                    SubjectStatus = _switch.SubjectStatus
                                });
                            break;
                        case 2: //Filter By School And SubjectStatus                            
                            sql = "SELECT A.SubjectID, A.SbjDeptID, A.SbjClassID, A.SchID, A.SubjectCode, A.Subject, A.SubjectStatus, " +
                                "A.SbjMerge, A.SbjMergeID, A.SbjMergeName, A.SortID, B.SbjDept AS SubjectDepartment, " +
                                "C.SbjClassification AS SubjectClassification, C.Status AS SubjectClassificationStatus, D.School, D.StaffID " +
                                "FROM ACDSubjects A " +
                                "INNER JOIN ACDSbjDept B ON B.SbjDeptID = A.SbjDeptID " +
                                "INNER JOIN ACDSbjClassification C ON C.SbjClassID = A.SbjClassID " +
                                "INNER JOIN ADMSchlList D ON D.SchID = A.SchID " +
                                "WHERE A.SchID = @SchID AND A.SubjectStatus = @SubjectStatus " +
                                "ORDER BY A.SchID, A.SbjDeptID, A.SbjClassID, A.Subject;";

                            _list = (List<ACDSubjects>)await connection.QueryAsync<ACDSubjects>(sql,
                                new
                                {
                                    SchID = _switch.SchID,
                                    SubjectStatus = _switch.SubjectStatus
                                });
                            break;
                        case 3: //Filter By Subject Department And SubjectStatus
                           sql = "SELECT A.SubjectID, A.SbjDeptID, A.SbjClassID, A.SchID, A.SubjectCode, A.Subject, A.SubjectStatus, A.SbjMerge, " +
                                "A.SbjMergeID, A.SbjMergeName, A.SortID, B.SbjDept AS SubjectDepartment, C.SbjClassification AS SubjectClassification, " +
                                "C.Status AS SubjectClassificationStatus, D.School, D.StaffID " +
                                "FROM ACDSubjects A " +
                                "INNER JOIN ACDSbjDept B ON B.SbjDeptID = A.SbjDeptID " +
                                "INNER JOIN ACDSbjClassification C ON C.SbjClassID = A.SbjClassID " +
                                "INNER JOIN ADMSchlList D ON D.SchID = A.SchID " +
                                "WHERE A.SbjDeptID = @SbjDeptID AND A.SubjectStatus = @SubjectStatus " +
                                "ORDER BY A.SchID, A.SbjDeptID, A.SbjClassID, A.Subject;";

                            _list = (List<ACDSubjects>)await connection.QueryAsync<ACDSubjects>(sql,
                                new
                                {
                                    SbjDeptID = _switch.SbjDeptID,
                                    SubjectStatus = _switch.SubjectStatus
                                });
                            break;
                        case 4: //Filter By Subject Classification And SubjectStatus
                            sql = "SELECT A.SubjectID, A.SbjDeptID, A.SbjClassID, A.SchID, A.SubjectCode, A.Subject, A.SubjectStatus, A.SbjMerge, " +
                               "A.SbjMergeID, A.SbjMergeName, A.SortID, B.SbjDept AS SubjectDepartment, C.SbjClassification AS SubjectClassification, " +
                               "C.Status AS SubjectClassificationStatus, D.School, D.StaffID " +
                               "FROM ACDSubjects A " +
                               "INNER JOIN ACDSbjDept B ON B.SbjDeptID = A.SbjDeptID " +
                               "INNER JOIN ACDSbjClassification C ON C.SbjClassID = A.SbjClassID " +
                               "INNER JOIN ADMSchlList D ON D.SchID = A.SchID " +
                               "WHERE A.SbjClassID = @SbjClassID AND A.SubjectStatus = @SubjectStatus " +
                               "ORDER BY A.SchID, A.SbjDeptID, A.SbjClassID, A.Subject;";

                            _list = (List<ACDSubjects>)await connection.QueryAsync<ACDSubjects>(sql,
                                new
                                {
                                    SbjClassID = _switch.SbjClassID,
                                    SubjectStatus = _switch.SubjectStatus
                                });
                            break;
                        case 5: //Filter By School, Department And Status
                            sql = "SELECT A.SubjectID, A.SbjDeptID, A.SbjClassID, A.SchID, A.SubjectCode, A.Subject, A.SubjectStatus, A.SbjMerge, " +
                               "A.SbjMergeID, A.SbjMergeName, A.SortID, B.SbjDept AS SubjectDepartment, C.SbjClassification AS SubjectClassification, " +
                               "C.Status AS SubjectClassificationStatus, D.School, D.StaffID " +
                               "FROM ACDSubjects A " +
                               "INNER JOIN ACDSbjDept B ON B.SbjDeptID = A.SbjDeptID " +
                               "INNER JOIN ACDSbjClassification C ON C.SbjClassID = A.SbjClassID " +
                               "INNER JOIN ADMSchlList D ON D.SchID = A.SchID " +
                               "WHERE  A.SchID =@SchID AND A.SbjDeptID = @SbjDeptID AND A.SubjectStatus = @SubjectStatus " +
                               "ORDER BY A.SchID, A.SbjDeptID, A.SbjClassID, A.Subject;";

                            _list = (List<ACDSubjects>)await connection.QueryAsync<ACDSubjects>(sql,
                                new
                                {
                                    SchID = _switch.SchID,
                                    SbjDeptID = _switch.SbjDeptID,
                                    SubjectStatus = _switch.SubjectStatus
                                });
                            break;
                        case 6: //Filter By School, Classification And Status
                            sql = "SELECT A.SubjectID, A.SbjDeptID, A.SbjClassID, A.SchID, A.SubjectCode, A.Subject, A.SubjectStatus, A.SbjMerge, " +
                               "A.SbjMergeID, A.SbjMergeName, A.SortID, B.SbjDept AS SubjectDepartment, C.SbjClassification AS SubjectClassification, " +
                               "C.Status AS SubjectClassificationStatus, D.School, D.StaffID " +
                               "FROM ACDSubjects A " +
                               "INNER JOIN ACDSbjDept B ON B.SbjDeptID = A.SbjDeptID " +
                               "INNER JOIN ACDSbjClassification C ON C.SbjClassID = A.SbjClassID " +
                               "INNER JOIN ADMSchlList D ON D.SchID = A.SchID " +
                               "WHERE  A.SchID =@SchID AND A.SbjClassID = @SbjClassID AND A.SubjectStatus = @SubjectStatus " +
                               "ORDER BY A.SchID, A.SbjDeptID, A.SbjClassID, A.Subject;";

                            _list = (List<ACDSubjects>)await connection.QueryAsync<ACDSubjects>(sql,
                                new
                                {
                                    SchID = _switch.SchID,
                                    SbjClassID = _switch.SbjClassID,
                                    SubjectStatus = _switch.SubjectStatus
                                });
                            break;
                        case 7: //Filter By Department, Classification And Subject Status
                            sql = "SELECT A.SubjectID, A.SbjDeptID, A.SbjClassID, A.SchID, A.SubjectCode, A.Subject, A.SubjectStatus, A.SbjMerge, " +
                              "A.SbjMergeID, A.SbjMergeName, A.SortID, B.SbjDept AS SubjectDepartment, C.SbjClassification AS SubjectClassification, " +
                              "C.Status AS SubjectClassificationStatus, D.School, D.StaffID " +
                              "FROM ACDSubjects A " +
                              "INNER JOIN ACDSbjDept B ON B.SbjDeptID = A.SbjDeptID " +
                              "INNER JOIN ACDSbjClassification C ON C.SbjClassID = A.SbjClassID " +
                              "INNER JOIN ADMSchlList D ON D.SchID = A.SchID " +
                              "WHERE A.SbjDeptID =@SbjDeptID AND A.SbjClassID = @SbjClassID AND A.SubjectStatus = @SubjectStatus " +
                              "ORDER BY A.SchID, A.SbjDeptID, A.SbjClassID, A.Subject;";

                            _list = (List<ACDSubjects>)await connection.QueryAsync<ACDSubjects>(sql,
                                new
                                {
                                    SbjDeptID = _switch.SbjDeptID,
                                    SbjClassID = _switch.SbjClassID,
                                    SubjectStatus = _switch.SubjectStatus
                                });
                            break;
                        case 8: //Filter By School, Department, Classification And Status
                            sql = "SELECT A.SubjectID, A.SbjDeptID, A.SbjClassID, A.SchID, A.SubjectCode, A.Subject, A.SubjectStatus, A.SbjMerge, " +
                              "A.SbjMergeID, A.SbjMergeName, A.SortID, B.SbjDept AS SubjectDepartment, C.SbjClassification AS SubjectClassification, " +
                              "C.Status AS SubjectClassificationStatus, D.School, D.StaffID " +
                              "FROM ACDSubjects A " +
                              "INNER JOIN ACDSbjDept B ON B.SbjDeptID = A.SbjDeptID " +
                              "INNER JOIN ACDSbjClassification C ON C.SbjClassID = A.SbjClassID " +
                              "INNER JOIN ADMSchlList D ON D.SchID = A.SchID " +
                              "WHERE A.SchID = @SchID AND A.SbjDeptID =@SbjDeptID AND A.SbjClassID = @SbjClassID AND A.SubjectStatus = @SubjectStatus " +
                              "ORDER BY A.SchID, A.SbjDeptID, A.SbjClassID, A.Subject;";

                            _list = (List<ACDSubjects>)await connection.QueryAsync<ACDSubjects>(sql,
                                new
                                {
                                    SchID = _switch.SchID,
                                    SbjDeptID = _switch.SbjDeptID,
                                    SbjClassID = _switch.SbjClassID,
                                    SubjectStatus = _switch.SubjectStatus
                                });
                            break;
                        case 9:
                            sql = "SELECT A.SubjectID, A.SbjDeptID, A.SbjClassID, A.SchID, A.SubjectCode, A.Subject, A.SubjectStatus, A.SbjMerge, " +
                              "A.SbjMergeID, A.SbjMergeName, A.SortID, B.SbjDept AS SubjectDepartment, C.SbjClassification AS SubjectClassification, " +
                              "C.Status AS SubjectClassificationStatus, D.School, D.StaffID " +
                              "FROM ACDSubjects A " +
                              "INNER JOIN ACDSbjDept B ON B.SbjDeptID = A.SbjDeptID " +
                              "INNER JOIN ACDSbjClassification C ON C.SbjClassID = A.SbjClassID " +
                              "INNER JOIN ADMSchlList D ON D.SchID = A.SchID WHERE A.SubjectStatus = 1 " +
                              "ORDER BY A.SchID, A.SbjDeptID, A.SbjClassID, A.SortID;";

                            _list = (List<ACDSubjects>)await connection.QueryAsync<ACDSubjects>(sql, new { });
                            break;
                        default:
                            sql = "SELECT A.SubjectID, A.SbjDeptID, A.SbjClassID, A.SchID, A.SubjectCode, A.Subject, A.SubjectStatus, A.SbjMerge, " +
                              "A.SbjMergeID, A.SbjMergeName, A.SortID, B.SbjDept AS SubjectDepartment, C.SbjClassification AS SubjectClassification, " +
                              "C.Status AS SubjectClassificationStatus, D.School, D.StaffID " +
                              "FROM ACDSubjects A " +
                              "INNER JOIN ACDSbjDept B ON B.SbjDeptID = A.SbjDeptID " +
                              "INNER JOIN ACDSbjClassification C ON C.SbjClassID = A.SbjClassID " +
                              "INNER JOIN ADMSchlList D ON D.SchID = A.SchID " +
                              "ORDER BY A.SchID, A.SbjDeptID, A.SbjClassID, A.Subject;";

                            _list = (List<ACDSubjects>)await connection.QueryAsync<ACDSubjects>(sql, new { });
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

        public async Task<ACDSubjects> GetByIdAsync(int id)
        {
            _details = new ACDSubjects();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.SbjDeptID, A.SbjClassID, A.SchID, A.SubjectCode, A.Subject, A.SubjectStatus, A.SbjMerge, " +
                    "A.SbjMergeID, A.SbjMergeName, A.SortID, B.SbjDept AS SubjectDepartment, C.SbjClassification AS SubjectClassification, " +
                    "C.Status AS SubjectClassificationStatus, D.School, D.StaffID " +
                    "FROM ACDSubjects A " +
                    "INNER JOIN ACDSbjDept B ON B.SbjDeptID = A.SbjDeptID " +
                    "INNER JOIN ACDSbjClassification C ON C.SbjClassID = A.SbjClassID " +
                    "INNER JOIN ADMSchlList D ON D.SchID = A.SchID " +
                    "WHERE A.SubjectID = @SubjectID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDSubjects>(sql, new { SubjectID = id });
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

        public async Task<ACDSubjects> AddAsync(ACDSubjects entity)
        {
            sql = @"INSERT INTO ACDSubjects (SbjDeptID, SbjClassID, SchID, SubjectCode, Subject, SubjectStatus, SortID) 
                    OUTPUT INSERTED.SubjectID VALUES (@SbjDeptID, @SbjClassID, @SchID, @SubjectCode, @Subject, @SubjectStatus, @SortID);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.SubjectID = result;
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

        public async Task<ACDSubjects> UpdateAsync(int id, ACDSubjects entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDSubjects SET SbjDeptID = @SbjDeptID, SbjClassID = @SbjClassID, SchID = @SchID, " +
                        "SubjectCode = @SubjectCode, Subject = @Subject, SubjectStatus = @SubjectStatus, SortID = @SortID " +
                        "WHERE SubjectID = @SubjectID;";
                    break;
                case 2: //Other Updates For Combining Subjects
                    sql = "UPDATE ACDSubjects SET SbjMerge = @SbjMerge, SbjMergeID = @SbjMergeID, SbjMergeName = @SbjMergeName " +
                        "WHERE SubjectID = @SubjectID;";
                    break;
                case 3:
                    sql = "UPDATE ACDSubjects SET Id = @Id WHERE SubjectID = @SubjectID;";
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
            sql = "DELETE FROM ACDSubjects WHERE SubjectID = @SubjectID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { SubjectID = id });
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

        public Task<IReadOnlyList<ACDSubjects>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDSubjectsClassRepository : IACDSubjectsClassRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDSbjClassification> _list { get; set; }
        ACDSbjClassification _details = new();

        public ACDSubjectsClassRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDSbjClassification>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT A.SbjClassID, A.SbjClassification, A.Remark, A.Status, COUNT(B.SbjClassID) AS SubjectCount " +
                                "FROM ACDSbjClassification A " +
                                "LEFT OUTER JOIN ACDSubjects B ON B.SbjClassID = A.SbjClassID " +
                                "WHERE A.Status = 'True' GROUP BY A.SbjClassID, A.SbjClassification, A.Remark, A.Status;";
                            break;
                        case 2: //PsychoMotor Marks
                            sql = "SELECT A.SbjClassID, A.SbjClassification, A.Remark, A.Status, COUNT(B.SbjClassID) AS SubjectCount " +
                                "FROM ACDSbjClassification A " +
                                "LEFT OUTER JOIN ACDSubjects B ON B.SbjClassID = A.SbjClassID " +
                                "WHERE A.Status = 'True' AND B.SbjClassID > 1 GROUP BY A.SbjClassID, " +
                                "A.SbjClassification, A.Remark, A.Status;";
                            break;
                        default:
                            sql = "SELECT * FROM ACDSbjClassification;";
                            break;
                    }

                    _list = (List<ACDSbjClassification>)await connection.QueryAsync<ACDSbjClassification>(sql, new { });
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

        public async Task<ACDSbjClassification> GetByIdAsync(int id)
        {
            _details = new ACDSbjClassification();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ACDSbjClassification WHERE SbjClassID = @SbjClassID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDSbjClassification>(sql, new { SbjClassID = id });
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

        public async Task<ACDSbjClassification> AddAsync(ACDSbjClassification entity)
        {
            sql = @"INSERT INTO ACDSbjClassification (SbjClassification, Remark, Status) OUTPUT INSERTED.SbjClassID 
                    VALUES (@SbjClassification, @Remark, @Status);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.SbjClassID = result;
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

        public async Task<ACDSbjClassification> UpdateAsync(int id, ACDSbjClassification entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDSbjClassification SET SbjClassification = @SbjClassification, Remark = @Remark, Status = @Status " +
                            "WHERE SbjClassID = @SbjClassID;";
                    break;
                case 2:
                    sql = "UPDATE ACDSbjClassification SET Id = @Id WHERE SbjClassID = @SbjClassID;";
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
            sql = "DELETE FROM ACDSbjClassification WHERE SbjClassID = @SbjClassID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { SbjClassID = id });
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

        public Task<IReadOnlyList<ACDSbjClassification>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDSubjectsDeptRepository : IACDSubjectsDeptRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDSbjDept> _list { get; set; }
        ACDSbjDept _details = new();

        public ACDSubjectsDeptRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDSbjDept>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT A.SbjDeptID, A.SbjDept, COUNT(B.SbjDeptID) AS SubjectCount " +
                                "FROM ACDSbjDept A " +
                                "LEFT OUTER JOIN ACDSubjects B ON B.SbjDeptID = A.SbjDeptID " +
                                "GROUP BY A.SbjDeptID, A.SbjDept;";

                            _list = (List<ACDSbjDept>)await connection.QueryAsync<ACDSbjDept>(sql, new { });
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

        public async Task<ACDSbjDept> GetByIdAsync(int id)
        {
            _details = new ACDSbjDept();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ACDSbjDept WHERE SbjDeptID = @SbjDeptID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDSbjDept>(sql, new { SbjDeptID = id });
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

        public async Task<ACDSbjDept> AddAsync(ACDSbjDept entity)
        {
            sql = @"INSERT INTO ACDSbjDept (SbjDept) OUTPUT INSERTED.SbjDeptID VALUES (@SbjDept);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.SbjDeptID = result;
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

        public async Task<ACDSbjDept> UpdateAsync(int id, ACDSbjDept entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ACDSbjDept SET SbjDept = @SbjDept WHERE SbjDeptID = @SbjDeptID;";
                    break;
                case 2:
                    sql = "UPDATE ACDSbjDept SET Id = @Id WHERE SbjDeptID = @SbjDeptID;";
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
            sql = "DELETE FROM ACDSbjDept WHERE SbjDeptID = @SbjDeptID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { SbjDeptID = id });
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

        public Task<IReadOnlyList<ACDSbjDept>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDStudentSubjectsAllocationRepository : IACDStudentSubjectsAllocationRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDSbjAllocationStudents> _list { get; set; }
        ACDSbjAllocationStudents _details = new();

        public ACDStudentSubjectsAllocationRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDSbjAllocationStudents>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:  //Filter By Allocated/Not-Allocated Subjects (By SbjSelection)
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.STDID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "B.SubjectCode, B.Subject, C.SbjClassification, B.SbjDeptID, B.SbjClassID, D.SbjDept, E.School, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END AS SchClass, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "H.Surname, H.FirstName, H.MiddleName, C.SbjClassID, " +
                                "H.Surname + ' ' + SUBSTRING(H.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(H.MiddleName, 0, 2) + '. ', '') " +
                                "+ ' [' + I.PrefixName + REPLACE(STR(H.StudentID, I.PrefixDigits), SPACE(1), '0') + ']'  AS StudentName, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, A.ClassListID, G.CATID, H.Surname ORDER BY A.SchID, " +
                                "A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID) AS SN " +
                                "FROM ACDSbjAllocationStudents A " +
                                "LEFT OUTER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification C ON C.SbjClassID = B.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept D ON D.SbjDeptID = B.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassGroup F ON F.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassList G ON G.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMStudents H ON H.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix I ON I.PrefixID = H.PrefixID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = G.CATID " +
                                "WHERE A.DeleteStatus = 'False' AND B.SubjectStatus = 'True' " +
                                "AND A.SchSession = @SchSession AND A.SbjSelection = @SbjSelection " +
                                "ORDER BY A.SchID, A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID;";

                            _list = (List<ACDSbjAllocationStudents>)await connection.QueryAsync<ACDSbjAllocationStudents>(sql,
                                new
                                {
                                    _switch.SchSession,
                                    _switch.SbjSelection
                                });
                            break;
                        case 2: //Filter By S Allocated/Not-Allocated Subjects And School
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.STDID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "B.SubjectCode, B.Subject, C.SbjClassification, B.SbjDeptID, B.SbjClassID, D.SbjDept, E.School, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END AS SchClass, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "H.Surname, H.FirstName, H.MiddleName, C.SbjClassID, " +
                                "H.Surname + ' ' + SUBSTRING(H.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(H.MiddleName, 0, 2) + '. ', '') " +
                                "+ ' [' + I.PrefixName + REPLACE(STR(H.StudentID, I.PrefixDigits), SPACE(1), '0') + ']'  AS StudentName, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, A.ClassListID, G.CATID, H.Surname ORDER BY A.SchID, " +
                                "A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID) AS SN " +
                                "FROM ACDSbjAllocationStudents A " +
                                "LEFT OUTER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification C ON C.SbjClassID = B.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept D ON D.SbjDeptID = B.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassGroup F ON F.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassList G ON G.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMStudents H ON H.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix I ON I.PrefixID = H.PrefixID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = G.CATID " +
                                "WHERE A.DeleteStatus = 'False' AND B.SubjectStatus = 'True' " +
                                "AND A.SchSession = @SchSession AND A.SbjSelection = @SbjSelection " +
                                "AND A.SchID = @SchID ORDER BY A.SchID, A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID;";

                            _list = (List<ACDSbjAllocationStudents>)await connection.QueryAsync<ACDSbjAllocationStudents>(sql,
                                new
                                {
                                    _switch.SchSession,
                                    _switch.SbjSelection,
                                    _switch.SchID
                                });
                            break;
                        case 3: //Filter By  Allocated/Not-Allocated Subjects, School And ClassGroup
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.STDID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "B.SubjectCode, B.Subject, C.SbjClassification, B.SbjDeptID, B.SbjClassID, D.SbjDept, E.School, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END AS SchClass, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "H.Surname, H.FirstName, H.MiddleName, C.SbjClassID, " +
                                "H.Surname + ' ' + SUBSTRING(H.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(H.MiddleName, 0, 2) + '. ', '') " +
                                "+ ' [' + I.PrefixName + REPLACE(STR(H.StudentID, I.PrefixDigits), SPACE(1), '0') + ']'  AS StudentName, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, A.ClassListID, G.CATID, H.Surname ORDER BY A.SchID, " +
                                "A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID) AS SN " +
                                "FROM ACDSbjAllocationStudents A " +
                                "LEFT OUTER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification C ON C.SbjClassID = B.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept D ON D.SbjDeptID = B.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassGroup F ON F.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassList G ON G.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMStudents H ON H.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix I ON I.PrefixID = H.PrefixID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = G.CATID " +
                                "WHERE A.DeleteStatus = 'False' AND B.SubjectStatus = 'True' " +
                                "AND A.SchSession = @SchSession AND A.SbjSelection = @SbjSelection " +
                                "AND A.SchID = @SchID AND A.ClassListID = @ClassListID " +
                                "ORDER BY A.SchID, A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID;";

                            _list = (List<ACDSbjAllocationStudents>)await connection.QueryAsync<ACDSbjAllocationStudents>(sql,
                                new
                                {
                                    _switch.SchSession,
                                    _switch.SbjSelection,
                                    _switch.SchID,
                                    _switch.ClassListID
                                });
                            break;
                        case 4: //Filter By Allocated/Not-Allocated Subjects, School, ClassGroup And ClassList 
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.STDID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "B.SubjectCode, B.Subject, C.SbjClassification, B.SbjDeptID, B.SbjClassID, D.SbjDept, E.School, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END AS SchClass, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "H.Surname, H.FirstName, H.MiddleName, C.SbjClassID, " +
                                "H.Surname + ' ' + SUBSTRING(H.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(H.MiddleName, 0, 2) + '. ', '') " +
                                "+ ' [' + I.PrefixName + REPLACE(STR(H.StudentID, I.PrefixDigits), SPACE(1), '0') + ']'  AS StudentName, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, A.ClassListID, G.CATID, H.Surname ORDER BY A.SchID, " +
                                "A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID) AS SN " +
                                "FROM ACDSbjAllocationStudents A " +
                                "LEFT OUTER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification C ON C.SbjClassID = B.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept D ON D.SbjDeptID = B.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassGroup F ON F.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassList G ON G.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMStudents H ON H.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix I ON I.PrefixID = H.PrefixID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = G.CATID " +
                                "WHERE A.DeleteStatus = 'False' AND B.SubjectStatus = 'True' " +
                                "AND A.SchSession = @SchSession AND A.SbjSelection = @SbjSelection " +
                                "AND A.SchID = @SchID AND A.ClassListID = @ClassListID AND A.ClassID = @ClassID " +
                                "ORDER BY A.SchID, A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID;";

                            _list = (List<ACDSbjAllocationStudents>)await connection.QueryAsync<ACDSbjAllocationStudents>(sql,
                                new
                                {
                                    _switch.SchSession,
                                    _switch.SbjSelection,
                                    _switch.SchID,
                                    _switch.ClassListID,
                                    _switch.ClassID
                                });
                            break;
                        case 5: //Filter By Allocated/Not-Allocated Subjects, School, Class Group, Class List And Student
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.STDID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "B.SubjectCode, B.Subject, C.SbjClassification, B.SbjDeptID, B.SbjClassID, D.SbjDept, E.School, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END AS SchClass, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "H.Surname, H.FirstName, H.MiddleName, C.SbjClassID, " +
                                "H.Surname + ' ' + SUBSTRING(H.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(H.MiddleName, 0, 2) + '. ', '') " +
                                "+ ' [' + I.PrefixName + REPLACE(STR(H.StudentID, I.PrefixDigits), SPACE(1), '0') + ']'  AS StudentName, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, A.ClassListID, G.CATID, H.Surname ORDER BY A.SchID, " +
                                "A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID) AS SN " +
                                "FROM ACDSbjAllocationStudents A " +
                                "LEFT OUTER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification C ON C.SbjClassID = B.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept D ON D.SbjDeptID = B.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassGroup F ON F.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassList G ON G.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMStudents H ON H.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix I ON I.PrefixID = H.PrefixID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = G.CATID " +
                                "WHERE A.DeleteStatus = 'False' AND B.SubjectStatus = 'True' " +
                                "AND A.SchSession = @SchSession AND A.SbjSelection = @SbjSelection " +
                                "AND A.SchID = @SchID AND A.ClassListID = @ClassListID AND A.ClassID = @ClassID " +
                                "AND A.STDID = @STDID ORDER BY A.SchID, A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID;";

                            _list = (List<ACDSbjAllocationStudents>)await connection.QueryAsync<ACDSbjAllocationStudents>(sql,
                                new
                                {
                                    _switch.SchSession,
                                    _switch.SbjSelection,
                                    _switch.SchID,
                                    _switch.ClassListID,
                                    _switch.ClassID,
                                    _switch.STDID
                                });
                            break;
                        case 6: //Filter By Allocated Subjects For All Students
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.STDID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "B.SubjectCode, B.Subject, C.SbjClassification, B.SbjDeptID, B.SbjClassID, D.SbjDept, E.School, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END AS SchClass, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "H.Surname, H.FirstName, H.MiddleName, C.SbjClassID, " +
                                "H.Surname + ' ' + SUBSTRING(H.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(H.MiddleName, 0, 2) + '. ', '') " +
                                "+ ' [' + I.PrefixName + REPLACE(STR(H.StudentID, I.PrefixDigits), SPACE(1), '0') + ']'  AS StudentName, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, A.ClassListID, G.CATID, H.Surname ORDER BY A.SchID, " +
                                "A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID) AS SN " +
                                "FROM ACDSbjAllocationStudents A " +
                                "LEFT OUTER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification C ON C.SbjClassID = B.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept D ON D.SbjDeptID = B.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassGroup F ON F.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassList G ON G.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMStudents H ON H.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix I ON I.PrefixID = H.PrefixID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = G.CATID " +
                                "WHERE A.DeleteStatus = 'False' AND AND B.SubjectStatus = 'True' " +
                                "AND A.SchSession AND @SchSession AND A.SbjSelection = @SbjSelection " +
                                "ORDER BY A.SchID, A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID;";

                            _list = (List<ACDSbjAllocationStudents>)await connection.QueryAsync<ACDSbjAllocationStudents>(sql,
                                new
                                {
                                    _switch.SchSession,
                                    SbjSelection = true
                                });
                            break;
                        case 7: //Filter By Allocated/Not-Allocated Subjects, School, Class Group, Class List And Student
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.STDID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "B.SubjectCode, B.Subject, C.SbjClassification, B.SbjDeptID, B.SbjClassID, D.SbjDept, E.School, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END AS SchClass, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "H.Surname, H.FirstName, H.MiddleName, C.SbjClassID, " +
                                "H.Surname + ' ' + SUBSTRING(H.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(H.MiddleName, 0, 2) + '. ', '') " +
                                "+ ' [' + I.PrefixName + REPLACE(STR(H.StudentID, I.PrefixDigits), SPACE(1), '0') + ']'  AS StudentName, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, A.ClassListID, G.CATID, H.Surname ORDER BY A.SchID, " +
                                "A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID) AS SN " +
                                "FROM ACDSbjAllocationStudents A " +
                                "LEFT OUTER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification C ON C.SbjClassID = B.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept D ON D.SbjDeptID = B.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassGroup F ON F.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassList G ON G.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMStudents H ON H.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix I ON I.PrefixID = H.PrefixID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = G.CATID " +
                                "WHERE A.DeleteStatus = 'False' AND B.SubjectStatus = 'True' " +
                                "AND A.SbjSelection = @SbjSelection AND A.SchSession = @SchSession " +
                                "AND A.SchID = @SchID AND A.ClassID = @ClassID AND A.SubjectID = @SubjectID " +
                                "AND A.STDID = @STDID " +
                                "ORDER BY A.SchID, A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID;";

                            _list = (List<ACDSbjAllocationStudents>)await connection.QueryAsync<ACDSbjAllocationStudents>(sql,
                                new
                                {
                                    _switch.SbjSelection,
                                    _switch.SchSession,
                                    _switch.SchID,
                                    _switch.ClassID,
                                    _switch.SubjectID,
                                    _switch.STDID
                                });
                            break;
                        case 8: //Filter By Allocated/Not-Allocated Subjects, School, Class Group, Class List And Student
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.STDID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "B.SubjectCode, B.Subject, C.SbjClassification, B.SbjDeptID, B.SbjClassID, D.SbjDept, E.School, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END AS SchClass, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "H.Surname, H.FirstName, H.MiddleName, C.SbjClassID, " +
                                "H.Surname + ' ' + SUBSTRING(H.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(H.MiddleName, 0, 2) + '. ', '') " +
                                "+ ' [' + I.PrefixName + REPLACE(STR(H.StudentID, I.PrefixDigits), SPACE(1), '0') + ']'  AS StudentName, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, A.ClassListID, G.CATID, H.Surname ORDER BY A.SchID, " +
                                "A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID) AS SN " +
                                "FROM ACDSbjAllocationStudents A " +
                                "LEFT OUTER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification C ON C.SbjClassID = B.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept D ON D.SbjDeptID = B.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassGroup F ON F.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassList G ON G.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMStudents H ON H.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix I ON I.PrefixID = H.PrefixID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = G.CATID " +
                                "WHERE A.DeleteStatus = 'False' AND A.SchSession = @SchSession " +
                                "AND A.SchID = @SchID AND A.ClassID = @ClassID AND " +
                                "A.SubjectID = @SubjectID AND A.STDID = @STDID " +
                                "ORDER BY A.SchID, A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID;";

                            _list = (List<ACDSbjAllocationStudents>)await connection.QueryAsync<ACDSbjAllocationStudents>(sql,
                                new
                                {
                                    _switch.SchSession,
                                    _switch.SchID,
                                    _switch.ClassID,
                                    _switch.SubjectID,
                                    _switch.STDID
                                });
                            break;
                        case 9: //Retrieve All Students Allocated PsychoMotor Subjects 
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.STDID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "B.SubjectCode, B.Subject, C.SbjClassification, B.SbjDeptID, B.SbjClassID, D.SbjDept, E.School, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END AS SchClass, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "H.Surname, H.FirstName, H.MiddleName, C.SbjClassID, " +
                                "H.Surname + ' ' + SUBSTRING(H.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(H.MiddleName, 0, 2) + '. ', '') " +
                                "+ ' [' + I.PrefixName + REPLACE(STR(H.StudentID, I.PrefixDigits), SPACE(1), '0') + ']'  AS StudentName, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, A.ClassListID, G.CATID, H.Surname ORDER BY A.SchID, " +
                                "A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID) AS SN " +
                                "FROM ACDSbjAllocationStudents A " +
                                "LEFT OUTER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification C ON C.SbjClassID = B.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept D ON D.SbjDeptID = B.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassGroup F ON F.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassList G ON G.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMStudents H ON H.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix I ON I.PrefixID = H.PrefixID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = G.CATID " +
                                "WHERE A.DeleteStatus = 'False' AND B.SubjectStatus = 'True' " +
                                "AND A.SchSession = @SchSession AND A.SbjSelection = @SbjSelection " +
                                "AND A.SchID = @SchID AND B.SbjClassID = @SbjClassID AND A.STDID = @STDID " +
                                "ORDER BY A.SchID, A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID;";

                            _list = (List<ACDSbjAllocationStudents>)await connection.QueryAsync<ACDSbjAllocationStudents>(sql,
                                new
                                {
                                    _switch.SchSession,
                                    _switch.SchID,
                                    SbjClassID = _switch.ClassID,  //SbjClassID was Omitted In API Call, So I use _switch.ClassID here in place of _switch.SbjClassID
                                    _switch.STDID,
                                    SbjSelection = true
                                });
                            break;
                        case 10: //Retrieve Selected Student Allocated Subjects
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.STDID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "B.SubjectCode, B.Subject, C.SbjClassification, B.SbjDeptID, B.SbjClassID, D.SbjDept, E.School, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END AS SchClass, " +
                                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                                "H.Surname, H.FirstName, H.MiddleName, C.SbjClassID, " +
                                "H.Surname + ' ' + SUBSTRING(H.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(H.MiddleName, 0, 2) + '. ', '') " +
                                "+ ' [' + I.PrefixName + REPLACE(STR(H.StudentID, I.PrefixDigits), SPACE(1), '0') + ']'  AS StudentName, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, A.ClassListID, G.CATID, H.Surname ORDER BY A.SchID, " +
                                "A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID) AS SN " +
                                "FROM ACDSbjAllocationStudents A " +
                                "LEFT OUTER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification C ON C.SbjClassID = B.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept D ON D.SbjDeptID = B.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList E ON E.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassGroup F ON F.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMSchClassList G ON G.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMStudents H ON H.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix I ON I.PrefixID = H.PrefixID " +
                                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = G.CATID " +
                                "WHERE A.DeleteStatus = 'False' AND A.SchSession = @SchSession " +
                                "AND A.SbjSelection = @SbjSelection AND B.SbjClassID = @SbjClassID AND A.STDID = @STDID " +
                                "ORDER BY A.SchID, A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID;";

                            _list = (List<ACDSbjAllocationStudents>)await connection.QueryAsync<ACDSbjAllocationStudents>(sql,
                                new
                                {
                                    _switch.SbjSelection,
                                    _switch.SchSession,
                                    SbjClassID = _switch.ClassID,
                                    _switch.STDID
                                });
                            break;
                        case 11: //Filter By Allocated/Not-Allocated Subjects, School, Class Group, Class List And Student
                            sql = "SELECT A.SbjAllocID, A.SchSession, A.STDID, A.SubjectID, A.SchID, A.ClassID, B.SbjClassID " +
                                "FROM ACDSbjAllocationStudents A INNER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                                "WHERE DeleteStatus = 'False' AND A.SbjSelection = 1 AND A.SchSession = @SchSession;";

                            _list = (List<ACDSbjAllocationStudents>)await connection.QueryAsync<ACDSbjAllocationStudents>(sql,
                                new
                                {
                                    _switch.SchSession                                    
                                });
                            break;
                        default: //Filter By Both Allocated And Not-Allocated Subjects
                            //sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.STDID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                            //    "B.SubjectCode, B.Subject, C.SbjClassification, B.SbjDeptID, B.SbjClassID, D.SbjDept, E.School, " +
                            //    "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END AS SchClass, " +
                            //    "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                            //    "H.Surname, H.FirstName, H.MiddleName, C.SbjClassID, " +
                            //    "H.Surname + ' ' + SUBSTRING(H.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(H.MiddleName, 0, 2) + '. ', '') " +
                            //    "+ ' [' + I.PrefixName + REPLACE(STR(H.StudentID, I.PrefixDigits), SPACE(1), '0') + ']'  AS StudentName, " +
                            //    "ROW_NUMBER() OVER (PARTITION BY A.SchID, A.ClassListID, G.CATID, H.Surname ORDER BY A.SchID, " +
                            //    "A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID) AS SN " +
                            //    "FROM ACDSbjAllocationStudents A " +
                            //    "LEFT OUTER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                            //    "LEFT OUTER JOIN ACDSbjClassification C ON C.SbjClassID = B.SbjClassID " +
                            //    "LEFT OUTER JOIN ACDSbjDept D ON D.SbjDeptID = B.SbjDeptID " +
                            //    "LEFT OUTER JOIN ADMSchlList E ON E.SchID = A.SchID " +
                            //    "LEFT OUTER JOIN ADMSchClassGroup F ON F.ClassListID = A.ClassListID " +
                            //    "LEFT OUTER JOIN ADMSchClassList G ON G.ClassID = A.ClassID " +
                            //    "LEFT OUTER JOIN ADMStudents H ON H.STDID = A.STDID " +
                            //    "LEFT OUTER JOIN SETPrefix I ON I.PrefixID = H.PrefixID " +
                            //    "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = G.CATID " +
                            //    "WHERE A.DeleteStatus = 'False' AND B.SubjectStatus = 'True' " +
                            //    "AND A.SchSession = @SchSession ORDER BY A.SchID, A.ClassListID, G.CATID, H.Surname, B.SbjClassID, B.SortID;";
                            //sql = "SELECT A.SbjAllocID, A.STDID, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, B.SubjectCode, " +
                            //    "B.Subject, H.Surname, H.FirstName, H.MiddleName, A.SbjSelection, " +
                            //    "H.Surname + ' ' + SUBSTRING(H.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(H.MiddleName, 0, 2) + '. ', '') " +
                            //    "+ ' [' + I.PrefixName + REPLACE(STR(H.StudentID, I.PrefixDigits), SPACE(1), '0') + ']'  AS StudentName " +
                            //    "FROM ACDSbjAllocationStudents A " +
                            //    "LEFT OUTER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                            //    "LEFT OUTER JOIN ADMStudents H ON H.STDID = A.STDID " +
                            //    "LEFT OUTER JOIN SETPrefix I ON I.PrefixID = H.PrefixID " +
                            //    "WHERE A.DeleteStatus = 'False' AND B.SubjectStatus = 'True' " +
                            //    "AND A.SchSession = @SchSession ORDER BY A.SchID, A.ClassListID, H.Surname;";
                            sql = "SELECT A.SbjAllocID, A.STDID, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, A.SbjSelection, " +
                                "B.SubjectCode, B.Subject, " +
                                "H.Surname + ' ' + SUBSTRING(H.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(H.MiddleName, 0, 2) + '. ', '') " +
                                "+ ' [' + I.PrefixName + REPLACE(STR(H.StudentID, I.PrefixDigits), SPACE(1), '0') + ']'  AS StudentName " +
                                "FROM ACDSbjAllocationStudents A " +
                                "LEFT OUTER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMStudents H ON H.STDID = A.STDID " +
                                "LEFT OUTER JOIN SETPrefix I ON I.PrefixID = H.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND B.SubjectStatus = 'True' " +
                                "AND A.SchSession = @SchSession ORDER BY A.SchID, A.ClassListID, H.Surname;";

                            _list = (List<ACDSbjAllocationStudents>)await connection.QueryAsync<ACDSbjAllocationStudents>(sql,
                                new
                                {
                                    _switch.SchSession
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

        public async Task<ACDSbjAllocationStudents> GetByIdAsync(int id)
        {
            _details = new ACDSbjAllocationStudents();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.TermID, A.SchSession, A.STDID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                "B.SubjectCode, B.Subject, C.SbjClassification, D.SbjDept, E.School, " +
                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END AS SchClass, " +
                "CASE WHEN F.UseConvension = 'True' THEN F.ConvensionalName ELSE F.SchClass END + ' ' + R.CATName AS ClassName, " +
                "H.Surname, H.FirstName, H.MiddleName, C.SbjClassID, " +
                "H.Surname + ' ' + SUBSTRING(H.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(H.MiddleName, 0, 2) + '. ', '') " +
                "+ ' [' + I.PrefixName + REPLACE(STR(H.StudentID, I.PrefixDigits), SPACE(1), '0') + ']'  AS StudentName, " +
                "ROW_NUMBER() OVER (PARTITION BY B.SbjClassID ORDER BY B.SortID) AS SN " +
                "FROM ACDSbjAllocationStudents A " +
                "INNER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                "LEFT OUTER JOIN ACDSbjClassification C ON C.SbjClassID = B.SbjClassID " +
                "LEFT OUTER JOIN ACDSbjDept D ON D.SbjDeptID = B.SbjDeptID " +
                "INNER JOIN ADMSchlList E ON E.SchID = A.SchID " +
                "INNER JOIN ADMSchClassGroup F ON F.ClassListID = A.ClassListID " +
                "INNER JOIN ADMSchClassList G ON G.ClassID = A.ClassID " +
                "INNER JOIN ADMStudents H ON H.STDID = A.STDID " +
                "LEFT OUTER JOIN SETPrefix I ON I.PrefixID = H.PrefixID " +
                "LEFT OUTER JOIN ADMSchClassCategory R ON R.CATID = G.CATID " +
                "WHERE A.SbjAllocID = @SbjAllocID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDSbjAllocationStudents>(sql, new { SbjAllocID = id });
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

        public async Task<ACDSbjAllocationStudents> AddAsync(ACDSbjAllocationStudents entity)
        {
            sql = @"INSERT INTO ACDSbjAllocationStudents (TermID, SchSession, STDID, SbjSelection, SubjectID, SchID, ClassID, ClassListID) 
                    OUTPUT INSERTED.SbjAllocID VALUES (@TermID, @SchSession, @STDID, @SbjSelection, @SubjectID, @SchID, @ClassID, @ClassListID);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.SbjAllocID = result;
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

        public async Task<ACDSbjAllocationStudents> UpdateAsync(int id, ACDSbjAllocationStudents entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1: //Update Allocation From Student Subject Allocation
                    sql = "UPDATE ACDSbjAllocationStudents SET SbjSelection = @SbjSelection WHERE SbjAllocID = @SbjAllocID;";
                    break;
                case 2: //Update Allocation When Student Class Change
                    sql = "UPDATE ACDSbjAllocationStudents SET SchID = @SchID, ClassID = @ClassID, ClassListID = @ClassListID " +
                        "WHERE SchSession = @SchSession AND STDID = @STDID;";
                    break;
                case 3:
                    sql = "UPDATE ACDSbjAllocationStudents SET Id = @Id WHERE SbjAllocID = @SbjAllocID;";
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
            sql = "DELETE FROM ACDSbjAllocationStudents WHERE SbjAllocID = @SbjAllocID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { SbjAllocID = id });
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

        public Task<IReadOnlyList<ACDSbjAllocationStudents>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ACDTeacherSubjectsAllocationRepsitory : IACDTeacherSubjectsAllocationRepsitory
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ACDSbjAllocationTeachers> _list { get; set; }
        ACDSbjAllocationTeachers _details = new();

        public ACDTeacherSubjectsAllocationRepsitory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ACDSbjAllocationTeachers>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: //Filter By School
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.StaffID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "A.StaffID_ClassTeacher, A.StaffID_Principal, B.Surname, " +
                                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                                "D.SubjectCode, D.Subject, D.SortID, D.SbjClassID, E.SbjClassification, D.SbjDeptID, F.SbjDept, G.School, " +
                                "(CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END) + ' ' + I.CATName AS ClassName, " +
                                "CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END AS ClassGroupName, " +
                                "K.Surname + ' ' + SUBSTRING(K.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(K.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(K.EmployeeID, L.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher, " +
                                "M.Surname + ' ' + SUBSTRING(M.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(M.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(M.EmployeeID, N.PrefixDigits), SPACE(1), '0') + ']'  AS Principal, " +
                                "ROW_NUMBER() OVER (PARTITION BY D.SbjClassID ORDER BY A.ClassListID, H.CATID, D.SortID) AS SN " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification E ON E.SbjClassID = D.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept F ON F.SbjDeptID = D.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList G ON G.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassList H ON H.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory I ON I.CATID = H.CATID " +
                                "LEFT OUTER JOIN ADMSchClassGroup J ON J.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee K ON K.StaffID = A.StaffID_ClassTeacher " +
                                "LEFT OUTER JOIN SETPrefix L ON L.PrefixID = K.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee M ON M.StaffID = A.StaffID_Principal " +
                                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND D.SubjectStatus = 'True' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.SchID = @SchID ORDER BY D.SbjClassID, A.ClassListID, H.CATID, D.SortID;";

                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
                                new
                                {
                                    _switch.SbjSelection,
                                    _switch.TermID,
                                    _switch.SchID
                                });
                            break;
                        case 2: //Filter By School And ClassGroup
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.StaffID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "A.StaffID_ClassTeacher, A.StaffID_Principal, B.Surname, " +
                                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                                "D.SubjectCode, D.Subject, D.SortID, D.SbjClassID, E.SbjClassification, D.SbjDeptID, F.SbjDept, G.School, " +
                                "(CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END) + ' ' + I.CATName AS ClassName, " +
                                "CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END AS ClassGroupName, " +
                                "K.Surname + ' ' + SUBSTRING(K.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(K.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(K.EmployeeID, L.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher, " +
                                "M.Surname + ' ' + SUBSTRING(M.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(M.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(M.EmployeeID, N.PrefixDigits), SPACE(1), '0') + ']'  AS Principal, " +
                                "ROW_NUMBER() OVER (PARTITION BY D.SbjClassID ORDER BY A.ClassListID, H.CATID, D.SortID) AS SN " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification E ON E.SbjClassID = D.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept F ON F.SbjDeptID = D.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList G ON G.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassList H ON H.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory I ON I.CATID = H.CATID " +
                                "LEFT OUTER JOIN ADMSchClassGroup J ON J.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee K ON K.StaffID = A.StaffID_ClassTeacher " +
                                "LEFT OUTER JOIN SETPrefix L ON L.PrefixID = K.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee M ON M.StaffID = A.StaffID_Principal " +
                                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND D.SubjectStatus = 'True' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.SchID = @SchID AND A.ClassListID = @ClassListID " +
                                "ORDER BY D.SbjClassID, A.ClassListID, H.CATID, D.SortID;";

                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
                                new
                                {
                                    _switch.SbjSelection,
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassListID
                                });
                            break;
                        case 3: //Filter By School, ClassGroup And ClassList
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.StaffID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "A.StaffID_ClassTeacher, A.StaffID_Principal, B.Surname, " +
                                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                                "D.SubjectCode, D.Subject, D.SortID, D.SbjClassID, E.SbjClassification, D.SbjDeptID, F.SbjDept, G.School, " +
                                "(CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END) + ' ' + I.CATName AS ClassName, " +
                                "CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END AS ClassGroupName, " +
                                "K.Surname + ' ' + SUBSTRING(K.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(K.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(K.EmployeeID, L.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher, " +
                                "M.Surname + ' ' + SUBSTRING(M.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(M.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(M.EmployeeID, N.PrefixDigits), SPACE(1), '0') + ']'  AS Principal, " +
                                "ROW_NUMBER() OVER (PARTITION BY D.SbjClassID ORDER BY A.ClassListID, H.CATID, D.SortID) AS SN " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification E ON E.SbjClassID = D.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept F ON F.SbjDeptID = D.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList G ON G.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassList H ON H.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory I ON I.CATID = H.CATID " +
                                "LEFT OUTER JOIN ADMSchClassGroup J ON J.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee K ON K.StaffID = A.StaffID_ClassTeacher " +
                                "LEFT OUTER JOIN SETPrefix L ON L.PrefixID = K.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee M ON M.StaffID = A.StaffID_Principal " +
                                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND D.SubjectStatus = 'True' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.SchID = @SchID AND A.ClassListID = @ClassListID AND A.ClassID = @ClassID " +
                                "ORDER BY D.SbjClassID, A.ClassListID, H.CATID, D.SortID;";

                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
                                new
                                {
                                    _switch.SbjSelection,
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassListID,
                                    _switch.ClassID
                                });
                            break;
                        case 4: //Filter By School, ClassGroup, ClassList And Teacher
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.StaffID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "A.StaffID_ClassTeacher, A.StaffID_Principal, B.Surname, " +
                                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                                "D.SubjectCode, D.Subject, D.SortID, D.SbjClassID, E.SbjClassification, D.SbjDeptID, F.SbjDept, G.School, " +
                                "(CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END) + ' ' + I.CATName AS ClassName, " +
                                "CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END AS ClassGroupName, " +
                                "K.Surname + ' ' + SUBSTRING(K.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(K.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(K.EmployeeID, L.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher, " +
                                "M.Surname + ' ' + SUBSTRING(M.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(M.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(M.EmployeeID, N.PrefixDigits), SPACE(1), '0') + ']'  AS Principal, " +
                                "ROW_NUMBER() OVER (PARTITION BY D.SbjClassID ORDER BY A.ClassListID, H.CATID, D.SortID) AS SN " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification E ON E.SbjClassID = D.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept F ON F.SbjDeptID = D.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList G ON G.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassList H ON H.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory I ON I.CATID = H.CATID " +
                                "LEFT OUTER JOIN ADMSchClassGroup J ON J.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee K ON K.StaffID = A.StaffID_ClassTeacher " +
                                "LEFT OUTER JOIN SETPrefix L ON L.PrefixID = K.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee M ON M.StaffID = A.StaffID_Principal " +
                                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND D.SubjectStatus = 'True' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.SchID = @SchID AND A.ClassListID = @ClassListID " +
                                "AND A.ClassID = @ClassID AND A.StaffID = @StaffID ORDER BY D.SbjClassID, A.ClassListID, H.CATID, D.SortID;";

                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
                                new
                                {
                                    _switch.SbjSelection,
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassListID,
                                    _switch.ClassID,
                                    _switch.StaffID
                                });
                            break;
                        case 5: //Filter By School And Teacher
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.StaffID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "A.StaffID_ClassTeacher, A.StaffID_Principal, B.Surname, " +
                                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                                "D.SubjectCode, D.Subject, D.SortID, D.SbjClassID, E.SbjClassification, D.SbjDeptID, F.SbjDept, G.School, " +
                                "(CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END) + ' ' + I.CATName AS ClassName, " +
                                "CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END AS ClassGroupName, " +
                                "K.Surname + ' ' + SUBSTRING(K.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(K.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(K.EmployeeID, L.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher, " +
                                "M.Surname + ' ' + SUBSTRING(M.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(M.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(M.EmployeeID, N.PrefixDigits), SPACE(1), '0') + ']'  AS Principal, " +
                                "ROW_NUMBER() OVER (PARTITION BY D.SbjClassID ORDER BY A.ClassListID, H.CATID, D.SortID) AS SN " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification E ON E.SbjClassID = D.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept F ON F.SbjDeptID = D.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList G ON G.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassList H ON H.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory I ON I.CATID = H.CATID " +
                                "LEFT OUTER JOIN ADMSchClassGroup J ON J.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee K ON K.StaffID = A.StaffID_ClassTeacher " +
                                "LEFT OUTER JOIN SETPrefix L ON L.PrefixID = K.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee M ON M.StaffID = A.StaffID_Principal " +
                                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND D.SubjectStatus = 'True' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.SchID = @SchID AND A.StaffID = @StaffID " +
                                "ORDER BY D.SbjClassID, A.ClassListID, H.CATID, D.SortID;";

                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
                                new
                                {
                                    _switch.SbjSelection,
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.StaffID
                                });
                            break;
                        case 6: //Filter By School, ClassGroup And Teacher
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.StaffID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "A.StaffID_ClassTeacher, A.StaffID_Principal, B.Surname, " +
                                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                                "D.SubjectCode, D.Subject, D.SortID, D.SbjClassID, E.SbjClassification, D.SbjDeptID, F.SbjDept, G.School, " +
                                "(CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END) + ' ' + I.CATName AS ClassName, " +
                                "CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END AS ClassGroupName, " +
                                "K.Surname + ' ' + SUBSTRING(K.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(K.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(K.EmployeeID, L.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher, " +
                                "M.Surname + ' ' + SUBSTRING(M.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(M.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(M.EmployeeID, N.PrefixDigits), SPACE(1), '0') + ']'  AS Principal, " +
                                "ROW_NUMBER() OVER (PARTITION BY D.SbjClassID ORDER BY A.ClassListID, H.CATID, D.SortID) AS SN " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification E ON E.SbjClassID = D.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept F ON F.SbjDeptID = D.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList G ON G.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassList H ON H.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory I ON I.CATID = H.CATID " +
                                "LEFT OUTER JOIN ADMSchClassGroup J ON J.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee K ON K.StaffID = A.StaffID_ClassTeacher " +
                                "LEFT OUTER JOIN SETPrefix L ON L.PrefixID = K.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee M ON M.StaffID = A.StaffID_Principal " +
                                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND D.SubjectStatus = 'True' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.SchID = @SchID AND A.ClassListID = @ClassListID AND A.StaffID = @StaffID " +
                                "ORDER BY D.SbjClassID, A.ClassListID, H.CATID, D.SortID;";

                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
                                new
                                {
                                    _switch.SbjSelection,
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassListID,
                                    _switch.StaffID
                                });
                            break;
                        case 7: //Filter By Teacher
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.StaffID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "A.StaffID_ClassTeacher, A.StaffID_Principal, B.Surname, " +
                                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                                "D.SubjectCode, D.Subject, D.SortID, D.SbjClassID, E.SbjClassification, D.SbjDeptID, F.SbjDept, G.School, " +
                                "(CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END) + ' ' + I.CATName AS ClassName, " +
                                "CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END AS ClassGroupName, " +
                                "K.Surname + ' ' + SUBSTRING(K.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(K.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(K.EmployeeID, L.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher, " +
                                "M.Surname + ' ' + SUBSTRING(M.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(M.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(M.EmployeeID, N.PrefixDigits), SPACE(1), '0') + ']'  AS Principal, " +
                                "ROW_NUMBER() OVER (PARTITION BY D.SbjClassID ORDER BY A.ClassListID, H.CATID, D.SortID) AS SN " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification E ON E.SbjClassID = D.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept F ON F.SbjDeptID = D.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList G ON G.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassList H ON H.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory I ON I.CATID = H.CATID " +
                                "LEFT OUTER JOIN ADMSchClassGroup J ON J.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee K ON K.StaffID = A.StaffID_ClassTeacher " +
                                "LEFT OUTER JOIN SETPrefix L ON L.PrefixID = K.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee M ON M.StaffID = A.StaffID_Principal " +
                                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND D.SubjectStatus = 'True' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.StaffID = @StaffID ORDER BY D.SbjClassID, A.ClassListID, H.CATID, D.SortID;";

                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
                                new
                                {
                                    _switch.SbjSelection,
                                    _switch.TermID,
                                    _switch.StaffID
                                });
                            break;
                        case 8: //Filter By Class And Teacher For Allocated Subjects
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.StaffID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "A.StaffID_ClassTeacher, A.StaffID_Principal, B.Surname, " +
                                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                                "D.SubjectCode, D.Subject, D.SortID, D.SbjClassID, E.SbjClassification, D.SbjDeptID, F.SbjDept, G.School, " +
                                "(CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END) + ' ' + I.CATName AS ClassName, " +
                                "CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END AS ClassGroupName, " +
                                "K.Surname + ' ' + SUBSTRING(K.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(K.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(K.EmployeeID, L.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher, " +
                                "M.Surname + ' ' + SUBSTRING(M.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(M.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(M.EmployeeID, N.PrefixDigits), SPACE(1), '0') + ']'  AS Principal, " +
                                "ROW_NUMBER() OVER (PARTITION BY D.SbjClassID ORDER BY A.ClassListID, H.CATID, D.SortID) AS SN " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification E ON E.SbjClassID = D.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept F ON F.SbjDeptID = D.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList G ON G.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassList H ON H.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory I ON I.CATID = H.CATID " +
                                "LEFT OUTER JOIN ADMSchClassGroup J ON J.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee K ON K.StaffID = A.StaffID_ClassTeacher " +
                                "LEFT OUTER JOIN SETPrefix L ON L.PrefixID = K.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee M ON M.StaffID = A.StaffID_Principal " +
                                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND D.SubjectStatus = 'True' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.SchID = @SchID AND A.ClassListID = @ClassListID " +
                                "AND A.ClassID = @ClassID AND A.StaffID = @StaffID ORDER BY D.SbjClassID, A.ClassListID, H.CATID, D.SortID;";

                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
                                new
                                {
                                    SbjSelection = true,
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassListID,
                                    _switch.ClassID,
                                    _switch.StaffID
                                });
                            break;

                        case 9: //Filter By Allocated/Not-Allocated Subjects
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.StaffID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "A.StaffID_ClassTeacher, A.StaffID_Principal, B.Surname, " +
                                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                                "D.SubjectCode, D.Subject, D.SortID, D.SbjClassID, E.SbjClassification, D.SbjDeptID, F.SbjDept, G.School, " +
                                "(CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END) + ' ' + I.CATName AS ClassName, " +
                                "CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END AS ClassGroupName, " +
                                "K.Surname + ' ' + SUBSTRING(K.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(K.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(K.EmployeeID, L.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher, " +
                                "M.Surname + ' ' + SUBSTRING(M.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(M.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(M.EmployeeID, N.PrefixDigits), SPACE(1), '0') + ']'  AS Principal, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, D.SbjClassID ORDER BY A.ClassListID, H.CATID, D.SortID) AS SN " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification E ON E.SbjClassID = D.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept F ON F.SbjDeptID = D.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList G ON G.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassList H ON H.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory I ON I.CATID = H.CATID " +
                                "LEFT OUTER JOIN ADMSchClassGroup J ON J.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee K ON K.StaffID = A.StaffID_ClassTeacher " +
                                "LEFT OUTER JOIN SETPrefix L ON L.PrefixID = K.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee M ON M.StaffID = A.StaffID_Principal " +
                                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND D.SubjectStatus = 'True' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID ORDER BY D.SbjClassID, A.ClassListID, H.CATID, D.SortID;";

                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
                                new
                                {
                                    _switch.SbjSelection,
                                    _switch.TermID
                                });
                            break;
                        case 10: //Filter By Subject
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.StaffID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "A.StaffID_ClassTeacher, A.StaffID_Principal, B.Surname, " +
                                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                                "D.SubjectCode, D.Subject, D.SortID, D.SbjClassID, E.SbjClassification, D.SbjDeptID, F.SbjDept, G.School, " +
                                "(CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END) + ' ' + I.CATName AS ClassName, " +
                                "CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END AS ClassGroupName, " +
                                "K.Surname + ' ' + SUBSTRING(K.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(K.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(K.EmployeeID, L.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher, " +
                                "M.Surname + ' ' + SUBSTRING(M.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(M.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(M.EmployeeID, N.PrefixDigits), SPACE(1), '0') + ']'  AS Principal, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, D.SbjClassID ORDER BY A.ClassListID, H.CATID, D.SortID) AS SN " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification E ON E.SbjClassID = D.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept F ON F.SbjDeptID = D.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList G ON G.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassList H ON H.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory I ON I.CATID = H.CATID " +
                                "LEFT OUTER JOIN ADMSchClassGroup J ON J.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee K ON K.StaffID = A.StaffID_ClassTeacher " +
                                "LEFT OUTER JOIN SETPrefix L ON L.PrefixID = K.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee M ON M.StaffID = A.StaffID_Principal " +
                                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND D.SubjectStatus = 'True' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.SchID = @SchID AND A.ClassID =@ClassID AND A.SubjectID = @SubjectID " +
                                "ORDER BY D.SbjClassID, A.ClassListID, H.CATID, D.SortID;";

                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
                                new
                                {
                                    _switch.SbjSelection,
                                    _switch.TermID,
                                    _switch.SchID,
                                    _switch.ClassID,
                                    _switch.SubjectID
                                });
                            break;
                        case 11:
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.StaffID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "A.StaffID_ClassTeacher, A.StaffID_Principal, B.Surname, " +
                                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                                "D.SubjectCode, D.Subject, D.SortID, D.SbjClassID, E.SbjClassification, D.SbjDeptID, F.SbjDept, G.School, " +
                                "(CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END) + ' ' + I.CATName AS ClassName, " +
                                "CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END AS ClassGroupName, " +
                                "K.Surname + ' ' + SUBSTRING(K.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(K.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(K.EmployeeID, L.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher, " +
                                "M.Surname + ' ' + SUBSTRING(M.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(M.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(M.EmployeeID, N.PrefixDigits), SPACE(1), '0') + ']'  AS Principal, " +
                                "ROW_NUMBER() OVER (PARTITION BY A.SchID, D.SbjClassID ORDER BY A.ClassListID, H.CATID, D.SortID) AS SN " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification E ON E.SbjClassID = D.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept F ON F.SbjDeptID = D.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList G ON G.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassList H ON H.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory I ON I.CATID = H.CATID " +
                                "LEFT OUTER JOIN ADMSchClassGroup J ON J.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee K ON K.StaffID = A.StaffID_ClassTeacher " +
                                "LEFT OUTER JOIN SETPrefix L ON L.PrefixID = K.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee M ON M.StaffID = A.StaffID_Principal " +
                                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND D.SubjectStatus = 'True' AND A.SbjSelection = @SbjSelection " +
                                "AND A.TermID = @TermID AND A.SchID = @SchID AND D.SbjClassID = @SbjClassID AND A.StaffID = @StaffID " +
                                "ORDER BY D.SbjClassID, A.ClassListID, H.CATID, D.SortID;";

                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
                                new
                                {
                                    _switch.SbjSelection,
                                    _switch.TermID,
                                    _switch.SchID,
                                    SbjClassID = _switch.ClassID,  //SbjClassID was Omitted In API Call, So I use _switch.ClassID here in place of _switch.SbjClassID
                                    _switch.StaffID
                                });
                            break;
                        case 12: //Distinct Class and Class Teacher
                            sql = "SELECT DISTINCT A.ClassID, A.StaffID_ClassTeacher," +
                                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "INNER JOIN ADMEmployee B ON B.StaffID = A.StaffID_ClassTeacher " +
                                "INNER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                                "WHERE A.TermID = @TermID AND A.SbjSelection = @SbjSelection;";
                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
                               new
                               {
                                   _switch.TermID,
                                   _switch.SbjSelection
                               });
                            break;
                        case 13: //Filter By All Subjects for Selected Term - Either Allocated Or Non-Allocated
                            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.StaffID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                                "A.StaffID_ClassTeacher, A.StaffID_Principal, B.Surname, " +
                                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                                "D.SubjectCode, D.Subject, D.SortID, D.SbjClassID, E.SbjClassification, D.SbjDeptID, F.SbjDept, G.School, " +
                                "(CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END) + ' ' + I.CATName AS ClassName, " +
                                "CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END AS ClassGroupName, " +
                                "K.Surname + ' ' + SUBSTRING(K.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(K.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(K.EmployeeID, L.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher, " +
                                "M.Surname + ' ' + SUBSTRING(M.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(M.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(M.EmployeeID, N.PrefixDigits), SPACE(1), '0') + ']'  AS Principal, " +
                                "ROW_NUMBER() OVER (PARTITION BY D.SbjClassID ORDER BY A.ClassListID, H.CATID, D.SortID) AS SN " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ACDSbjClassification E ON E.SbjClassID = D.SbjClassID " +
                                "LEFT OUTER JOIN ACDSbjDept F ON F.SbjDeptID = D.SbjDeptID " +
                                "LEFT OUTER JOIN ADMSchlList G ON G.SchID = A.SchID " +
                                "LEFT OUTER JOIN ADMSchClassList H ON H.ClassID = A.ClassID " +
                                "LEFT OUTER JOIN ADMSchClassCategory I ON I.CATID = H.CATID " +
                                "LEFT OUTER JOIN ADMSchClassGroup J ON J.ClassListID = A.ClassListID " +
                                "LEFT OUTER JOIN ADMEmployee K ON K.StaffID = A.StaffID_ClassTeacher " +
                                "LEFT OUTER JOIN SETPrefix L ON L.PrefixID = K.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee M ON M.StaffID = A.StaffID_Principal " +
                                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND A.TermID = @TermID AND A.SbjSelection = @SbjSelection " +
                                "ORDER BY D.SbjClassID, A.ClassListID, H.CATID, D.SortID;";

                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.SbjSelection
                                });
                            break;
                        case 14: //Teachers Subject Allocation for Cognitive Mark Entry for Administrator
                            sql = "SELECT A.SbjAllocID, A.TermID, A.StaffID, A.SubjectID, A.SchID, A.ClassID, D.SubjectCode, " +
                                "D.Subject, D.SortID, D.SbjClassID, A.StaffID_ClassTeacher, " +
                                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                                "E.Surname + ' ' + SUBSTRING(E.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(E.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(E.EmployeeID, F.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMEmployee E ON E.StaffID = A.StaffID_ClassTeacher " +
                                "LEFT OUTER JOIN SETPrefix F ON F.PrefixID = E.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND D.SubjectStatus = 'True' " +
                                "AND A.SbjSelection = 1 AND A.TermID = @TermID ORDER BY D.SortID;";

                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
                                new
                                {
                                    _switch.TermID
                                });
                            break;
                        case 15: //Teachers Subject Allocation for Cognitive Mark Entry for Subject Teacher
                            sql = "SELECT A.SbjAllocID, A.TermID, A.StaffID, A.SubjectID, A.SchID, A.ClassID, D.SubjectCode, " +
                                "D.Subject, D.SortID, D.SbjClassID, A.StaffID_ClassTeacher, " +
                                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                                "E.Surname + ' ' + SUBSTRING(E.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(E.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(E.EmployeeID, F.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                                "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                                "LEFT OUTER JOIN ADMEmployee E ON E.StaffID = A.StaffID_ClassTeacher " +
                                "LEFT OUTER JOIN SETPrefix F ON F.PrefixID = E.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND D.SubjectStatus = 'True' " +
                                "AND A.SbjSelection = 1 AND A.TermID = @TermID AND A.StaffID = @StaffID ORDER BY D.SortID;";

                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
                                new
                                {
                                    _switch.TermID,
                                    _switch.StaffID
                                });
                            break;                        
                        default: //Filter By All Subjects for Both Allocated And Non-Allocated
                            //sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.StaffID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                            //    "A.StaffID_ClassTeacher, A.StaffID_Principal, B.Surname, " +
                            //    "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                            //    "D.SubjectCode, D.Subject, D.SortID, D.SbjClassID, E.SbjClassification, D.SbjDeptID, F.SbjDept, G.School, " +
                            //    "(CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END) + ' ' + I.CATName AS ClassName, " +
                            //    "CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END AS ClassGroupName, " +
                            //    "K.Surname + ' ' + SUBSTRING(K.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(K.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(K.EmployeeID, L.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher, " +
                            //    "M.Surname + ' ' + SUBSTRING(M.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(M.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(M.EmployeeID, N.PrefixDigits), SPACE(1), '0') + ']'  AS Principal, " +
                            //    "ROW_NUMBER() OVER (PARTITION BY D.SbjClassID ORDER BY A.ClassListID, H.CATID, D.SortID) AS SN " +
                            //    "FROM ACDSbjAllocationTeachers A " +
                            //    "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                            //    "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                            //    "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                            //    "LEFT OUTER JOIN ACDSbjClassification E ON E.SbjClassID = D.SbjClassID " +
                            //    "LEFT OUTER JOIN ACDSbjDept F ON F.SbjDeptID = D.SbjDeptID " +
                            //    "LEFT OUTER JOIN ADMSchlList G ON G.SchID = A.SchID " +
                            //    "LEFT OUTER JOIN ADMSchClassList H ON H.ClassID = A.ClassID " +
                            //    "LEFT OUTER JOIN ADMSchClassCategory I ON I.CATID = H.CATID " +
                            //    "LEFT OUTER JOIN ADMSchClassGroup J ON J.ClassListID = A.ClassListID " +
                            //    "LEFT OUTER JOIN ADMEmployee K ON K.StaffID = A.StaffID_ClassTeacher " +
                            //    "LEFT OUTER JOIN SETPrefix L ON L.PrefixID = K.PrefixID " +
                            //    "LEFT OUTER JOIN ADMEmployee M ON M.StaffID = A.StaffID_Principal " +
                            //    "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                            //    "WHERE A.DeleteStatus = 'False' AND A.TermID = @TermID " +
                            //    "ORDER BY D.SbjClassID, A.ClassListID, H.CATID, D.SortID;";

                            sql = "SELECT A.SbjAllocID, A.StaffID, A.SubjectID, A.SchID, A.ClassID, A.TermID, A.ClassListID, " +
                                "A.SbjSelection, B.SubjectCode, B.Subject, B.SbjClassID, " +
                                "C.Surname + ' ' + SUBSTRING(C.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(C.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(C.EmployeeID, D.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher " +
                                "FROM ACDSbjAllocationTeachers A " +
                                "INNER JOIN ACDSubjects B ON B.SubjectID = A.SubjectID " +
                                "INNER JOIN ADMEmployee C ON C.StaffID = A.StaffID " +
                                "INNER JOIN SETPrefix D ON D.PrefixID = C.PrefixID " +
                                "WHERE A.DeleteStatus = 'False' AND A.TermID = @TermID " +
                                "ORDER BY B.SbjClassID, A.SchID, A.ClassListID, C.Surname;";

                            _list = (List<ACDSbjAllocationTeachers>)await connection.QueryAsync<ACDSbjAllocationTeachers>(sql,
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

        public async Task<ACDSbjAllocationTeachers> GetByIdAsync(int id)
        {
            _details = new ACDSbjAllocationTeachers();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.SbjAllocID, A.TermID, A.SchSession, A.StaffID, A.SbjSelection, A.SubjectID, A.SchID, A.ClassID, A.ClassListID, " +
                "A.StaffID_ClassTeacher, A.StaffID_Principal, B.Surname, " +
                "B.Surname + ' ' + SUBSTRING(B.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(B.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(B.EmployeeID, C.PrefixDigits), SPACE(1), '0') + ']'  AS SubjectTeacher, " +
                "D.SubjectCode, D.Subject, D.SortID, D.SbjClassID, E.SbjClassification, D.SbjDeptID, F.SbjDept, G.School, " +
                "(CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END) + ' ' + I.CATName AS ClassName, " +
                "CASE WHEN J.UseConvension = 'True' THEN J.ConvensionalName ELSE J.SchClass END AS ClassGroupName, " +
                "K.Surname + ' ' + SUBSTRING(K.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(K.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(K.EmployeeID, L.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacher, " +
                "M.Surname + ' ' + SUBSTRING(M.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(M.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(M.EmployeeID, N.PrefixDigits), SPACE(1), '0') + ']'  AS Principal, " +
                "ROW_NUMBER() OVER (PARTITION BY D.SbjClassID ORDER BY D.SbjDeptID, D.SortID) AS SN " +
                "FROM ACDSbjAllocationTeachers A " +
                "LEFT OUTER JOIN ADMEmployee B ON B.StaffID = A.StaffID " +
                "LEFT OUTER JOIN SETPrefix C ON C.PrefixID = B.PrefixID " +
                "LEFT OUTER JOIN ACDSubjects D ON D.SubjectID = A.SubjectID " +
                "LEFT OUTER JOIN ACDSbjClassification E ON E.SbjClassID = D.SbjClassID " +
                "LEFT OUTER JOIN ACDSbjDept F ON F.SbjDeptID = D.SbjDeptID " +
                "LEFT OUTER JOIN ADMSchlList G ON G.SchID = A.SchID " +
                "LEFT OUTER JOIN ADMSchClassList H ON H.ClassID = A.ClassID " +
                "LEFT OUTER JOIN ADMSchClassCategory I ON I.CATID = H.CATID " +
                "LEFT OUTER JOIN ADMSchClassGroup J ON J.ClassListID = A.ClassListID " +
                "LEFT OUTER JOIN ADMEmployee K ON K.StaffID = A.StaffID_ClassTeacher " +
                "LEFT OUTER JOIN SETPrefix L ON L.PrefixID = K.PrefixID " +
                "LEFT OUTER JOIN ADMEmployee M ON M.StaffID = A.StaffID_Principal " +
                "LEFT OUTER JOIN SETPrefix N ON N.PrefixID = M.PrefixID " +
                "WHERE A.SbjAllocID = @SbjAllocID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<ACDSbjAllocationTeachers>(sql, new { SbjAllocID = id });
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

        public async Task<ACDSbjAllocationTeachers> AddAsync(ACDSbjAllocationTeachers entity)
        {
            sql = @"INSERT INTO ACDSbjAllocationTeachers (TermID, SchSession, StaffID, SbjSelection, SubjectID, SchID, ClassID, ClassListID, 
                    StaffID_ClassTeacher, StaffID_Principal) OUTPUT INSERTED.SbjAllocID VALUES 
                    (@TermID, @SchSession, @StaffID, @SbjSelection, @SubjectID, @SchID, @ClassID, @ClassListID, 
                    @StaffID_ClassTeacher, @StaffID_Principal);SELECT CAST(SCOPE_IDENTITY() as int)";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.SbjAllocID = result;
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

        public async Task<ACDSbjAllocationTeachers> UpdateAsync(int id, ACDSbjAllocationTeachers entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1: //Update Selected Subject Allocation
                    sql = "UPDATE ACDSbjAllocationTeachers SET SbjSelection = @SbjSelection, StaffID = @StaffID " +
                        "WHERE SbjAllocID = @SbjAllocID;";
                    break;
                case 2: //Update Class Teacher ID On Change of Class Teacher
                    sql = "UPDATE ACDSbjAllocationTeachers SET StaffID_ClassTeacher = @StaffID_ClassTeacher " +
                        "WHERE TermID = @TermID AND SchID = @SchID AND ClassID = @ClassID;";
                    break;
                case 3: //Update Principal ID On Change of School Principal
                    sql = "UPDATE ACDSbjAllocationTeachers SET StaffID_Principal = @StaffID_Principal " +
                        "WHERE TermID = @TermID AND SchID = @SchID;";
                    break;
                case 4: //Update Class Teacher ID On Change of Class Teacher For Pyscho & Others
                    sql = "UPDATE ACDSbjAllocationTeachers SET StaffID = @StaffID FROM  ACDSbjAllocationTeachers A " +
                        "INNER JOIN ACDSubjects B ON A.SubjectID = B.SubjectID " +
                        "WHERE A.TermID = @TermID AND A.SchID = @SchID AND A.ClassID = @ClassID AND B.SbjClassID > 1;";
                    break;
                case 5:
                    sql = "UPDATE ACDSbjAllocationTeachers SET Id = @Id WHERE SbjAllocID = @SbjAllocID;";
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
            sql = "DELETE FROM ACDSbjAllocationTeachers WHERE SbjAllocID = @SbjAllocID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { SbjAllocID = id });
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

        public Task<IReadOnlyList<ACDSbjAllocationTeachers>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }
}
