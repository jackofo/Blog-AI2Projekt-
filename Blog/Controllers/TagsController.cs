using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models;

namespace Blog.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Tags
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<Tag>>> ListAll()
        {
            return await _context.Tags.ToListAsync();
        }

        // GET: api/Tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> List()
        {
            return View(await _context.Tags.ToListAsync());
        }

        [HttpGet("Create")]
        public ActionResult<IEnumerable<Tag>> Create()
        {
            return View();
        }

        // GET: api/Tags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> Edit(Guid id)
        {
            if (User.IsInRole("admin"))
            {
                var tag = await _context.Tags.FindAsync(id);

                if (tag == null)
                {
                    return NotFound();
                }

                return View(tag);
            }
            else
            {
                return View("AccessDenied");
            }
        }

        // POST: api/Tags
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Tag>> PostTag([FromForm] Tag tag)
        {
            tag.Id = Guid.NewGuid();
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return Redirect("/Tags");
        }

        [HttpPost("Update")]
        public async Task<ActionResult<Tag>> Update([FromForm] Tag tag)
        {
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();

            return Redirect("/Tags");
        }

        [HttpGet("Posts/{id}")]
        public async Task<ActionResult<IList<Post>>> GetPostsByTag(Guid id)
        {
            List<Post> posts = new List<Post>();
            var poststags = await _context.PostsTags.Where(pt => pt.Tag.Id == id).Include(p => p.Post).ToListAsync();
            foreach (var pt in poststags)
            {
                posts.Add(await _context.Posts.FirstAsync(p => p.Id == pt.Post.Id));
            }

            if (posts == null)
            {
                return NotFound();
            }

            return View("Posts", posts);
        }

        
        [HttpGet("Delete/{id}")]
        public async Task<ActionResult<Tag>> Delete(Guid id)
        {
            if (User.IsInRole("admin"))
            {
                _context.Tags.Remove(await _context.Tags.FirstAsync(t => t.Id == id));
                await _context.SaveChangesAsync();

                return Redirect("/Tags");
            }
            else
            {
                return View("AccessDenied");
            }
        }
    }
}
