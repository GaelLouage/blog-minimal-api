using Infrastructuur.Dtos;
using Infrastructuur.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Mappers
{
    public static class BlogMappers
    {
        public static BlogDto MapToBlogDto(this Blog blog)
        {
            return new BlogDto
            {
                Title = blog.Title,
                Categories = blog.Categories,
                PublishDate = blog.PublishDate,
                Author = blog.Author.MapToUserDto(),
                Content = blog.Content,
            };
        }
    }
}
