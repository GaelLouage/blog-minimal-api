using Infrastructuur.Entities;
using Infrastructuur.Extensions;
using Infrastructuur.Repositories.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Helpers
{
    public static class BlogValidation
    {
        public static (bool isValid, string result) ValidationUpdate(Blog blog, Blog blogToUpdate, IEnumerable<Blog?> blogs)
        {
            if (blogToUpdate is null)
            {
                return (isValid: false, $"Blog with title {blog.Title} not found!");
            }
            var validChars = blog.Content.IsMaxChars();
            if (validChars.isMaxChars)
            {
                return (isValid: false, validChars.result);
            }
            if (blogs.Any(x => x.Title == blog.Title))
            {
                return (isValid: false, "There is already a blog with this title");
            }
            return (isValid: true, "");
        }
        public static async Task<(bool isValid, string result)> ValidationCreationAsync(Blog blog, MongoRepository<Blog> blogRepository, MongoRepository<User> userRepository)
        {
            var blogExist = (await blogRepository.GetAllAsync()).Any(x => x.Title == blog.Title);
            if (blogExist)
            {
                return (isValid: false, result: $"There is already a blog witth title {blog.Title}");
            }
            // check if the author exist(user)
            var author = await userRepository.GetByUserNameAsync(blog.Author.Name);
            if (author == null)
            {
                return (isValid: false, result: $"Author(user) with name '{blog.Author.Name}' does not exist!");
            }
            blog.Author = author;
            // blog validation inputs
            if (string.IsNullOrEmpty(blog.Content) ||
                string.IsNullOrEmpty(blog.Title))
            {
                return (isValid: false, result: "Title and Content are required.");
            }
            var validChars = blog.Content.IsMaxChars();
            if (validChars.isMaxChars)
            {
                return (isValid: false, result: validChars.result);
            }
            // categories length should be at least 1
            if (blog.Categories.Count == 0)
            {
                return (isValid: false, result: $"There are no categories assigned to your blog!");
            }
            // add blog to database
            if (!await blogRepository.InsertAsync(blog))
            {
                return (isValid: false, result: "Failed to add blog!");
            }
            return (isValid: true, result: "");
        }
    }
}
