using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
	public class PostWithTags
	{
		public Post Post { get; set; }
		public string Tags { get; set; }
	}
}
