using Infrastructuur.Entities;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Helpers
{
    public static class JwtTokenGenerator
    {
        private static readonly string Secret = "mykey";

        public static string GenerateToken(User user)
        {
            string secretKey = GenerateSecretKey();

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, nameof(user.Role))
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "your-issuer",
                audience: "your-audience",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string ExtractTokenFromAuthHeader(string authHeader)
        {
            // Check if the authorization header is null or empty
            if (string.IsNullOrEmpty(authHeader))
            {
                throw new ArgumentException("Authorization header cannot be null or empty");
            }

            // Check if the authorization header starts with "Bearer "
            if (!authHeader.StartsWith("Bearer "))
            {
                throw new ArgumentException("Invalid authorization header format");
            }

            // Extract the token from the authorization header
            return authHeader.Substring("Bearer ".Length);
        }

        private static string GenerateSecretKey()
        {
            const int keySize = 32; // The size of the key in bytes

            // Use a cryptographically secure random number generator to generate a new key
            using var generator = new RNGCryptoServiceProvider();
            var key = new byte[keySize];
            generator.GetBytes(key);

            // Convert the key to a Base64-encoded string for use in the JWT token
            return Convert.ToBase64String(key);
        }
    }
}
