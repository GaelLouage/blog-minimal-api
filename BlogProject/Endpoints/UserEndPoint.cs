using BlogProject.Extensions;
using Infrastructuur.Dtos;
using Infrastructuur.Entities;
using Infrastructuur.Extensions;
using Infrastructuur.Helpers;
using Infrastructuur.Mappers;
using Infrastructuur.Repositories.Classes;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlogProject.Endpoints
{
    public static class UserEndPoint
    {
        public static void RegisterUserEndPoints(this WebApplication app)
        {
            app.MapGet("/GetUsers", GetUsersAsync).WithDisplayName("GetUsersAsync");
            app.MapGet("/GetUserByName", GetUserByNameAsync).WithDisplayName("GetUserByNameAsync");
            app.MapPost("/AddUserAsync", AddUserAsync).WithDisplayName("AddUserAsync");
            app.MapPatch("/UpdateUser", UpdateUserAsync).WithDisplayName("UpdateUserAsync");
            app.MapPost("/DeleteUserByName", DeleteUserByNameAsync).WithDisplayName("DeleteUserByNameAsync");
        }
        // get all the users
        private static async Task<IResult> GetUsersAsync([FromServices] MongoRepository<User> userRepo)
        {
            var users = (await userRepo.GetAllAsync()).Select(x => x.MapToUserDto());
            var result = new ResultDto<List<UserDto>>();
            result.Result = new List<UserDto>();
            if(users is null || users.Count() == 0)
            {
                result.Errors.Add("No users found!");
                return Results.NotFound(result);
            }
            result.Result.AddRange(users);
            return Results.Ok(result);
        }
        // get the user by name
        private static async Task<IResult> GetUserByNameAsync([FromQuery] string userName, [FromServices] MongoRepository<User> userRepo)
        {
            var user = await userRepo.GetByUserNameAsync(userName);
            var result = new ResultDto<UserDto>();
            if (user is null)
            {
                result.Errors.Add($"No user with name {userName} found!");
                return Results.NotFound(result);
            }
            result.Result = user.MapToUserDto();
            return Results.Ok(result);
        }

       // add a user
       [ValidateAntiForgeryToken]
        private static async Task<IResult> AddUserAsync([FromBody] User user, [FromServices] MongoRepository<User> userRepo)
        {
            var result = new ResultDto<UserDto>();
            var users = await userRepo.GetAllAsync();
            if(users.Any(x => x.Name == user.Name))
            {
                result.Errors.Add($"There is already a user with username : {user.Name} in the database");
                return Results.BadRequest(result);
            }
            if(users.Any(x => x.Email == user.Email))
            {
                result.Errors.Add($"The email address '{user.Email}' is already in use. Please use a different email address or try to reset your password if you have forgotten it.");
                return Results.BadRequest(result);
            }
            // check if the user is valid and every prop is not empty
            var validation = UserValidation.IsValidUserOnCreate(user);
            if (!validation.isvalid)
            {
                result.Errors.Add(validation.error);
                return Results.BadRequest(result);
            }
            // hash the user password
            user.Password = user.Password.HashToPassword();
            result.Result = user.MapToUserDto();
            if (!(await userRepo.InsertAsync(user)))
            {
                result.Errors.Add("ailed to create new user!");
                return Results.BadRequest(result);
            }
            return Results.Ok(result);
        }
        // update a user
        [ValidateAntiForgeryToken]
        private static async Task<IResult> UpdateUserAsync([FromQuery]string userName,  [FromBody]User user, [FromServices] MongoRepository<User> userRepo)
        {
            var result = new ResultDto<UserDto>();
            var userToUpdate =  await userRepo.GetByUserNameAsync(userName);
            
            if(userToUpdate is null)
            {
                result.Errors.Add($"User with name {userName} does not exist!");
                return Results.NotFound(result);
            }
            // check if user has valid email address
            if (!user.Email.IsValidEmail())
            {
                result.Errors.Add($"{user.Email} is not a valid e-mail address");
                return Results.BadRequest(result);
            }
            user.Id = userToUpdate.Id;
            // update the user
            await userRepo.UpdateByUsernameAsync(userName, user);
            result.Result = user.MapToUserDto();
            return Results.Ok(result);
        }
        // delete a user
        [ValidateAntiForgeryToken]
        private static async Task<IResult> DeleteUserByNameAsync([FromQuery] string userName, [FromServices] MongoRepository<User> userRepo)
        {
            var result = new ResultDto<UserDto>();
            var userToDelete = await userRepo.GetByUserNameAsync(userName);

            if (userToDelete is null)
            {
                result.Errors.Add($"User with name {userName} does not exist!");
                return Results.NotFound(result);
            }
            if (!await userRepo.DeleteAsync(userToDelete.Id)) 
            {
                result.Errors.Add($"Failed to delete {userName}!");
                return Results.BadRequest(result);
            }
            return Results.Ok($"Succesfully deleted {userName}");
        }
    }
}
