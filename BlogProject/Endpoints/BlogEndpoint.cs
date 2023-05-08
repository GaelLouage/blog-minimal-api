using Infrastructuur.Dtos;
using Infrastructuur.Entities;
using Infrastructuur.Mappers;
using Infrastructuur.Repositories.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using Infrastructuur.Extensions;
using Infrastructuur.Helpers;

namespace BlogProject.Endpoints
{
    public static class BlogEndpoint
    {
        public static void RegisterBlogEndPoints(this WebApplication app)
        {
            app.MapGet("/GetAllBLogsAsync", GetAllBLogsAsync).WithDisplayName("GetAllBLogsAsync");
            app.MapGet("/GetBlogByTitleAsync", GetBlogByTitleAsync).WithDisplayName("GetBlogByTitleAsync");
            app.MapPost("/CreateBlogAsync", CreateBlogAsync).WithDisplayName("CreateBlogAsync");
            app.MapPatch("/UpdateBlogByTitleAsync", UpdateBlogByTitleAsync).WithDisplayName("UpdateBlogByTitleAsync");
            app.MapDelete("/DeleteBlogByTitleAsync", DeleteBlogByTitleAsync).WithDisplayName("DeleteBlogByTitleAsync");
        }
        // get all blogs
        private static async Task<IResult> GetAllBLogsAsync([FromServices] MongoRepository<Blog> blogRepository)
        {
            var result = new ResultDto<List<BlogDto>>();
            // get all the blogs and map them
            var blogs = (await blogRepository.GetAllAsync()).Select(x => x.MapToBlogDto());
            if(blogs is null || blogs.Count() == 0)
            {
                result.Errors.Add("No blogs found!");
                return Results.NotFound(result);
            }
            result.Result = new List<BlogDto>();
            result.Result = blogs.ToList(); 
            return Results.Ok(result);
        }
        // get blog by title
        private static async Task<IResult> GetBlogByTitleAsync([FromQuery]string blogTitle,[FromServices] MongoRepository<Blog> blogRepository)
        {
            var result = new ResultDto<BlogDto>();
            var blog = (await blogRepository.GetAllAsync()).FirstOrDefault(x => x.Title == blogTitle);
            if(blog == null)
            {
                result.Errors.Add($"No blog with title '{blogTitle}' found!");
                return Results.NotFound(result);
            }
            result.Result = blog.MapToBlogDto();   
            return Results.Ok(result);
        }
        // create blog
        private static async Task<IResult> CreateBlogAsync([FromBody] Blog blog, [FromServices] MongoRepository<Blog> blogRepository, [FromServices] MongoRepository<User> userRepository)
        {
            var result = new ResultDto<BlogDto>();
            var validation = await BlogValidation.ValidationCreationAsync(blog, blogRepository, userRepository);
            if (!validation.isValid)
            {
                result.Errors.Add(validation.result);
                return Results.BadRequest(result);
            }
            return Results.Ok("Succesfully added blog!");
        }
       
        // update blog by title
        private static async Task<IResult> UpdateBlogByTitleAsync([FromQuery] string blogTitle,[FromBody] Blog blog, [FromServices] MongoRepository<Blog> blogRepository)
        {
            var result = new ResultDto<Blog>();
            var blogToUpdate = (await blogRepository.GetAllAsync()).FirstOrDefault(x => x.Title == blogTitle);
            var blogs = (await blogRepository.GetAllAsync());
            var validation = BlogValidation.ValidationUpdate(blog , blogToUpdate,blogs);
            if(!validation.isValid)
            {
                result.Errors.Add(validation.result);
                return Results.BadRequest(result);
            }
            // update the blog
            blog.Author = blogToUpdate.Author;
            blog.Id = blogToUpdate.Id;
            await blogRepository.UpdateAsync(blogToUpdate.Id, blog);
            
            return Results.Ok("Succesfully updated blog.");
        }
    
        
        // delete blog by title
        private static async Task<IResult> DeleteBlogByTitleAsync([FromQuery] string blogTitle, [FromServices] MongoRepository<Blog> blogRepository)
        {
            var result = new ResultDto<Blog>();
            var blogToUpdate = (await blogRepository.GetAllAsync()).FirstOrDefault(x => x.Title == blogTitle);
            if (blogToUpdate is null)
            {
                result.Errors.Add($"Blog with title {blogTitle} not found!");
                return Results.NotFound(result);
            }
            //delete the blog 
            if(!await blogRepository.DeleteAsync(blogToUpdate.Id))
            {
                result.Errors.Add($"Failed to delete the blog!");
                return Results.BadRequest(result);
            }
            return Results.Ok($"Succesfully deleted blog {blogTitle}!");
        }
    }
}
