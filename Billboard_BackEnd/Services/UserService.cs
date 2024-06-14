using Billboard_BackEnd.Contracts;
using Billboard_BackEnd.Models;
using Billboard_BackEnd.Repositories;

namespace Billboard_BackEnd.Services
{
    public class UserService : IUserService
    {
        #region SETUP / INITIALISATION
        private readonly IUserDapperContext _dbRepoDapper;
        //private readonly IUserMongoDBContext _dbRepoMongo;

        public UserService(string connectionString)
        {
            _dbRepoDapper = new UserRepository(connectionString);
        }
        #endregion

        #region SERVICES RELATED TO LOCAL ACTIONS
        public bool CreateNewUser(User newUser)
        {
            if (_dbRepoDapper.ExecuteCreateUserRecordSQL(newUser))
                return true;
            else
                return false;
        }

        public IEnumerable<User> GetAllUsers() => _dbRepoDapper.ExecuteFetchUserRecordsSQL();

        public User? GetUserById(int id)
        {
            int recordCount = _dbRepoDapper.GetNumberOfUserRecordsInDb();
            if (id >= 0 && id <= recordCount)
            {
                return _dbRepoDapper.ExecuteFetchUserRecordByIdSQL(id);
            }
            else
                return null;
        }

        public bool UpdateUserDetailsById(int id, User userUpdate)
        {
            int recordCount = _dbRepoDapper.GetNumberOfUserRecordsInDb();
            if (id >= 0 && id <= recordCount)
            {
                return _dbRepoDapper.ExecuteUpdateUserRecordByIdSQL(id, userUpdate);
            }
            else
                return false;
        }

        public bool UpdateUserPasswordById(int id, string passwordUpdate)
        {
            int recordCount = _dbRepoDapper.GetNumberOfUserRecordsInDb();         
            if (id >= 0 && id <= recordCount)
            {
                return _dbRepoDapper.ExecuteUpdateUserPasswordByIdSQL(id, passwordUpdate);
            }
            else
                return false;
        }
        
        public bool DeleteUser(int id)
        {
            int recordCount = _dbRepoDapper.GetNumberOfUserRecordsInDb();
            if (id >= 0 && id <= recordCount)
            {
                return _dbRepoDapper.ExecuteDeleteUserRecordByIdSQL(id);
            }
            else
                return false;
        }
        #endregion

        #region USER SPECIFIC SERVICES
        public User? UserLoginService(string username, string password)
        {
            User? user = _dbRepoDapper.ExecuteUserLogin(username, password);
            if (user != null)
            {
                return user;
            }
            else 
                return null;
        }
        #endregion
    }
}
