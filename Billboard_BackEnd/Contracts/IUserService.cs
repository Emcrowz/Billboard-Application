using Billboard_BackEnd.Models;
using Billboard_BackEnd.ModelsDTO;

namespace Billboard_BackEnd.Contracts
{
    public interface IUserService
    {
        // Create 
        bool CreateNewUser(UserDTO newUser);

        // Read / Get
        IEnumerable<User> GetAllUsers();
        User? GetUserById(int id);

        // Update
        bool UpdateUserDetailsById(int id, User userUpdate);

        // Delete
        bool DeleteUser(string username, string password);

        // User Specific operations
        User? UserLoginService(string username, string password);
    }
}