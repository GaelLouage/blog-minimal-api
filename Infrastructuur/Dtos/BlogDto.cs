using Infrastructuur.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructuur.Dtos
{
    public class BlogDto
    {
        public string Title { get; set; }
        public List<string> Categories { get; set; } = new List<string>();
        public UserDto Author { get; set; }
        public DateTime PublishDate { get; set; }
        public string Content { get; set; }
    }
}
