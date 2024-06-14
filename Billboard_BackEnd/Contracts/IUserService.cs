using Billboard_BackEnd.Models;

namespace Billboard_BackEnd.Contracts
{
    public interface IUserService
    {
        // Create 
        bool CreateNewUser(User newUser);

        // Read / Get
        IEnumerable<User> GetAllUsers();
        User? GetUserById(int id);

        // Update
        bool UpdateUserDetailsById(int id, User userUpdate);
        bool UpdateUserPasswordById(int id, string passwordUpdate);

        // Delete
        bool DeleteUser(int id);

        // User Specific operations
        User? UserLoginService(string username, string password);
    }
}