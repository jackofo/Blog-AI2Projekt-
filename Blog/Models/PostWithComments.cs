using System.Collections.Generic;
using Blog.Models;
using Microsoft.AspNetCore.Identity;

namespace Blog.Models
{
    public class PostWithComments
    {
        public IdentityUser CurrentUser { get; set; }
        public Post Post { get; set; }
        public Comment Comment { get; set; }
        public List<Comment> Comments { get; set; }
    }
}