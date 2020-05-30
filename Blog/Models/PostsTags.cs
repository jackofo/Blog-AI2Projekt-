using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
	public class PostsTags : BaseEntity
	{
		public Post Post { get; set; }
		public Tag Tag { get; set; }
	}
}
