using TA_sem3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TA_sem3.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var featured = await _db.Destinations.AsNoTracking()
            .Include(d => d.DestinationCategories).ThenInclude(dc => dc.Category)
            .Where(d => d.IsActive)
            .OrderBy(d => d.DestinationId)
            .Take(4)
            .ToListAsync();

        ViewBag.Regions = await _db.Destinations.Where(d => d.IsActive)
            .Select(d => d.Region).Distinct().OrderBy(r => r).ToListAsync();
        ViewBag.Categories = await _db.Categories.AsNoTracking().OrderBy(c => c.CategoryId).ToListAsync();

        return View(featured);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}