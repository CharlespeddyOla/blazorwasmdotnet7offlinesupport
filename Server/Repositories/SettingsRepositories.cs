using WebAppAcademics.Server.Interfaces.Settings;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Settings;
using System.Data;
using System.Data.SqlClient;

namespace WebAppAcademics.Server.Repositories
{
    public class SETSchSessionsRepository : ISETSchSessionsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<SETSchSessions> academicsessions { get; set; }
        SETSchSessions academicsession = new();

        public SETSchSessionsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<SETSchSessions>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT A.TermID, A.SchSession, A.SchTerm, A.StartDate, A.EndDate, A.Status, A.CalendarID, A.Attendance, " +
                                "B.StartMonthID, B.StartMonth, B.EndMonthID, B.EndMonth, " +
                                "LEFT(A.SchSession, 4) + '/' + RIGHT(A.SchSession, 4) AS AcademicYear, " +
                                "LEFT(A.SchSession, 4) + '/' + RIGHT(A.SchSession, 4) + ' - ' + A.SchTerm  + ' Term' AS AcademicSession " +
                                "FROM SETSchSessions A INNER JOIN SETSchCalendar B ON A.CalendarID = B.CalendarID;";

                            academicsessions = (List<SETSchSessions>)await connection.QueryAsync<SETSchSessions>(sql, new { });
                            break;
                        case 2:
                            sql = "SELECT DISTINCT SchSession, SUBSTRING(CAST(SchSession AS varchar(14)), 0, 5) + '/' + " +
                                "SUBSTRING(CAST(SchSession AS varchar(14)), 5, 6) AS AcademicYear FROM SETSchSessions;";
                            academicsessions = (List<SETSchSessions>)await connection.QueryAsync<SETSchSessions>(sql, new { });
                            break;
                        case 3:
                            sql = "SELECT A.TermID, A.SchSession, A.SchTerm, A.StartDate, A.EndDate, A.Status, A.CalendarID, A.Attendance, " +
                                "B.StartMonthID, B.StartMonth, B.EndMonthID, B.EndMonth, " +
                                "LEFT(A.SchSession, 4) + '/' + RIGHT(A.SchSession, 4) AS AcademicYear, " +
                                "LEFT(A.SchSession, 4) + '/' + RIGHT(A.SchSession, 4) + ' - ' + A.SchTerm  + ' Term' AS AcademicSession " +
                                "FROM SETSchSessions A INNER JOIN SETSchCalendar B ON A.CalendarID = B.CalendarID " +
                                "WHERE SchSession = @SchSession;";

                            academicsessions = (List<SETSchSessions>)await connection.QueryAsync<SETSchSessions>(sql, 
                                new 
                                { 
                                    SchSession = _switch.SchSession
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

            return academicsessions;
        }

        public async Task<SETSchSessions> GetByIdAsync(int id)
        {
            academicsession = new SETSchSessions();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.TermID, A.SchSession, A.SchTerm, A.StartDate, A.EndDate, A.Status, A.CalendarID, A.Attendance, " +
                "B.StartMonthID, B.StartMonth, B.EndMonthID, B.EndMonth, " +
                "LEFT(A.SchSession, 4) + '/' + RIGHT(A.SchSession, 4) AS AcademicYear, " +
                "LEFT(A.SchSession, 4) + '/' + RIGHT(A.SchSession, 4) + ' - ' + A.SchTerm  + ' Term' AS AcademicSession " +
                "FROM SETSchSessions A INNER JOIN SETSchCalendar B ON A.CalendarID = B.CalendarID WHERE A.TermID = @TermID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                academicsession = await connection.QueryFirstOrDefaultAsync<SETSchSessions>(sql, new { TermID = id });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return academicsession;
        }

        public async Task<SETSchSessions> AddAsync(SETSchSessions entity)
        {
            sql = @"INSERT INTO SETSchSessions (SchSession, SchTerm, StartDate, EndDate, Status, CalendarID, Attendance) OUTPUT INSERTED.TermID 
                VALUES (@SchSession, @SchTerm, @StartDate, @EndDate, @Status, @CalendarID, @Attendance);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.TermID = result;
                }
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

        public async Task<SETSchSessions> UpdateAsync(int id, SETSchSessions entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE SETSchSessions SET StartDate = @StartDate, EndDate = @EndDate, Attendance = @Attendance, CalendarID = @CalendarID " +
                        "WHERE TermID = @TermID;";
                    break;
                case 2:
                    sql = "UPDATE SETSchSessions SET Id = @Id WHERE TermID = @TermID;";
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
            sql = "DELETE FROM SETSchSessions WHERE TermID = @TermID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { TermID = id });
            }
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

        public Task<IReadOnlyList<SETSchSessions>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class SETSchCalendarRepository : ISETSchCalendarRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<SETSchCalendar> calendarlist { get; set; }
        SETSchCalendar calendar = new();

        public SETSchCalendarRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<SETSchCalendar>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM SETSchCalendar;";

                            calendarlist = (List<SETSchCalendar>)await connection.QueryAsync<SETSchCalendar>(sql, new { });
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

            return calendarlist;
        }

