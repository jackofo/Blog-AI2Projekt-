using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
	public class Comment : BaseEntity
	{
		[Required]
		public string Text { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public Guid PostId { get; set; }
		public IdentityUser IdentityUser { get; set; }
	}
}
