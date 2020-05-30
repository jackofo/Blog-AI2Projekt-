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
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<Category>>> List()
        {
            return await _context.Categories.ToListAsync();
        }

        // GET: api/Categories
        [HttpGet("Create")]
        public ActionResult<Category> Create()
        {
            return View();
        }

        [HttpPost("/Add")]
        public async Task<ActionResult<IEnumerable<Category>>> Add([FromForm] Category category)
        {
            if (User.IsInRole("admin"))
            {
                category.Id = Guid.NewGuid();
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                return Redirect("/Categories");
            }
            else
            {
                return View("AccessDenied");
            }
        }

        [HttpGet("/Delete/{id}")]
        public async Task<ActionResult<Category>> Delete(Guid id)
        {
            if (User.IsInRole("admin"))
            {
                _context.Categories.Remove(await _context.Categories.FirstAsync(c => c.Id == id));
                await _context.SaveChangesAsync();

                return Redirect("/Categories");
            }
            else
            {
                return View("AccessDenied");
            }
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            if (User.IsInRole("admin"))
            {
                return View("Categories", await _context.Categories.ToListAsync());
            }
            else
            {
                return View("AccessDenied");
            }
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // GET: api/Categories
        [HttpGet("/Edit/{id}")]
        public async Task<ActionResult<IEnumerable<Category>>> Edit(Guid id)
        {
            return View("Edit", await _context.Categories.FirstAsync(c => c.Id == id));
        }

        // GET: api/Categories
        [HttpPost("/Edit/{id}")]
        public async Task<ActionResult<IEnumerable<Category>>> Edit([FromForm] Category category)
        {
            if(User.IsInRole("admin"))
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                return Redirect("/Categories");
            }
            else
            {
                return View("AccessDenied");
            }
        }

        // GET: api/Categories/5
        [HttpGet("Posts/{id}")]
        public async Task<ActionResult<IList<Post>>> GetPostsByCategory(Guid id)
        {
            var posts = await _context.Posts.Where(p => p.CategoryId == id).ToListAsync();

            if (posts == null)
            {
                return NotFound();
            }

            return View("Posts", posts);
        }

        // POST: api/Categories
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("/Edit/{id}")]
        public async Task<ActionResult<Category>> PostCategory(Guid id, [FromForm] Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();

            return Redirect("/Categories");
        }
    }
}