        public async Task<SETSchCalendar> GetByIdAsync(int id)
        {
            calendar = new SETSchCalendar();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM SETSchCalendar WHERE CalendarID = @CalendarID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                calendar = await connection.QueryFirstOrDefaultAsync<SETSchCalendar>(sql, new { CalendarID = id });
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return calendar;
        }

        public async Task<SETSchCalendar> AddAsync(SETSchCalendar entity)
        {
            sql = @"INSERT INTO SETSchCalendar (SchTerm, StartMonthID, StartMonth, EndMonthID, EndMonth) OUTPUT INSERTED.CalendarID 
                    VALUES (@SchTerm, @StartMonthID, @StartMonth, @EndMonthID, @EndMonth);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.CalendarID = result;
                }
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
          
        public async Task<SETSchCalendar> UpdateAsync(int id, SETSchCalendar entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE SETSchCalendar SET StartMonthID = @StartMonthID, StartMonth = @StartMonth, EndMonthID = @EndMonthID, " +
                            "EndMonth = @EndMonth WHERE CalendarID = @CalendarID;";
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
            sql = "DELETE FROM SETSchCalendar WHERE CalendarID = @CalendarID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { CalendarID = id });
            }
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

        public Task<IReadOnlyList<SETSchCalendar>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class SETSchInformationRepository : ISETSchInformationRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<SETSchInformation> _list { get; set; }
        SETSchInformation _details = new();

        public SETSchInformationRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<SETSchInformation>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT A.SchInfoID, A.SchName, A.SchSlogan, A.SchCode, A.SchType, A.SchAddress, A.SchAddressLine2, A.SchPhones, " +
                                "A.SchPhones2, A.SchEmails, A.SchWebsites, A.CountryID, A.StateID, A.LGAID, A.SchSplashScreen, A.SchLogo, A.StatusTypeID, " +
                                "A.DefaultSch, A.EmailSettings, A.SchEmailPW, A.EmailPortSMTP, A.EmailPortSSLTLS, B.Country, C.State, D.LGA, E.StatusType, " +
                                "A.EmailPassword " +
                                "FROM SETSchInformation A " +
                                "INNER JOIN SETCountries B ON B.CountryID = A.CountryID " +
                                "INNER JOIN SETStates C ON C.StateID = A.StateID " +
                                "INNER JOIN SETLGA D ON D.LGAID = A.LGAID " +
                                "INNER JOIN SETStatusType E ON E.StatusTypeID = A.StatusTypeID;";

                            _list = (List<SETSchInformation>)await connection.QueryAsync<SETSchInformation>(sql, new { });
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

