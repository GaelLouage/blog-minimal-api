using Infrastructuur.Dtos;
using Infrastructuur.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Mappers
{
    public static class UserMapper
    {
        public static UserDto MapToUserDto(this User user)
        {
            return new UserDto
            {
                Name = user.Name,
                Age = user.Age,
                Role = user.Role,
                Email = user.Email,
                Password = user.Password
            };
        }
    }
}
