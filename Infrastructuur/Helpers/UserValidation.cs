using Infrastructuur.Entities;
using Infrastructuur.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Helpers
{
    public class UserValidation
    {
        public static (bool isvalid, string error) IsValidUserOnCreate(User user)
        {
            if (string.IsNullOrEmpty(user.Name) ||
                string.IsNullOrEmpty(user.Age.ToString()) ||
                string.IsNullOrEmpty(user.Email) ||
                string.IsNullOrEmpty(user.Role.ToString()) ||
                string.IsNullOrEmpty(user.Password))
            {
                return (isvalid: false, error: "All Fields are required");
            }
            if (!user.Email.IsValidEmail())
            {
                return (isvalid: false, error: "Invalid email address!");
            }

            return (isvalid: true, error: "");
        }
    }
}
