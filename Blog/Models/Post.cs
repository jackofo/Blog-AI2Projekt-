using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
	public class Post : BaseEntity
	{
		[Required]
		public string Title { get; set; }
		[Required]
		public string Description { get; set; }
		[Required]
		public string Text { get; set; }
		public string ImagePath { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public IdentityUser IdentityUser { get; set; }
		[Required]
		public Guid CategoryId { get; set; }
		public Category Category { get; set; }
	}
}
