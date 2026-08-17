using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // عرض جميع المهام
        public async Task<IActionResult> Index(string statusFilter, string sortOrder)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var tasksQuery = _context.Tasks.Where(t => t.UserId == user.Id);

            if (!string.IsNullOrEmpty(statusFilter))
                tasksQuery = tasksQuery.Where(t => t.Status == statusFilter);

            if (sortOrder == "desc")
            {
                tasksQuery = tasksQuery.OrderByDescending(t => t.DueDate);
                ViewBag.SortOrder = "asc";
            }
            else
            {
                tasksQuery = tasksQuery.OrderBy(t => t.DueDate);
                ViewBag.SortOrder = "desc";
            }

            ViewBag.StatusFilter = new SelectList(new[] { "To Do", "In Progress", "Done" });

            var tasks = await tasksQuery.ToListAsync();
            return View(tasks);
        }

        // إنشاء مهمة جديدة
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Status,DueDate")] TaskModel taskModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
                taskModel.UserId = user.Id;

            ModelState.Remove("User");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                _context.Add(taskModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taskModel);
        }

        // تعديل مهمة
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (task.UserId != user.Id)
                return Forbid();

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Status,DueDate,UserId")] TaskModel taskModel)
        {
            if (id != taskModel.Id)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            taskModel.UserId = user.Id;

            ModelState.Remove("User");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                _context.Update(taskModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taskModel);
        }

        // عرض تفاصيل
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

            if (task == null)
                return NotFound();

            return View(task);
        }

        // حذف مهمة
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var task = await _context.Tasks.FirstOrDefaultAsync(m => m.Id == id && m.UserId == user.Id);

            if (task == null)
                return NotFound();

            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
