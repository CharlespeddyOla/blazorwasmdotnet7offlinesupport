using WebAppAcademics.Server.Interfaces.Financials.Banks;
using WebAppAcademics.Shared.Helpers;
using WebAppAcademics.Shared.Models.Financials.Banks;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppAcademics.Server.Repositories
{
    public class FINBankAcctTypeRepository : IFINBankAcctTypeRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<FINBankAcctType> _list { get; set; }
        FINBankAcctType _details = new();

        public FINBankAcctTypeRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<FINBankAcctType>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1:
                            sql = "SELECT * FROM FINBankAcctType;";

                            _list = (List<FINBankAcctType>)await connection.QueryAsync<FINBankAcctType>(sql, new { });
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

        public async Task<FINBankAcctType> GetByIdAsync(int id)
        {
            _details = new FINBankAcctType();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT * FROM FINBankAcctType WHERE BnkAcctTypeID = @BnkAcctTypeID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<FINBankAcctType>(sql, new { BnkAcctTypeID = id });
            }
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

        public async Task<FINBankAcctType> AddAsync(FINBankAcctType entity)
        {
            sql = @"INSERT INTO FINBankAcctType (BankAcctType) OUTPUT INSERTED.BnkAcctTypeID VALUES (@BankAcctType);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.BnkAcctTypeID = result;
                }
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

        public async Task<FINBankAcctType> UpdateAsync(int id, FINBankAcctType entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE FINBankAcctType SET BankAcctType = @BankAcctType WHERE BnkAcctTypeID = @BnkAcctTypeID;";
                    break;
                case 2:
                    sql = "UPDATE FINBankAcctType SET Id = @Id WHERE BnkAcctTypeID = @BnkAcctTypeID;";
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
            sql = "DELETE FROM FINBankAcctType WHERE BnkAcctTypeID = @BnkAcctTypeID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { BnkAcctTypeID = id });
            }
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

        public Task<IReadOnlyList<FINBankAcctType>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }

    public class FINBanksRepository : IFINBanksRepository
    {
        private string sql = string.Empty;
        private readonly IConfiguration configuration;
        IReadOnlyList<FINBankDetails> _list { get; set; }
        FINBankDetails _details = new();

        public FINBanksRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IReadOnlyList<FINBankDetails>> GetAllAsync(SwitchModel _switch)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                try
                {
                    switch (_switch.SwitchID)
                    {
                        case 1: //All Banks
                            sql = "SELECT A.BankID, A.SchInfoID, A.AcctID, A.BankAcctName, A.AcctDescription, A.BnkAcctTypeID, " +
                                "A.BankAcctNumber, A.Branch, A.ChequeStartNo, A.ChequeEndNo, A.ChequeNo, B.BankAcctType " +
                                "FROM FINBankDetails A " +
                                "INNER JOIN FINBankAcctType B ON B.BnkAcctTypeID = A.BnkAcctTypeID;";

                            _list = (List<FINBankDetails>)await connection.QueryAsync<FINBankDetails>(sql, new { });
                            break;
                        case 2: //Bank List By Bank Account Type
                            sql = "SELECT A.BankID, A.SchInfoID, A.AcctID, A.BankAcctName, A.AcctDescription, A.BnkAcctTypeID, " +
                               "A.BankAcctNumber, A.Branch, A.ChequeStartNo, A.ChequeEndNo, A.ChequeNo, B.BankAcctType " +
                               "FROM FINBankDetails A " +
                               "INNER JOIN FINBankAcctType B ON B.BnkAcctTypeID = A.BnkAcctTypeID " +
                               "WHERE A.BnkAcctTypeID = @BnkAcctTypeID;";

                            _list = (List<FINBankDetails>)await connection.QueryAsync<FINBankDetails>(sql, 
                                new 
                                {
                                    BnkAcctTypeID = _switch.BnkAcctTypeID
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

        public async Task<FINBankDetails> GetByIdAsync(int id)
        {
            _details = new FINBankDetails();

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            sql = "SELECT A.BankID, A.SchInfoID, A.AcctID, A.BankAcctName, A.AcctDescription, A.BnkAcctTypeID, " +
                "A.BankAcctNumber, A.Branch, A.ChequeStartNo, A.ChequeEndNo, A.ChequeNo, B.BankAcctType " +
                "FROM FINBankDetails A " +
                "INNER JOIN FINBankAcctType B ON B.BnkAcctTypeID = A.BnkAcctTypeID " +
                "WHERE A.BankID = @BankID;";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            try
            {
                _details = await connection.QueryFirstOrDefaultAsync<FINBankDetails>(sql, new { BankID = id });
            }
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

        public async Task<FINBankDetails> AddAsync(FINBankDetails entity)
        {
            sql = @"INSERT INTO FINBankDetails (SchInfoID, AcctID, BankAcctName, AcctDescription, BnkAcctTypeID, BankAcctNumber, Branch, 
                ChequeStartNo, ChequeEndNo, ChequeNo) OUTPUT INSERTED.BankID VALUES (@SchInfoID, @AcctID, @BankAcctName, @AcctDescription, 
                @BnkAcctTypeID, @BankAcctNumber, @Branch, @ChequeStartNo, @ChequeEndNo, @ChequeNo);";

            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    var result = await connection.ExecuteScalarAsync<int>(sql, entity);
                    entity.BankID = result;
                }
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

        public async Task<FINBankDetails> UpdateAsync(int id, FINBankDetails entity)
        {
            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            switch (id)
            {
                case 1:
                    sql = "UPDATE FINBankDetails SET AcctID = @AcctID, BankAcctName = @BankAcctName, AcctDescription = @AcctDescription, " +
                        "BnkAcctTypeID = @BnkAcctTypeID, BankAcctNumber = @BankAcctNumber, Branch = @Branch, ChequeStartNo = @ChequeStartNo, " +
                        "ChequeEndNo = @ChequeEndNo, ChequeNo = @ChequeNo WHERE BankID = @BankID;";
                    break;
                case 2:
                    sql = "UPDATE FINBankDetails SET Id = @Id WHERE BankID = @BankID;";
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
            sql = "DELETE FROM FINBankDetails WHERE BankID = @BankID";

            using var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            try
            {
                await connection.ExecuteAsync(sql, new { BankID = id });
            }
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

        public Task<IReadOnlyList<FINBankDetails>> SearchAsync(SwitchModel _switch)
        {
            throw new NotImplementedException();
        }
    }
}
