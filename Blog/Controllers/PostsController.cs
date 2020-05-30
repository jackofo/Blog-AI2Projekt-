using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace Blog.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class PostsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public PostsController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Post>> ListAll()
		{
			IEnumerable<Post> posts = await _context.Posts.ToListAsync();
			foreach (var post in posts)
			{
				post.Category = await _context.Categories.FirstAsync(c => c.Id == post.CategoryId);
			}

			return posts;
		}

		[HttpGet("/Posts/All")]
		public async Task<ActionResult<IEnumerable<Post>>> All()
		{
			ViewData["PostsTags"] = await _context.PostsTags.Include(p => p.Post).Include(p => p.Tag).ToListAsync(); 
			var posts = await ListAll();

			return View("Posts", posts);
		}

		[HttpGet("/Posts/List")]
		public async Task<ActionResult<IEnumerable<Post>>> List()
		{
			if (User.IsInRole("admin"))
			{
				var posts = await ListAll();

				return View(posts);
			}
			else
			{
				return View("AccessDenied");
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Post>> Get(Guid id)
		{
			IdentityUser user = null;
			if (User.Identity.Name != null)
			{
				user = await _context.Users.FirstAsync(u => u.UserName == User.Identity.Name);
			}
			var comments = await _context.Comments.Include(c => c.IdentityUser).Include(c => c.IdentityUser).Where(c => c.PostId == id).ToListAsync();
			var post = await _context.Posts.FirstAsync(p => p.Id == id);
			PostWithComments pwc = new PostWithComments
			{
				CurrentUser = user,
				Post = post,
				Comments = comments
			};

			return View("Get", pwc);
		}

		// GET: api/Posts
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Post>>> Post()
		{
			if (User.IsInRole("admin"))
			{
				ViewData["categories"] = await _context.Categories.ToListAsync();
				return View("Post");
			}
			return Redirect("/Identity/Account/Login");
		}

		// GET: api/Posts
		[HttpGet("/Posts/Edit/{id}")]
		public async Task<ActionResult<IEnumerable<Post>>> Edit(Guid id)
		{
			if (User.IsInRole("admin"))
			{
				ViewData["categories"] = await _context.Categories.ToListAsync();
				return View(await _context.Posts.FirstAsync(p => p.Id == id));
			}
			return Redirect("/Identity/Account/Login");
		}

		[HttpPost("/Posts/Edit/{id}")]
		public async Task<ActionResult<IEnumerable<Post>>> Edit([FromForm] Post post)
		{
			post.UpdatedAt = DateTime.Now;
			_context.Posts.Update(post);
			await _context.SaveChangesAsync();

			return Redirect("/Posts/List");
		}

		// POST: api/Posts
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for
		// more details see https://aka.ms/RazorPagesCRUD.
		[HttpPost]
		public async Task<ActionResult<Post>> Post([FromForm] PostWithTags pwt)
		{

			pwt.Post.Id = Guid.NewGuid();
			IdentityUser currentUser = await _context.Users.FirstAsync(u => u.Email == User.Identity.Name);
			pwt.Post.IdentityUser = currentUser;
			pwt.Post.CreatedAt = DateTime.Now;
			pwt.Post.UpdatedAt = DateTime.Now;
			pwt.Post.Category = await _context.Categories.FirstAsync(p => p.Id == pwt.Post.CategoryId);

			var fileName = Guid.NewGuid().ToString() + ".html";
			var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Posts", fileName);
			using (var stream = new StreamWriter(filePath))
			{
				await stream.WriteAsync(pwt.Post.Text);
			}

			pwt.Post.Text = fileName;
			await _context.Posts.AddAsync(pwt.Post);

			char[] separators = { ' ', ',', ';' };
			string[] tags = pwt.Tags.Split(separators);
			foreach (var t in tags)
			{
				PostsTags pt;
				Tag tag;

				if (_context.Tags.Where(tg => tg.Name == t).Count() <= 0)
				{
					tag = new Tag
					{
						Id = Guid.NewGuid(),
						Name = t
					};
					await _context.Tags.AddAsync(tag);
				}
				else
				{
					tag = await _context.Tags.Where(tg => tg.Name == t).FirstAsync();
				}

				await _context.PostsTags.AddAsync(new PostsTags { Id = Guid.NewGuid(), Tag = tag, Post = pwt.Post });
			}

			await _context.SaveChangesAsync();

			return Redirect("/Posts/List");
		}

		[HttpGet("Delete/{id}")]
		public async Task<ActionResult<Post>> Delete(Guid id)
		{
			if (User.IsInRole("admin"))
			{
				_context.Posts.Remove(await _context.Posts.FirstAsync(p => p.Id == id));
				await _context.SaveChangesAsync();

				return Redirect("/Posts/List");
			}
			else
			{
				return View("AccessDenied");
			}
		}

		[HttpPost("/Comment")]
		public async Task<ActionResult<Comment>> Comment([FromForm] Comment comment)
		{
			if (User.IsInRole("user") || User.IsInRole("admin"))
			{
				comment.IdentityUser = await _context.Users.FirstAsync(u => u.UserName == User.Identity.Name);
				comment.CreatedAt = DateTime.Now;
				comment.UpdatedAt = DateTime.Now;

				await _context.Comments.AddAsync(comment);
				await _context.SaveChangesAsync();

				return Redirect("/Posts/" + comment.PostId);
			}
			else
			{
				return Redirect("AccessDenied");
			}
		}

		[HttpPost("Search")]
		public async Task<ActionResult<Comment>> Search([FromForm] string query)
		{
			List<Post> posts = new List<Post>();
			string[] queries = query.Split(' ');
			if (queries != null)
			{
				foreach (var q in queries)
				{
					var cats = await _context.Categories.Where(c => c.Name == q).ToListAsync();
					var tags = await _context.Tags.Where(t => t.Name == q).ToListAsync();
					var pt = await _context.Posts.Where(p => p.Title == q).ToListAsync();
					var pd = await _context.Posts.Where(p => p.Description == query).ToListAsync();

					foreach (var c in cats)
					{
						foreach (var p in await _context.Posts.Where(p => p.CategoryId == c.Id).ToListAsync())
						{
							posts.Add(p);
						}
					}

					foreach (var t in tags)
					{
						foreach (var pwt in await _context.PostsTags.Where(p => p.Tag.Id == t.Id).Include(pp => pp.Post).ToListAsync())
						{
							posts.Add(await _context.Posts.Where(pp => pp.Id == pwt.Post.Id).FirstAsync());
						}
					}

					posts.AddRange(pt);
					posts.AddRange(pd);
				}
			}

			return View("Posts", posts);
		}
	}
}
