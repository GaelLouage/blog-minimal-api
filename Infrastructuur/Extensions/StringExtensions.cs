using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructuur.Extensions
{
    public static class StringExtensions
    {
        public static (bool isMaxChars, string result) IsMaxChars(this string blogContent) => blogContent.Length > 2000 ? (true, "Blog content cannot exceed 2000 chars!") : (false, "");
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                // Normalize the email address by removing whitespace and converting to lowercase
                email = Regex.Replace(email.Trim(), @"\s+", "").ToLower();

                // Use the built-in .NET EmailAddress class to validate the email address
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
