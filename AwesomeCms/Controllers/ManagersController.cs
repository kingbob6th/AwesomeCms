using AwesomeCms.DataContext;
using AwesomeCms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace AwesomeCms.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ManagersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        private readonly string folderSeparator;

        public ManagersController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;

            folderSeparator = _configuration.GetSection("FolderSeparator").Value;
        }

        // GET: api/Managers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Manager>>> GetManagers()
        {
            return await _context.Managers.ToListAsync();
        }

        // GET: api/Managers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Manager>> GetManager(int id)
        {
            var manager = await _context.Managers.FindAsync(id);

            if (manager == null)
            {
                return NotFound();
            }

            string rootPath = _webHostEnvironment.WebRootPath ?? _webHostEnvironment.ContentRootPath;
            var uploads = Path.Combine(rootPath, $"uploads{folderSeparator}managers");
            manager.ImageLocation = Path.Combine(uploads, manager.ImageLocation);
            return manager;
        }

        // POST: api/Managers
        [HttpPost]
        public async Task<ActionResult<Manager>> PostManager([FromForm] Manager manager, [FromForm] IFormFile imageFile)
        {
            string fullImgPath = "";
            if (imageFile != null)
            {
                string rootPath = _webHostEnvironment.WebRootPath ?? _webHostEnvironment.ContentRootPath;
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(rootPath, $"uploads{folderSeparator}managers");
                var extension = Path.GetExtension(imageFile.FileName);

                if (!Directory.Exists(uploads))
                {
                    DirectoryInfo folder = Directory.CreateDirectory(uploads);
                }
                using (var filestream =
                       new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    await imageFile.CopyToAsync(filestream);
                }
                manager.ImageLocation = fileName + extension;
                fullImgPath = Path.Combine(uploads, fileName + extension);
            }

            manager.CreatedDate = DateTime.Now;

            _context.Managers.Add(manager);
            await _context.SaveChangesAsync();

            manager.ImageLocation = fullImgPath;
            return CreatedAtAction("GetManager", new { id = manager.ManagerId }, manager);
        }

        // PUT: api/Managers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutManager(int id, [FromForm] Manager manager, [FromForm] IFormFile? imageFile)
        {
            if (id != manager.ManagerId)
            {
                return BadRequest();
            }

            var existingManager = (await _context.Managers.ToListAsync()).Single(x => x.ManagerId == id);
            string fullImgPath = "";
            if (imageFile != null)
            {
                //upload image + update image file name
                string rootPath = _webHostEnvironment.WebRootPath ?? _webHostEnvironment.ContentRootPath;
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(rootPath, $"uploads{folderSeparator}managers");
                var extension = Path.GetExtension(imageFile.FileName);

                if (System.IO.File.Exists(Path.Combine(uploads, existingManager.ImageLocation)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, existingManager.ImageLocation));
                }
                using (var filestream =
                       new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    await imageFile.CopyToAsync(filestream);
                }
                existingManager.ImageLocation = fileName + extension;
                fullImgPath = Path.Combine(uploads, fileName + extension);
            }

            existingManager.UpdatedDate = DateTime.Now;
            existingManager.BaseSalary = manager.BaseSalary;
            existingManager.ManagerName = manager.ManagerName;
            existingManager.DateOfBirth = manager.DateOfBirth;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManagerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Manager Updated successfully");
        }

        // DELETE: api/Managers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManager(int id)
        {
            var manager = await _context.Managers.FindAsync(id);
            if (manager == null)
            {
                return NotFound();
            }

            _context.Managers.Remove(manager);
            await _context.SaveChangesAsync();

            return Ok("Manager Removed successfully");
        }

        private bool ManagerExists(int id)
        {
            return _context.Managers.Any(e => e.ManagerId == id);
        }
    }
}