        public async Task<SETSchInformation> GetByIdAsync(int id)
        {
            _details = new SETSchInformation();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.SchInfoID, A.SchName, A.SchSlogan, A.SchCode, A.SchType, A.SchAddress, A.SchAddressLine2, A.SchPhones, " +
                "A.SchPhones2, A.SchEmails, A.SchWebsites, A.CountryID, A.StateID, A.LGAID, A.SchSplashScreen, A.SchLogo, A.StatusTypeID, " +
                "A.DefaultSch, A.EmailSettings, A.SchEmailPW, A.EmailPortSMTP, A.EmailPortSSLTLS, B.Country, C.State, D.LGA, E.StatusType, " +
                "A.EmailPassword " +
                "FROM SETSchInformation A " +
                "INNER JOIN SETCountries B ON B.CountryID = A.CountryID " +
                "INNER JOIN SETStates C ON C.StateID = A.StateID " +
                "INNER JOIN SETLGA D ON D.LGAID = A.LGAID " +
                "INNER JOIN SETStatusType E ON E.StatusTypeID = A.StatusTypeID " +
                "WHERE A.SchInfoID = @SchInfoID; ";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<SETSchInformation>(sql, new { SchInfoID = id });
            }
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

        public async Task<SETSchInformation> AddAsync(SETSchInformation entity)
        {
            sql = @"INSERT INTO SETSchInformation (SchName, SchSlogan, SchCode, SchType, SchAddress, SchAddressLine2, SchPhones, SchPhones2, SchEmails, 
                    SchWebsites, CountryID, StateID, LGAID, SchSplashScreen, SchLogo, StatusTypeID, DefaultSch, SchEmailPW, EmailSettings, EmailPortSMTP, 
                    EmailPortSSLTLS, EmailPassword) OUTPUT INSERTED.SchInfoID VALUES (@SchName, @SchSlogan, @SchCode, @SchType, @SchAddress, @SchAddressLine2, @SchPhones, 
                    @SchPhones2, @SchEmails, @SchWebsites, @CountryID, @StateID, @LGAID, @SchSplashScreen, @SchLogo, @StatusTypeID, @DefaultSch, 
                    @SchEmailPW, @EmailSettings, @EmailPortSMTP, @EmailPortSSLTLS, @EmailPassword);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.SchInfoID = result;
                }
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

        public async Task<SETSchInformation> UpdateAsync(int id, SETSchInformation entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE SETSchInformation SET SchName = @SchName, SchSlogan = @SchSlogan, SchCode = @SchCode, SchType = @SchType, " +
                        "SchAddress = @SchAddress, SchAddressLine2 = @SchAddressLine2, SchPhones = @SchPhones, SchPhones2 = @SchPhones2, " +
                        " SchEmails = @SchEmails, SchWebsites = @SchWebsites, CountryID = @CountryID, StateID = @StateID, LGAID = @LGAID, " +
                        "SchSplashScreen = @SchSplashScreen, SchLogo = @SchLogo, StatusTypeID = @StatusTypeID, SchEmailPW = @SchEmailPW, " +
                        "EmailSettings = @EmailSettings, EmailPortSMTP = @EmailPortSMTP, EmailPortSSLTLS = @EmailPortSSLTLS, " +
                        "EmailPassword = @EmailPassword WHERE SchInfoID = @SchInfoID;";
                    break;
                case 2: //Set Default School
                    sql = "UPDATE SETSchInformation SET DefaultSch = @DefaultSch WHERE SchInfoID = @SchInfoID;";
                    break;
                case 3:
                    sql = "UPDATE SETSchInformation SET Id = @Id WHERE SchInfoID = @SchInfoID;";
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
            sql = "DELETE FROM SETSchInformation WHERE SchInfoID = @SchInfoID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { SchInfoID = id });
            }
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

        public Task<IReadOnlyList<SETSchInformation>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class SETCountriesRepository : ISETCountriesRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<SETCountries> _list { get; set; }
        SETCountries _details = new();

        public SETCountriesRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<SETCountries>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM SETCountries ORDER BY Country;";

                            _list = (List<SETCountries>)await connection.QueryAsync<SETCountries>(sql, new { });
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

        public async Task<SETCountries> GetByIdAsync(int id)
        {
            _details = new SETCountries();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM SETCountries WHERE CountryID = @CountryID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<SETCountries>(sql, new { CountryID = id });
            }
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

        public async Task<SETCountries> AddAsync(SETCountries entity)
        {
            sql = @"INSERT INTO SETCountries (CountryCode, Country) OUTPUT INSERTED.CountryID VALUES (@CountryCode, @Country);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.CountryID = result;
                }
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

        public async Task<SETCountries> UpdateAsync(int id, SETCountries entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE SETCountries SET CountryCode = @CountryCode, Country = @Country WHERE CountryID = @CountryID;";
                    break;
                case 2:
                    sql = "UPDATE SETCountries SET Id = @Id WHERE CountryID = @CountryID;";
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
            sql = "DELETE FROM SETCountries WHERE CountryID = @CountryID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { CountryID = id });
            }
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

        public Task<IReadOnlyList<SETCountries>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class SETStatesRepository : ISETStatesRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<SETStates> _list { get; set; }
        SETStates _details = new();

        public SETStatesRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<SETStates>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT StateID, StateCode, State, A.CountryID, B.Country " +
                                "FROM SETStates A " +
                                "INNER JOIN SETCountries B ON B.CountryID = A.CountryID " +
                                "WHERE A.CountryID = @CountryID ORDER BY State;";

                            _list = (List<SETStates>)await connection.QueryAsync<SETStates>(sql, 
                                new 
                                { 
                                    CountryID = _switch.CountryID
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

        public async Task<SETStates> GetByIdAsync(int id)
        {
            _details = new SETStates();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM SETStates WHERE StateID = @StateID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<SETStates>(sql, new { StateID = id });
            }
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

        public async Task<SETStates> AddAsync(SETStates entity)
        {
            sql = @"INSERT INTO SETStates (StateCode, State, CountryID) OUTPUT INSERTED.StateID VALUES (@StateCode, @State, @CountryID);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.StateID = result;
                }
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

        public async Task<SETStates> UpdateAsync(int id, SETStates entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE SETStates SET StateCode = @StateCode, State = @State, CountryID = @CountryID WHERE StateID = @StateID;";
                    break;
                case 2:
                    sql = "UPDATE SETStates SET Id = @Id WHERE StateID = @StateID;";
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
            sql = "DELETE FROM SETStates WHERE StateID = @StateID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { StateID = id });
            }
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

        public Task<IReadOnlyList<SETStates>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class SETLGARepository : ISETLGARepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<SETLGA> _list { get; set; }
        SETLGA _details = new();

        public SETLGARepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<SETLGA>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT LGAID, LGACode, LGA, A.StateID, B.State FROM SETLGA A INNER JOIN SETStates B ON A.StateID = B.StateID " +
                                    "WHERE A.StateID = @StateID ORDER BY LGA;";

                            _list = (List<SETLGA>)await connection.QueryAsync<SETLGA>(sql,
                                new
                                {
                                    StateID = _switch.StateID
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

        public async Task<SETLGA> GetByIdAsync(int id)
        {
            _details = new SETLGA();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM SETLGA WHERE LGAID = @LGAID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<SETLGA>(sql, new { LGAID = id });
            }
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

        public async Task<SETLGA> AddAsync(SETLGA entity)
        {
            sql = @"INSERT INTO SETLGA (LGACode, LGA, StateID) OUTPUT INSERTED.LGAID VALUES (@LGACode, @LGA, @StateID);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.LGAID = result;
                }
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

        public async Task<SETLGA> UpdateAsync(int id, SETLGA entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE SETLGA SET LGACode = @LGACode, LGA = @LGA, StateID = @StateID WHERE LGAID = @LGAID;";
                    break;
                case 2:
                    sql = "UPDATE SETLGA SET Id = @Id WHERE LGAID = @LGAID;";
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
            sql = "DELETE FROM SETLGA WHERE LGAID = @LGAID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { LGAID = id });
            }
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

        public Task<IReadOnlyList<SETLGA>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class SETStatusTypeRepository : ISETStatusTypeRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<SETStatusType> _list { get; set; }
        SETStatusType _details = new();

        public SETStatusTypeRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<SETStatusType>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM SETStatusType WHERE StatusTypeID < 3;";

                            _list = (List<SETStatusType>)await connection.QueryAsync<SETStatusType>(sql,
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

        public async Task<SETStatusType> GetByIdAsync(int id)
        {
            _details = new SETStatusType();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM SETStatusType WHERE StatusTypeID = @StatusTypeID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<SETStatusType>(sql, new { StatusTypeID = id });
            }
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

        public async Task<SETStatusType> AddAsync(SETStatusType entity)
        {
            sql = @"INSERT INTO SETStatusType (StatusType) OUTPUT INSERTED.StatusTypeID VALUES (@StatusType);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.StatusTypeID = result;
                }
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

        public async Task<SETStatusType> UpdateAsync(int id, SETStatusType entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE SETStatusType SET StatusType = @StatusType WHERE StatusTypeID = @StatusTypeID;";
                    break;
                case 2:
                    sql = "UPDATE SETStatusType SET Id = @Id WHERE StatusTypeID = @StatusTypeID;";
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
            sql = "DELETE FROM SETStatusType WHERE StatusTypeID = @StatusTypeID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { StatusTypeID = id });
            }
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

        public Task<IReadOnlyList<SETStatusType>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class SETPayeeTypeRepository : ISETPayeeTypeRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<SETPayeeType> _list { get; set; }
        SETPayeeType _details = new();

        public SETPayeeTypeRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<SETPayeeType>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: //Parent List for Student Module
                            sql = "SELECT * FROM SETPayeeType WHERE PayeeTypeID BETWEEN 2 AND 3;";
                            _list = (List<SETPayeeType>)await connection.QueryAsync<SETPayeeType>(sql, new { });
                            break;
                        case 2: //For Financial Module
                            sql = "SELECT * FROM SETPayeeType;";
                            _list = (List<SETPayeeType>)await connection.QueryAsync<SETPayeeType>(sql, new { });
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

        public async Task<SETPayeeType> GetByIdAsync(int id)
        {
            _details = new SETPayeeType();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM SETPayeeType WHERE PayeeTypeID = @PayeeTypeID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<SETPayeeType>(sql, new { PayeeTypeID = id });
            }
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

        public async Task<SETPayeeType> AddAsync(SETPayeeType entity)
        {
            sql = @"INSERT INTO SETPayeeType (PayeeType) OUTPUT INSERTED.PayeeTypeID VALUES (@PayeeType);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.PayeeTypeID = result;
                }
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

        public async Task<SETPayeeType> UpdateAsync(int id, SETPayeeType entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE SETPayeeType SET PayeeType = @PayeeType WHERE PayeeTypeID = @PayeeTypeID;";
                    break;
                case 2:
                    sql = "UPDATE SETPayeeType SET Id = @Id WHERE PayeeTypeID = @PayeeTypeID;";
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
            sql = "DELETE FROM SETPayeeType WHERE PayeeTypeID = @PayeeTypeID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { PayeeTypeID = id });
            }
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

        public Task<IReadOnlyList<SETPayeeType>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class SETGenderRepository : ISETGenderRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<SETGender> _list { get; set; }
        SETGender _details = new();

        public SETGenderRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<SETGender>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: 
                            sql = "SELECT * FROM SETGender WHERE GenderID < 3;";

                            _list = (List<SETGender>)await connection.QueryAsync<SETGender>(sql, new { });
                            break;
                        case 2: //All
                            sql = "SELECT * FROM SETGender;";

                            _list = (List<SETGender>)await connection.QueryAsync<SETGender>(sql, new { });
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

        public async Task<SETGender> GetByIdAsync(int id)
        {
            _details = new SETGender();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM SETGender WHERE GenderID = @GenderID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<SETGender>(sql, new { GenderID = id });
            }
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

        public async Task<SETGender> AddAsync(SETGender entity)
        {
            sql = @"INSERT INTO SETGender (Gender, GenderAbrv) OUTPUT INSERTED.GenderID VALUES (@Gender, @GenderAbrv);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.GenderID = result;
                }
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

        public async Task<SETGender> UpdateAsync(int id, SETGender entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE SETGender SET Gender = @Gender, GenderAbrv = @GenderAbrv WHERE GenderID = @GenderID;";
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
            sql = "DELETE FROM SETGender WHERE GenderID = @GenderID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { GenderID = id });
            }
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

        public Task<IReadOnlyList<SETGender>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class SETMedicalRepository : ISETMedicalRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<SETMedical> _list { get; set; }
        SETMedical _details = new();

        public SETMedicalRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<SETMedical>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM SETMedical;";

                            _list = (List<SETMedical>)await connection.QueryAsync<SETMedical>(sql, new { });
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

        public async Task<SETMedical> GetByIdAsync(int id)
        {
            _details = new SETMedical();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM SETMedical WHERE MEDID = @MEDID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<SETMedical>(sql, new { MEDID = id });
            }
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

        public async Task<SETMedical> AddAsync(SETMedical entity)
        {
            sql = @"INSERT INTO SETMedical (MEDValue, MEDName) OUTPUT INSERTED.MEDID VALUES (@MEDValue, @MEDName);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.MEDID = result;
                }
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

        public async Task<SETMedical> UpdateAsync(int id, SETMedical entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE SETMedical SET MEDValue = @MEDValue, MEDName = @MEDName WHERE MEDID = @MEDID;";
                    break;
                case 2:
                    sql = "UPDATE SETMedical SET Id = @Id WHERE MEDID = @MEDID;";
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
            sql = "DELETE FROM SETMedical WHERE MEDID = @MEDID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { MEDID = id });
            }
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

        public Task<IReadOnlyList<SETMedical>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class SETReligionRepository : ISETReligionRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<SETReligion> _list { get; set; }
        SETReligion _details = new();

        public SETReligionRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<SETReligion>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM SETReligion;";

                            _list = (List<SETReligion>)await connection.QueryAsync<SETReligion>(sql, new { });
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

        public async Task<SETReligion> GetByIdAsync(int id)
        {
            _details = new SETReligion();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM SETReligion WHERE ReligionID = @ReligionID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<SETReligion>(sql, new { ReligionID = id });
            }
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

        public async Task<SETReligion> AddAsync(SETReligion entity)
        {
            sql = @"INSERT INTO SETReligion (Religion) OUTPUT INSERTED.ReligionID VALUES (@Religion);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.ReligionID = result;
                }
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

        public async Task<SETReligion> UpdateAsync(int id, SETReligion entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE SETReligion SET Religion = @Religion WHERE ReligionID = @ReligionID;";
                    break;
                case 2:
                    sql = "UPDATE SETReligion SET Id = @Id WHERE ReligionID = @ReligionID;";
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
            sql = "DELETE FROM SETReligion WHERE ReligionID = @ReligionID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { ReligionID = id });
            }
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

        public Task<IReadOnlyList<SETReligion>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class SETMonthListRepository : ISETMonthListRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<SETMonthList> _list { get; set; }
        SETMonthList _details = new();

        public SETMonthListRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<SETMonthList>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT  MonthID AS StartMonthID, Month AS StartMonth, MonthID AS EndMonthID, Month AS EndMonth FROM SETMonthList;";

                            _list = (List<SETMonthList>)await connection.QueryAsync<SETMonthList>(sql, new { });
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

        public async Task<SETMonthList> GetByIdAsync(int id)
        {
            _details = new SETMonthList();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT Month, MonthCode, NoofDays, NoOfWeeks, PRLStatus FROM SETMonthList WHERE MonthID = @MonthID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<SETMonthList>(sql, new { MonthID = id });
            }
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

        public Task<SETMonthList> AddAsync(SETMonthList entity)
        {
            throw new NotImplementedException();
        }

        public async Task<SETMonthList> UpdateAsync(int id, SETMonthList entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE SETMonthList SET MonthCode = @MonthCode, NoofDays = @NoofDays, NoOfWeeks = @NoOfWeeks " +
                        "WHERE MonthID = @MonthID;";
                    break;
                case 2:
                    sql = "UPDATE SETMonthList SET PRLStatus = @PRLStatus WHERE MonthID = @MonthID;";
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

        public Task<int> CountAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<SETMonthList>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class SETRoleRepository : ISETRoleRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<SETRole> _list { get; set; }
        SETRole _details = new();

        public SETRoleRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<SETRole>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM SETRole WHERE RoleID > 1;";

                            _list = (List<SETRole>)await connection.QueryAsync<SETRole>(sql, new { });
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

        public async Task<SETRole> GetByIdAsync(int id)
        {
            _details = new SETRole();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM SETRole WHERE RoleID = @RoleID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<SETRole>(sql, new { RoleID = id });
            }
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

        public async Task<SETRole> AddAsync(SETRole entity)
        {
            sql = @"INSERT INTO SETRole (RoleDesc) OUTPUT INSERTED.RoleID VALUES (@RoleDesc);";

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

        public async Task<SETRole> UpdateAsync(int id, SETRole entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE SETRole SET RoleDesc = @RoleDesc WHERE RoleID = @RoleID;";
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
            sql = "DELETE FROM SETRole WHERE RoleID = @RoleID";

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

        public Task<int> CountAsync(SwitchModel _switchd)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<SETRole>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class SETReportsRepository : ISETReportsRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<SETReports> _list { get; set; }
        SETReports _details = new();

        public SETReportsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<SETReports>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM SETReports WHERE ReportClass = 'Academics';";

                            _list = (List<SETReports>)await connection.QueryAsync<SETReports>(sql, new { });
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

        public async Task<SETReports> GetByIdAsync(int id)
        {
            _details = new SETReports();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM SETReports WHERE ReportID = @ReportID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<SETReports>(sql, new { ReportID = id });
            }
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

        public async Task<SETReports> AddAsync(SETReports entity)
        {
            entity.Delete = false;
            sql = @"INSERT INTO SETReports (ReportCode, ReportFileName, ReportDescr, ReportClass, delete) OUTPUT INSERTED.ReportID 
                    VALUES (@ReportCode, @ReportFileName, @ReportDescr, @ReportClass, @delete);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.ReportID = result;
                }
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

        public async Task<SETReports> UpdateAsync(int id, SETReports entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE SETReports SET ReportFileName = @ReportFileName, ReportDescr = @ReportDescr " +
                            "WHERE ReportID = @ReportID;";
                    break;
                case 2:
                    sql = "UPDATE SETReports SET Id = @Id WHERE ReportID = @ReportID;";
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
            sql = "DELETE FROM SETReports WHERE ReportID = @ReportID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { ReportID = id });
            }
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

        public Task<IReadOnlyList<SETReports>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class SETAppLicenseRepository : ISETAppLicenseRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<SETAppLicense> _list { get; set; }
        SETAppLicense _details = new();

        public SETAppLicenseRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<SETAppLicense>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        default:
                            sql = "SELECT * FROM SETAppLicense;";

                            _list = (List<SETAppLicense>)await connection.QueryAsync<SETAppLicense>(sql, new { });
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

        public async Task<SETAppLicense> GetByIdAsync(int id)
        {
            _details = new SETAppLicense();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM SETAppLicense WHERE UserID = @UserID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<SETAppLicense>(sql, new { UserID = id });
            }
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

        public async Task<SETAppLicense> AddAsync(SETAppLicense entity)
        {
            sql = @"INSERT INTO SETAppLicense (UserID, LicenseStatus, ExpiredStatus, PerpetualStatus, LicenseDate, LicenseDuration, License) 
                    OUTPUT INSERTED.UserID 
                    VALUES (@UserID, @LicenseStatus, @ExpiredStatus, @PerpetualStatus, @LicenseDate, @LicenseDuration, @License);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.UserID = result;
                }
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

        public async Task<SETAppLicense> UpdateAsync(int id, SETAppLicense entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1: //Update License Status as Expired (True) Or Not Expired (False)
                    sql = "UPDATE SETAppLicense SET LicenseStatus = @LicenseStatus, ExpiredStatus = @ExpiredStatus " +
                        "WHERE UserID = @UserID;";
                    break;
                case 2: //Update License Date And License Duration - Renewal
                    sql = "UPDATE SETAppLicense SET LicenseStatus = @LicenseStatus, PerpetualStatus = @PerpetualStatus, LicenseDate = @LicenseDate, " +
                        "LicenseDuration = @LicenseDuration WHERE UserID = @UserID;";
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

        public async Task<int> CountAsync(SwitchModel _switchd)
        {
            int recordCount = 0;

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                switch (_switchd.SwitchID)
                {
                    default:
                        sql = "SELECT COUNT(*) FROM SETAppLicense;";
                        var resultSingle = await connection.ExecuteScalarAsync<int>(sql, new { });
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

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<SETAppLicense>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }
}
