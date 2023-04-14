using WebAppAcademics.Server.Interfaces.School;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Administration.School;
using System.Data;
using System.Data.SqlClient;

namespace WebAppAcademics.Server.Repositories
{
    public class ADMSchlListRepository : IADMSchlListRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMSchlList> schools { get; set; }
        ADMSchlList school = new();

        public ADMSchlListRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ADMSchlList>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT A.SchID, A.School, A.StaffID, A.SchImageID, A.Head, A.HeadCode, A.Status, " +
                                "B.Surname + ' ' + B.FirstName + ' ' + ISNULL(B.MiddleName, ' ') AS SchoolHead, C.Title AS SchoolHeadlTitle, " +
                                "B.Surname + ' ' + B.FirstName + ' ' + ISNULL(B.MiddleName, ' ') + ' [' + D.PrefixName + REPLACE(STR(B.EmployeeID, D.PrefixDigits), SPACE(1), '0') + ']' AS SchoolHeadWithNo " +
                                "FROM ADMSchlList A " +
                                "LEFT OUTER JOIN ADMEmployee B ON A.StaffID = B.StaffID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle C ON C.TitleID = B.TitleID " +
                                "LEFT OUTER JOIN SETPrefix D ON D.PrefixID = B.PrefixID " +
                                "WHERE A.SchID > 1;";
                            break;
                        default:
                            sql = "SELECT A.SchID, A.School, A.StaffID, A.SchImageID, A.Head, A.HeadCode, A.Status, " +
                                "B.Surname + ' ' + B.FirstName + ' ' + ISNULL(B.MiddleName, ' ') AS SchoolHead, C.Title AS SchoolHeadlTitle, " +
                                "B.Surname + ' ' + B.FirstName + ' ' + ISNULL(B.MiddleName, ' ') + ' [' + D.PrefixName + REPLACE(STR(B.EmployeeID, D.PrefixDigits), SPACE(1), '0') + ']' AS SchoolHeadWithNo " +
                                "FROM ADMSchlList A " +
                                "LEFT OUTER JOIN ADMEmployee B ON A.StaffID = B.StaffID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle C ON C.TitleID = B.TitleID " +
                                "LEFT OUTER JOIN SETPrefix D ON D.PrefixID = B.PrefixID " +
                                "WHERE A.Status ='True';";
                            break;
                    }

                    schools = (IReadOnlyList<ADMSchlList>)await connection.QueryAsync<ADMSchlList>(sql, new { });
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

            return schools;
        }

        public async Task<ADMSchlList> GetByIdAsync(int id)
        {
            school = new ADMSchlList();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.SchID, A.School, A.StaffID, A.SchImageID, A.Head, A.HeadCode, A.Status, " +
                "B.Surname + ' ' + B.FirstName + ' ' + ISNULL(B.MiddleName, ' ') AS SchoolHead, C.Title AS SchoolHeadlTitle, " +
                "B.Surname + ' ' + B.FirstName + ' ' + ISNULL(B.MiddleName, ' ') + ' [' + D.PrefixName + REPLACE(STR(B.EmployeeID, D.PrefixDigits), SPACE(1), '0') + ']' AS SchoolHeadWithNo " +
                "FROM ADMSchlList A " +
                "LEFT OUTER JOIN ADMEmployee B ON A.StaffID = B.StaffID " +
                "LEFT OUTER JOIN ADMEmployeeTitle C ON C.TitleID = B.TitleID " +
                "LEFT OUTER JOIN SETPrefix D ON D.PrefixID = B.PrefixID " +
                "WHERE A.SchID = @SchID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                school = await connection.QueryFirstOrDefaultAsync<ADMSchlList>(sql, new { SchID = id });
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

            return school;
        }

        public async Task<ADMSchlList> AddAsync(ADMSchlList entity)
        {
            sql = "INSERT INTO ADMSchlList (School, StaffID, SchImageID, Head, HeadCode, Status) OUTPUT INSERTED.SchID VALUES " +
                        "(@School, @StaffID, @SchImageID, @Head, @HeadCode, @Status);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.SchID = result;
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

        public async Task<ADMSchlList> UpdateAsync(int id, ADMSchlList entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMSchlList SET School = @School, StaffID = @StaffID, Head = @Head, Status = @Status WHERE SchID = @schid;";
                    break;
                case 2:
                    sql = "UPDATE ADMSchlList SET Id = @Id WHERE SchID = @SchID;";
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
            sql = "DELETE FROM ADMSchlList WHERE SchID = @SchID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { SchID = id });                
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

        public Task<IReadOnlyList<ADMSchlList>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ADMSchClassListRepository : IADMSchClassListRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMSchClassList> classList { get; set; }
        ADMSchClassList classDetails = new();

        public ADMSchClassListRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ADMSchClassList>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT A.ClassID, A.SchInfoID, A.SchID, A.ClassListID, A.DisciplineID, A.StaffID, A.CATID, A.FinalYearClass, A.JuniorFinalYearClass, " +
                                "A.CheckPointClass, A.IGCSEClass, B.School, B.StaffID AS PrincipalID, C.SchClass , C.ConvensionalName, C.UseConvension, D.CATName, " +
                                "E.Discipline, (CASE WHEN C.UseConvension = 'True' THEN C.ConvensionalName ELSE C.SchClass END) + ' ' + D.CATName AS ClassName, " +
                                "CASE WHEN C.UseConvension = 'True' THEN C.ConvensionalName ELSE C.SchClass END AS ClassGroupName, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, G.Title AS ClassTeacherTitle, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') + ' [' + H.PrefixName + REPLACE(STR(F.EmployeeID, H.PrefixDigits), SPACE(1), '0') + ']' AS ClassTeacherWithNo, " +
                                "F.Surname + ' ' + SUBSTRING(F.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(F.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(F.EmployeeID, H.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacherInitials, " +
                                "I.Surname + ' ' + I.FirstName + ' ' + ISNULL(I.MiddleName, ' ') AS Principal, J.Title AS PrincipalTitle, " + 
                                "I.Surname + ' ' + I.FirstName + ' ' +  ISNULL(I.MiddleName, ' ')  + ' [' + K.PrefixName + REPLACE(STR(I.EmployeeID, K.PrefixDigits), SPACE(1), '0') + ']' AS PrincipalWithNo, " +
                                "I.Surname + ' ' + SUBSTRING(I.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(I.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(I.EmployeeID, K.PrefixDigits), SPACE(1), '0') + ']'  AS PrincipalInitials, " +
                                "ROW_NUMBER() Over (ORDER BY A.SchID, C.ClassListID, D.CATID) AS SN, D.CATCode " +
                                "FROM ADMSchClassList A " +
                                "INNER JOIN ADMSchlList B ON B.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup C ON C.ClassListID = A.ClassListID " +
                                "INNER JOIN ADMSchClassCategory D ON D.CATID = A.CATID " +
                                "INNER JOIN ADMSchClassDiscipline E ON E.DisciplineID = A.DisciplineID " +
                                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle G ON G.TitleID = F.TitleID " +
                                "LEFT OUTER JOIN SETPrefix H ON H.PrefixID = F.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee I ON I.StaffID = B.StaffID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle J ON J.TitleID = I.TitleID " +                                
                                "LEFT OUTER JOIN SETPrefix K ON K.PrefixID = I.PrefixID " +
                                "WHERE  A.SchID = @SchID AND B.Status = 'True' ORDER BY  A.SchID, C.ClassListID, D.CATID;";
                           
                            classList = (IReadOnlyList<ADMSchClassList>)await connection.QueryAsync<ADMSchClassList>(sql,
                                new
                                {
                                    SchID = _switch.SchID
                                });
                            break;
                        case 2:
                            sql = "SELECT A.ClassID, A.SchInfoID, A.SchID, A.ClassListID, A.DisciplineID, A.StaffID, A.CATID, A.FinalYearClass, A.JuniorFinalYearClass, " +
                                "A.CheckPointClass, A.IGCSEClass, B.School, B.StaffID AS PrincipalID, C.SchClass , C.ConvensionalName, C.UseConvension, D.CATName, " +
                                "E.Discipline, (CASE WHEN C.UseConvension = 'True' THEN C.ConvensionalName ELSE C.SchClass END) + ' ' + D.CATName AS ClassName, " +
                                "CASE WHEN C.UseConvension = 'True' THEN C.ConvensionalName ELSE C.SchClass END AS ClassGroupName, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, G.Title AS ClassTeacherTitle, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') + ' [' + H.PrefixName + REPLACE(STR(F.EmployeeID, H.PrefixDigits), SPACE(1), '0') + ']' AS ClassTeacherWithNo, " +
                                "F.Surname + ' ' + SUBSTRING(F.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(F.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(F.EmployeeID, H.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacherInitials, " +
                                "I.Surname + ' ' + I.FirstName + ' ' + ISNULL(I.MiddleName, ' ') AS Principal, J.Title AS PrincipalTitle, " +
                                "I.Surname + ' ' + I.FirstName + ' ' +  ISNULL(I.MiddleName, ' ')  + ' [' + K.PrefixName + REPLACE(STR(I.EmployeeID, K.PrefixDigits), SPACE(1), '0') + ']' AS PrincipalWithNo, " +
                                "I.Surname + ' ' + SUBSTRING(I.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(I.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(I.EmployeeID, K.PrefixDigits), SPACE(1), '0') + ']'  AS PrincipalInitials, " +
                                "ROW_NUMBER() Over (ORDER BY A.SchID, C.ClassListID, D.CATID) AS SN, D.CATCode " +
                                "FROM ADMSchClassList A " +
                                "INNER JOIN ADMSchlList B ON B.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup C ON C.ClassListID = A.ClassListID " +
                                "INNER JOIN ADMSchClassCategory D ON D.CATID = A.CATID " +
                                "INNER JOIN ADMSchClassDiscipline E ON E.DisciplineID = A.DisciplineID " +
                                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle G ON G.TitleID = F.TitleID " +
                                "LEFT OUTER JOIN SETPrefix H ON H.PrefixID = F.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee I ON I.StaffID = B.StaffID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle J ON J.TitleID = I.TitleID " +
                                "LEFT OUTER JOIN SETPrefix K ON K.PrefixID = I.PrefixID " +
                                "WHERE A.ClassListID = @ClassListID ORDER BY A.SchID, C.ClassListID, D.CATID;";

                            classList = (IReadOnlyList<ADMSchClassList>)await connection.QueryAsync<ADMSchClassList>(sql,
                                new
                                {
                                    ClassListID = _switch.ClassListID
                                });
                            break;
                        default:
                            sql = "SELECT A.ClassID, A.SchInfoID, A.SchID, A.ClassListID, A.DisciplineID, A.StaffID, A.CATID, A.FinalYearClass, A.JuniorFinalYearClass, " +
                                "A.CheckPointClass, A.IGCSEClass, B.School, B.StaffID AS PrincipalID, C.SchClass , C.ConvensionalName, C.UseConvension, D.CATName, " +
                                "E.Discipline, (CASE WHEN C.UseConvension = 'True' THEN C.ConvensionalName ELSE C.SchClass END) + ' ' + D.CATName AS ClassName, " +
                                "CASE WHEN C.UseConvension = 'True' THEN C.ConvensionalName ELSE C.SchClass END AS ClassGroupName, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, G.Title AS ClassTeacherTitle, " +
                                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') + ' [' + H.PrefixName + REPLACE(STR(F.EmployeeID, H.PrefixDigits), SPACE(1), '0') + ']' AS ClassTeacherWithNo, " +
                                "F.Surname + ' ' + SUBSTRING(F.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(F.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(F.EmployeeID, H.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacherInitials, " +
                                "I.Surname + ' ' + I.FirstName + ' ' + ISNULL(I.MiddleName, ' ') AS Principal, J.Title AS PrincipalTitle, " +
                                "I.Surname + ' ' + I.FirstName + ' ' +  ISNULL(I.MiddleName, ' ')  + ' [' + K.PrefixName + REPLACE(STR(I.EmployeeID, K.PrefixDigits), SPACE(1), '0') + ']' AS PrincipalWithNo, " +
                                "I.Surname + ' ' + SUBSTRING(I.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(I.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(I.EmployeeID, K.PrefixDigits), SPACE(1), '0') + ']'  AS PrincipalInitials, " +
                                "ROW_NUMBER() Over (ORDER BY A.SchID, C.ClassListID, D.CATID) AS SN, D.CATCode " +
                                "FROM ADMSchClassList A " +
                                "INNER JOIN ADMSchlList B ON B.SchID = A.SchID " +
                                "INNER JOIN ADMSchClassGroup C ON C.ClassListID = A.ClassListID " +
                                "INNER JOIN ADMSchClassCategory D ON D.CATID = A.CATID " +
                                "INNER JOIN ADMSchClassDiscipline E ON E.DisciplineID = A.DisciplineID " +
                                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle G ON G.TitleID = F.TitleID " +
                                "LEFT OUTER JOIN SETPrefix H ON H.PrefixID = F.PrefixID " +
                                "LEFT OUTER JOIN ADMEmployee I ON I.StaffID = B.StaffID " +
                                "LEFT OUTER JOIN ADMEmployeeTitle J ON J.TitleID = I.TitleID " +
                                "LEFT OUTER JOIN SETPrefix K ON K.PrefixID = I.PrefixID " +
                                "WHERE A.SchID > 1 AND B.Status = 'True' ORDER BY A.SchID, C.ClassListID, D.CATID;";

                            classList = (IReadOnlyList<ADMSchClassList>)await connection.QueryAsync<ADMSchClassList>(sql, new { });
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

            return classList;
        }

        public async Task<ADMSchClassList> GetByIdAsync(int id)
        {
            classDetails = new ADMSchClassList();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.ClassID, A.SchInfoID, A.SchID, A.ClassListID, A.DisciplineID, A.StaffID, A.CATID, A.FinalYearClass, A.JuniorFinalYearClass, " +
                "A.CheckPointClass, A.IGCSEClass, B.School, B.StaffID AS PrincipalID, C.SchClass , C.ConvensionalName, C.UseConvension, D.CATName, " +
                "E.Discipline, (CASE WHEN C.UseConvension = 'True' THEN C.ConvensionalName ELSE C.SchClass END) + ' ' + D.CATName AS ClassName, " +
                "CASE WHEN C.UseConvension = 'True' THEN C.ConvensionalName ELSE C.SchClass END AS ClassGroupName, " +
                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') AS ClassTeacher, G.Title AS ClassTeacherTitle, " +
                "F.Surname + ' ' + F.FirstName + ' ' + ISNULL(F.MiddleName, ' ') + ' [' + H.PrefixName + REPLACE(STR(F.EmployeeID, H.PrefixDigits), SPACE(1), '0') + ']' AS ClassTeacherWithNo, " +
                "F.Surname + ' ' + SUBSTRING(F.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(F.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(F.EmployeeID, H.PrefixDigits), SPACE(1), '0') + ']'  AS ClassTeacherInitials, " +
                "I.Surname + ' ' + I.FirstName + ' ' + ISNULL(I.MiddleName, ' ') AS Principal, J.Title AS PrincipalTitle, " +
                "I.Surname + ' ' + I.FirstName + ' ' +  ISNULL(I.MiddleName, ' ')  + ' [' + K.PrefixName + REPLACE(STR(I.EmployeeID, K.PrefixDigits), SPACE(1), '0') + ']' AS PrincipalWithNo, " +
                "I.Surname + ' ' + SUBSTRING(I.FirstName, 0, 2) + '. ' + ISNULL(SUBSTRING(I.MiddleName, 0, 2) + '. ', '') + ' [' + REPLACE(STR(I.EmployeeID, K.PrefixDigits), SPACE(1), '0') + ']'  AS PrincipalInitials " +
                "FROM ADMSchClassList A " +
                "INNER JOIN ADMSchlList B ON B.SchID = A.SchID " +
                "INNER JOIN ADMSchClassGroup C ON C.ClassListID = A.ClassListID " +
                "INNER JOIN ADMSchClassCategory D ON D.CATID = A.CATID " +
                "INNER JOIN ADMSchClassDiscipline E ON E.DisciplineID = A.DisciplineID " +
                "LEFT OUTER JOIN ADMEmployee F ON F.StaffID = A.StaffID " +
                "LEFT OUTER JOIN ADMEmployeeTitle G ON G.TitleID = F.TitleID " +
                "LEFT OUTER JOIN SETPrefix H ON H.PrefixID = F.PrefixID " +
                "LEFT OUTER JOIN ADMEmployee I ON I.StaffID = B.StaffID " +
                "LEFT OUTER JOIN ADMEmployeeTitle J ON J.TitleID = I.TitleID " +
                "LEFT OUTER JOIN SETPrefix K ON K.PrefixID = I.PrefixID " +
                "WHERE A.ClassID = @ClassID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                classDetails = await connection.QuerySingleOrDefaultAsync<ADMSchClassList>(sql, new { ClassID = id });
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

            return classDetails;
        }

        public async Task<ADMSchClassList> AddAsync(ADMSchClassList entity)
        {
            sql = @"INSERT INTO ADMSchClassList (SchInfoID, SchID, ClassListID, DisciplineID, StaffID, CATID, FinalYearClass, JuniorFinalYearClass, 
                CheckPointClass, IGCSEClass) OUTPUT INSERTED.ClassID VALUES (@SchInfoID, @SchID, @ClassListID, @DisciplineID, @StaffID, @CATID, @FinalYearClass, 
                @JuniorFinalYearClass, @CheckPointClass, @IGCSEClass);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.ClassID = result;
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

        public async Task<ADMSchClassList> UpdateAsync(int id, ADMSchClassList entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMSchClassList SET SchID = @SchID, ClassListID = @ClassListID, DisciplineID = @DisciplineID, " +
                           "StaffID = @StaffID, CATID = @CATID, FinalYearClass = @FinalYearClass, JuniorFinalYearClass = @JuniorFinalYearClass, " +
                           "CheckPointClass = @CheckPointClass, IGCSEClass = @IGCSEClass WHERE ClassID = @classid;";
                    break;
                case 2:
                    sql = "UPDATE ADMSchClassList SET Id = @Id WHERE ClassID = @ClassID;";
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
            sql = "DELETE FROM ADMSchClassList WHERE ClassID = @ClassID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { ClassID = id });
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

        public Task<IReadOnlyList<ADMSchClassList>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ADMSchClassCategoryRepository : IADMSchClassCategoryRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMSchClassCategory> categories { get; set; }
        ADMSchClassCategory category = new();

        public ADMSchClassCategoryRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<ADMSchClassCategory>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT CATID, CATName, CATCode FROM ADMSchClassCategory;";

                            categories = (IReadOnlyList<ADMSchClassCategory>)await connection.QueryAsync<ADMSchClassCategory>(sql, new { });
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

            return categories;
        }

        public async Task<ADMSchClassCategory> GetByIdAsync(int id)
        {
            category = new ADMSchClassCategory();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ADMSchClassCategory WHERE CATID = @CATID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                category = await connection.QuerySingleOrDefaultAsync<ADMSchClassCategory>(sql, new { CATID = id });
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

            return category;
        }

        public async Task<ADMSchClassCategory> AddAsync(ADMSchClassCategory entity)
        {
            sql = @"INSERT INTO ADMSchClassCategory (CATName, CATCode) OUTPUT INSERTED.CATID VALUES (@CATName, @CATCode);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.CATID = result;
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

        public async Task<ADMSchClassCategory> UpdateAsync(int id, ADMSchClassCategory entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMSchClassCategory SET CATName = @CATName, CATCode = @CATCode WHERE CATID = @CATID;";
                    break;
                case 2:
                    sql = "UPDATE ADMSchClassCategory SET Id = @Id WHERE CATID = @CATID;";
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
            sql = "DELETE FROM ADMSchClassCategory WHERE CATID = @CATID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { CATID = id });
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

        public Task<IReadOnlyList<ADMSchClassCategory>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ADMSchClassGroupRepository : IADMSchClassGroupRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMSchClassGroup> classGroupList { get; set; }
        ADMSchClassGroup classGroup = new();

        public ADMSchClassGroupRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
       
        public async Task<IReadOnlyList<ADMSchClassGroup>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT A.ClassListID, CASE WHEN A.UseConvension = 'True' THEN A.ConvensionalName ELSE A.SchClass END AS ClassName, " +
                                "A.SchID, B.School, A.SchClass, A.ConvensionalName, A.UseConvension, " +
                                "CASE WHEN  A.UseConvension = 'True' THEN A.ConvensionalName ELSE A.SchClass END AS ClassGroupName  " +
                                "FROM ADMSchClassGroup A INNER JOIN ADMSchlList B ON A.SchID = B.SchID WHERE A.SchID = @SchID;";

                            classGroupList = (List<ADMSchClassGroup>)await connection.QueryAsync<ADMSchClassGroup>(sql, new { SchID = _switch.SchID });
                            break;
                        default:
                            sql = "SELECT A.ClassListID, CASE WHEN A.UseConvension = 'True' THEN A.ConvensionalName ELSE A.SchClass END AS ClassName, " +
                                "A.SchID, B.School, A.SchClass, A.ConvensionalName, A.UseConvension, " +
                                "CASE WHEN  A.UseConvension = 'True' THEN A.ConvensionalName ELSE A.SchClass END AS ClassGroupName  " +
                                "FROM ADMSchClassGroup A INNER JOIN ADMSchlList B ON A.SchID = B.SchID;";

                            classGroupList = (IReadOnlyList<ADMSchClassGroup>)await connection.QueryAsync<ADMSchClassGroup>(sql, new { });
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

            return classGroupList;
        }

        public async Task<ADMSchClassGroup> GetByIdAsync(int id)
        {
            classGroup = new ADMSchClassGroup();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.ClassListID, CASE WHEN A.UseConvension = 'True' THEN A.ConvensionalName ELSE A.SchClass END AS ClassName, " +
                "A.SchID, B.School, A.SchClass, A.ConvensionalName, A.UseConvension, " +
                "CASE WHEN  A.UseConvension = 'True' THEN A.ConvensionalName ELSE A.SchClass END AS ClassGroupName  " +
                "FROM ADMSchClassGroup A INNER JOIN ADMSchlList B ON A.SchID = B.SchID WHERE A.ClassListID = @ClassListID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                classGroup = await connection.QuerySingleOrDefaultAsync<ADMSchClassGroup>(sql, new { ClassListID = id });
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

            return classGroup;
        }

        public async Task<ADMSchClassGroup> AddAsync(ADMSchClassGroup entity)
        {
            sql = @"INSERT INTO ADMSchClassGroup (SchClass, SchID, ConvensionalName, UseConvension) OUTPUT INSERTED.ClassListID
                    VALUES (@SchClass, @SchID, @ConvensionalName, @UseConvension);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.ClassListID = result;
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

        public async Task<ADMSchClassGroup> UpdateAsync(int id, ADMSchClassGroup entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMSchClassGroup SET SchClass = @SchClass, SchID = @SchID, ConvensionalName = @ConvensionalName, " +
                            "UseConvension = @UseConvension WHERE ClassListID = @classlistid;";
                    break;
                case 2:
                    sql = "UPDATE ADMSchClassGroup SET UseConvension = @UseConvension;";
                    break;
                case 3:
                    sql = "UPDATE ADMSchClassGroup SET Id = @Id WHERE ClassListID = @ClassListID;";
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
            sql = "DELETE FROM ADMSchClassGroup WHERE ClassListID = @ClassListID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { ClassListID = id });
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

        public Task<IReadOnlyList<ADMSchClassGroup>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ADMSchClassDisciplineRepository : IADMSchClassDisciplineRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMSchClassDiscipline> disciplines { get; set; }
        ADMSchClassDiscipline discipline = new();

        public ADMSchClassDisciplineRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
                
        public async Task<IReadOnlyList<ADMSchClassDiscipline>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT DisciplineID, Discipline FROM ADMSchClassDiscipline;";

                            disciplines = (IReadOnlyList<ADMSchClassDiscipline>)await connection.QueryAsync<ADMSchClassDiscipline>(sql, new { });
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

            return disciplines;
        }

        public async Task<ADMSchClassDiscipline> GetByIdAsync(int id)
        {
            discipline = new ADMSchClassDiscipline();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ADMSchClassDiscipline WHERE DisciplineID = @DisciplineID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                discipline = await connection.QuerySingleOrDefaultAsync<ADMSchClassDiscipline>(sql, new { DisciplineID = id });
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

            return discipline;
        }

        public async Task<ADMSchClassDiscipline> AddAsync(ADMSchClassDiscipline entity)
        {
            sql = @"INSERT INTO ADMSchClassDiscipline (Discipline) OUTPUT INSERTED.DisciplineID VALUES (@Discipline);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.DisciplineID = result;
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

        public async Task<ADMSchClassDiscipline> UpdateAsync(int id, ADMSchClassDiscipline entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMSchClassDiscipline SET Discipline = @Discipline WHERE DisciplineID = @DisciplineID;";
                    break;
                case 2:
                    sql = "UPDATE ADMSchClassDiscipline SET Id = @Id WHERE DisciplineID = @DisciplineID;";
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
            sql = "DELETE FROM ADMSchClassDiscipline WHERE DisciplineID = @DisciplineID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { DisciplineID = id });
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

        public Task<IReadOnlyList<ADMSchClassDiscipline>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class ADMSchEducationInstituteRepository : IADMSchEducationInstituteRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<ADMSchEducationInstitute> previousSchools { get; set; }
        ADMSchEducationInstitute previousSchool = new();

        public ADMSchEducationInstituteRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
               
        public async Task<IReadOnlyList<ADMSchEducationInstitute>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {                        
                        default:
                            sql = "SELECT * FROM ADMSchEducationInstitute;";

                            previousSchools = (IReadOnlyList<ADMSchEducationInstitute>)await connection.QueryAsync<ADMSchEducationInstitute>(sql, new { });
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

            return previousSchools;
        }

        public async Task<ADMSchEducationInstitute> GetByIdAsync(int id)
        {
            previousSchool = new ADMSchEducationInstitute();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM ADMSchEducationInstitute WHERE EDUID = @EDUID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                previousSchool = await connection.QuerySingleOrDefaultAsync<ADMSchEducationInstitute>(sql, new { EDUID = id });
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

            return previousSchool;
        }

        public async Task<ADMSchEducationInstitute> AddAsync(ADMSchEducationInstitute entity)
        {
            sql = @"INSERT INTO ADMSchEducationInstitute (EDUInstitute, Address) OUTPUT INSERTED.EDUID VALUES (@EDUInstitute, @Address);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.EDUID = result;
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

        public async Task<ADMSchEducationInstitute> UpdateAsync(int id, ADMSchEducationInstitute entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE ADMSchEducationInstitute SET EDUInstitute = @EDUInstitute, Address = @Address WHERE EDUID = @EDUID;";
                    break;
                case 2:
                    sql = "UPDATE ADMSchEducationInstitute SET Id = @Id WHERE EDUID = @EDUID;";
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
            sql = "DELETE FROM ADMSchEducationInstitute WHERE EDUID = @EDUID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { EDUID = id });
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

        public Task<IReadOnlyList<ADMSchEducationInstitute>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }



}
