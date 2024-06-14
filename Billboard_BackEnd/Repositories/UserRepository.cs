using Billboard_BackEnd.Contracts;
using Billboard_BackEnd.Models;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Billboard_BackEnd.Repositories
{
    public class UserRepository : IUserDapperContext, IUserMongoContext
    {
        #region SETUP / INITIALISATION
        private IDbConnection _dbConnectionLocal;

        public UserRepository(string localDbConnectionString)
        {
            _dbConnectionLocal = new SqlConnection(localDbConnectionString);
        }
        #endregion

        #region DAPPER CRUD
        // Create
        public bool ExecuteCreateUserRecordSQL(User newUser)
        {
            _dbConnectionLocal.Execute($"INSERT INTO Users ( [FirstName], [LastName], [Username], [Password], [UserCategory] ) VALUES ( '{newUser.FirstName}', '{newUser.LastName}', '{newUser.Username}', '{newUser.Password}', '{newUser.UserCategory.ToString()}' )");
            
            int additionIndex = _dbConnectionLocal.QuerySingle<int>($"SELECT MAX(UserId) FROM Users"); // Left just in case.
            newUser.UserId = additionIndex;
            return true;
        }

        public User ExecuteFetchUserRecordByIdSQL(int id)
        {
            return _dbConnectionLocal.QuerySingle<User>($"SELECT [UserId], [FirstName], [LastName], [Username], [Password], [UserCategory], [VehicleListingIds] FROM Users WHERE [UserId] = {id}");
        }

        public IEnumerable<User> ExecuteFetchUserRecordsSQL()
        {
            return _dbConnectionLocal.Query<User>($"SELECT [UserId], [FirstName], [LastName], [Username], [Password], [UserCategory], [VehicleListingIds] FROM Users");
        }
        
        public bool ExecuteUpdateUserRecordByIdSQL(int id, User userUpdate)
        {
            return _dbConnectionLocal.Execute($"UPDATE Users SET [FirstName] = '{userUpdate.FirstName}', [LastName] = '{userUpdate.LastName}', [Username] = '{userUpdate.Username}' WHERE [UserId] = {id}") > 0;
        }

        public bool ExecuteUpdateUserPasswordByIdSQL(int id, string passwordToUpdate)
        {
            return _dbConnectionLocal.Execute($"UPDATE Users SET [Password] = '{passwordToUpdate}' WHERE [UserId] = {id}") > 0;
        }

        public bool ExecuteDeleteUserRecordByIdSQL(int id)
        {
            return _dbConnectionLocal.Execute($"DELETE FROM Users WHERE [UserId] = {id}") > 0;
        }
        #endregion

        #region HELPER OPERATIONS
        public int GetNumberOfUserRecordsInDb()
        {
            return _dbConnectionLocal.QuerySingle<int>("SELECT COUNT(UserId) FROM Users");
        }
        #endregion

        #region USER SPECIFIC ACTIONS
        public User? ExecuteUserLogin(string username, string password)
        {
            return _dbConnectionLocal.QuerySingle<User?>($"SELECT [Username], [Password] FROM Users WHERE [Username] = '{username}' AND [Password] = '{password}'");
        }
        #endregion
    }
}
