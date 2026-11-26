using TA_sem3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TA_sem3.Controllers;

public class DestinationsController : Controller
{
    private readonly ApplicationDbContext _db;
    public DestinationsController(ApplicationDbContext db) => _db = db;

    // Katalog: pencarian nama + filter wilayah + filter kategori aktivitas
    public async Task<IActionResult> Index(string? q, string? region, string? category)
    {
        var query = _db.Destinations.AsNoTracking()
            .Include(d => d.DestinationCategories).ThenInclude(dc => dc.Category)
            .Where(d => d.IsActive);

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(d => d.Name.Contains(q.Trim()));
        if (!string.IsNullOrWhiteSpace(region))
            query = query.Where(d => d.Region == region);
        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(d => d.DestinationCategories.Any(dc => dc.Category.CategoryName == category));

        ViewBag.Regions = await _db.Destinations.Where(d => d.IsActive)
            .Select(d => d.Region).Distinct().OrderBy(r => r).ToListAsync();
        ViewBag.Categories = await _db.Categories.AsNoTracking().OrderBy(c => c.CategoryId).ToListAsync();

        return View(await query.OrderBy(d => d.Name).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var d = await _db.Destinations.AsNoTracking()
            .Include(x => x.DestinationCategories).ThenInclude(dc => dc.Category)
            .Include(x => x.DestinationFacilities).ThenInclude(df => df.Facility)
            .FirstOrDefaultAsync(x => x.DestinationId == id);

        // Destinasi nonaktif hanya boleh dilihat Admin
        if (d == null || (!d.IsActive && !User.IsInRole("Admin"))) return NotFound();

        ViewBag.Related = await _db.Destinations.AsNoTracking()
            .Include(x => x.DestinationCategories).ThenInclude(dc => dc.Category)
            .Where(x => x.IsActive && x.DestinationId != id)
            .OrderByDescending(x => x.Region == d.Region)
            .ThenBy(x => x.Name)
            .Take(4).ToListAsync();

        return View(d);
    }
}
