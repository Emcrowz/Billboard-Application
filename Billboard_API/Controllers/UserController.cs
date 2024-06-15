using Billboard_BackEnd.Contracts;
using Billboard_BackEnd.Models;
using Billboard_BackEnd.ModelsDTO;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Billboard_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        #region SETUP / INITIALISATION
        readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        #endregion

        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            Log.Information($"Attempt to fetch {typeof(User).Name} records.");
            IEnumerable<User> users = _userService.GetAllUsers();
            if (users != null)
            {
                Log.Information($"Successfully read records from DB. Count of records: {users.Count()}");
                return Ok(users);
            }
            else
            {
                Log.Error($"Failed to fetch {typeof(User).Name} records from the DB.");
                return NotFound();
            }
        }

        [HttpPost("Register")]
        public IActionResult Register(UserDTO newUser)
        {
            Log.Information($"Attempt to create {typeof(User).Name} record.");
            if (_userService.CreateNewUser(newUser))
            {
                Log.Information($"Successfully created new {typeof(User).Name} instance.");
                return Ok(newUser);
            }
            else
            {
                Log.Error($"Failed to create {typeof(User).Name} instance in the DB.");
                return NotFound();
            }
        }

        [HttpPost("UpdateUser/{id}")]
        public IActionResult UpdateUser(int id, UserDTO userUpdate)
        {
            Log.Information($"Attempt to update {typeof(User).Name} record with ID: {id}.");
            if (_userService.UpdateUserDetailsById(id, new User() {
                FirstName = userUpdate.FirstName,
                LastName = userUpdate.LastName,
                Email = userUpdate.Email,
                Username = userUpdate.Username,
                Password = userUpdate.Password
            }))
            {
                Log.Information($"Successfully updated {typeof(User).Name} ID: {id} instance.");
                return Ok();
            }
            else
            {
                Log.Error($"Failed to update {typeof(User).Name} instance in the DB with ID: {id}.");
                return NotFound();
            }
        }

        [HttpPost("LoginUser")]
        public IActionResult LoginUser(string username, string password)
        {
            Log.Information($"Attempt to login with Username: [{username}] Password: [{password}]");
            User? user = _userService.UserLoginService(username, password);
            if (user != null)
            {
                Log.Information($"Successful login to - ID: [{user.UserId}] Username [{user.Username}]");
                return Ok(user);
            }
            else
            {
                Log.Error($"Failed to login with Username: [{username}] Password: [{password}]");
                return Unauthorized();
            }
        }

        [HttpDelete("DeleteUserById/{id}")]
        public IActionResult DeleteUserById(int id, string username, string password)
        {
            Log.Information($"Attempt to login with Username: [{username}] Password: [{password}]");
            User? user = _userService.UserLoginService(username, password);
            if (user != null) 
            { 
                Log.Information($"Attempt to delete {typeof(User).Name} record with ID: {id}");
                if (_userService.DeleteUser(username, password))
                {
                    Log.Information($"Successfully deleted record with ID: {id} inside DB");
                    return Ok();
                }
                else
                {
                    Log.Error($"Failed to delete {typeof(User).Name} record inside DB with ID: {id}");
                    return NotFound();
                }
            }
            else
            {
                Log.Error($"Failed to login with Username: [{username}] Password: [{password}]");
                return Unauthorized();
            }
        }
    }
}
