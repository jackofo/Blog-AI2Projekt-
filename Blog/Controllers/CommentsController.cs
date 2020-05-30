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
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> List()
        {
            if (User.IsInRole("admin"))
            {
                return View(await _context.Comments.Include(c => c.IdentityUser).ToListAsync());
            }
            else
            {
                return View("AccessDenied");
            }
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> Edit(Guid id)
        {
            if (User.IsInRole("admin") || User.IsInRole("user"))
            {
                var comment = await _context.Comments.FindAsync(id);

                if (comment == null)
                {
                    return NotFound();
                }

                return View(comment);
            }
            else
            {
                return View("AccessDenied");
            }
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<Comment>> Edit([FromForm] Comment comment)
        {
            if (User.IsInRole("admin") || User.IsInRole("user"))
            {
                comment.UpdatedAt = DateTime.Now;
                _context.Comments.Update(comment);
                await _context.SaveChangesAsync();

                return Redirect(User.IsInRole("admin") ? "/Comments" : "/Posts/"+comment.PostId);
            }
            else
            {
                return View("AccessDenied");
            }
        }

        // POST: api/Comments
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
        }

        // DELETE: api/Comments/5
        [HttpGet("Delete/{id}")]
        public async Task<ActionResult<Comment>> Delete(Guid id)
        {
            if (User.IsInRole("admin") || User.IsInRole("user"))
            {
                var comment = await _context.Comments.FindAsync(id);
                if (comment == null)
                {
                    return NotFound();
                }

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                return Redirect("/");
            }
            else
            {
                return View("AccessDenied");
            }
        }

        private bool CommentExists(Guid id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
