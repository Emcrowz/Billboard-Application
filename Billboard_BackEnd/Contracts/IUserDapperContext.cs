using Billboard_BackEnd.Models;

namespace Billboard_BackEnd.Contracts
{
    public interface IUserDapperContext
    {
        // Create
        bool ExecuteCreateUserRecordSQL(User newUser);

        // Read / Get
        User ExecuteFetchUserRecordByIdSQL(int id);
        IEnumerable<User> ExecuteFetchUserRecordsSQL();

        // Update
        bool ExecuteUpdateUserRecordByIdSQL(int id, User userUpdate);
        bool ExecuteUpdateUserPasswordByIdSQL(int id, string passwordToUpdate);

        // Delete
        bool ExecuteDeleteUserRecordByIdSQL(int id);

        // Helpers
        int GetNumberOfUserRecordsInDb();

        // User Specific Actions
        User? ExecuteUserLogin(string username, string password);
    }
}