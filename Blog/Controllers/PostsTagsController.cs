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
    public class PostsTagsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostsTagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PostsTags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostsTags>>> GetPostsTags()
        {
            return await _context.PostsTags.ToListAsync();
        }

        // GET: api/PostsTags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostsTags>> GetPostsTags(Guid id)
        {
            var postsTags = await _context.PostsTags.FindAsync(id);

            if (postsTags == null)
            {
                return NotFound();
            }

            return postsTags;
        }

        // PUT: api/PostsTags/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPostsTags(Guid id, PostsTags postsTags)
        {
            if (id != postsTags.Id)
            {
                return BadRequest();
            }

            _context.Entry(postsTags).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostsTagsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PostsTags
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<PostsTags>> PostPostsTags(PostsTags postsTags)
        {
            _context.PostsTags.Add(postsTags);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostsTags", new { id = postsTags.Id }, postsTags);
        }

        // DELETE: api/PostsTags/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PostsTags>> DeletePostsTags(Guid id)
        {
            var postsTags = await _context.PostsTags.FindAsync(id);
            if (postsTags == null)
            {
                return NotFound();
            }

            _context.PostsTags.Remove(postsTags);
            await _context.SaveChangesAsync();

            return postsTags;
        }

        private bool PostsTagsExists(Guid id)
        {
            return _context.PostsTags.Any(e => e.Id == id);
        }
    }
}
