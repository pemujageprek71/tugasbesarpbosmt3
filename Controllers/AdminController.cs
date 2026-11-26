using TA_sem3.Data;
using TA_sem3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TA_sem3.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private const string Fields = "Name,Description,Height,Region,Address,TicketPrice,OpeningHours,ImageUrl,MapsUrl,Difficulty,IsActive";
    private static readonly string[] AllowedExt = { ".jpg", ".jpeg", ".png", ".webp" };

    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public AdminController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Dashboard()
    {
        var list = await _db.Destinations.AsNoTracking().OrderBy(d => d.Name).ToListAsync();
        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadLookupsAsync();
        return View(new Destination { IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Fields)] Destination model, int[] selectedCategoryIds, int[] selectedFacilityIds, IFormFile? imageFile)
    {
        var (path, error) = await SaveImageAsync(imageFile);
        if (error != null) ModelState.AddModelError("ImageUrl", error);

        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync(selectedCategoryIds, selectedFacilityIds);
            return View(model);
        }

        if (path != null) model.ImageUrl = path;
        await SyncRelationsAsync(model, selectedCategoryIds, selectedFacilityIds);
        _db.Destinations.Add(model);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Destinasi “{model.Name}” berhasil ditambahkan.";
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var d = await _db.Destinations.AsNoTracking()
            .Include(x => x.DestinationCategories).Include(x => x.DestinationFacilities)
            .FirstOrDefaultAsync(x => x.DestinationId == id);
        if (d == null) return NotFound();

        await LoadLookupsAsync(
            d.DestinationCategories.Select(x => x.CategoryId),
            d.DestinationFacilities.Select(x => x.FacilityId));
        return View(d);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("DestinationId," + Fields)] Destination model, int[] selectedCategoryIds, int[] selectedFacilityIds, IFormFile? imageFile)
    {
        if (id != model.DestinationId) return NotFound();

        var d = await _db.Destinations
            .Include(x => x.DestinationCategories).Include(x => x.DestinationFacilities)
            .FirstOrDefaultAsync(x => x.DestinationId == id);
        if (d == null) return NotFound();

        var (path, error) = await SaveImageAsync(imageFile);
        if (error != null) ModelState.AddModelError("ImageUrl", error);

        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync(selectedCategoryIds, selectedFacilityIds);
            return View(model);
        }

        var oldImage = d.ImageUrl;
        d.Name = model.Name;
        d.Description = model.Description;
        d.Height = model.Height;
        d.Region = model.Region;
        d.Address = model.Address;
        d.TicketPrice = model.TicketPrice;
        d.OpeningHours = model.OpeningHours;
        d.MapsUrl = model.MapsUrl;
        d.Difficulty = model.Difficulty;
        d.IsActive = model.IsActive;
        d.ImageUrl = path ?? model.ImageUrl;

        await SyncRelationsAsync(d, selectedCategoryIds, selectedFacilityIds);
        await _db.SaveChangesAsync();

        if (path != null && oldImage != null && oldImage.StartsWith("/images/destinasi/"))
        {
            var oldFile = Path.Combine(_env.WebRootPath, oldImage.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(oldFile)) System.IO.File.Delete(oldFile);
        }

        TempData["Success"] = $"Perubahan “{d.Name}” berhasil disimpan.";
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var d = await _db.Destinations.FindAsync(id);
        if (d == null) return NotFound();

        d.IsActive = !d.IsActive;
        await _db.SaveChangesAsync();

        TempData["Success"] = $"“{d.Name}” sekarang {(d.IsActive ? "aktif" : "nonaktif")}.";
        return RedirectToAction(nameof(Dashboard));
    }

    private async Task LoadLookupsAsync(IEnumerable<int>? selectedCats = null, IEnumerable<int>? selectedFacs = null)
    {
        ViewBag.Regions = await _db.Destinations.Select(d => d.Region).Distinct().OrderBy(r => r).ToListAsync();
        ViewBag.Categories = await _db.Categories.AsNoTracking().OrderBy(c => c.CategoryId).ToListAsync();
        ViewBag.Facilities = await _db.Facilities.AsNoTracking().OrderBy(f => f.FacilityName).ToListAsync();
        ViewBag.SelectedCategoryIds = (selectedCats ?? Enumerable.Empty<int>()).ToList();
        ViewBag.SelectedFacilityIds = (selectedFacs ?? Enumerable.Empty<int>()).ToList();
    }

    private async Task SyncRelationsAsync(Destination d, int[] catIds, int[] facIds)
    {
        var validCats = await _db.Categories.Where(c => catIds.Contains(c.CategoryId)).Select(c => c.CategoryId).ToListAsync();
        var validFacs = await _db.Facilities.Where(f => facIds.Contains(f.FacilityId)).Select(f => f.FacilityId).ToListAsync();

        var removeCats = d.DestinationCategories.Where(x => !validCats.Contains(x.CategoryId)).ToList();
        _db.DestinationCategories.RemoveRange(removeCats);
        foreach (var id in validCats.Where(id => d.DestinationCategories.All(x => x.CategoryId != id)))
            d.DestinationCategories.Add(new DestinationCategory { CategoryId = id });

        var removeFacs = d.DestinationFacilities.Where(x => !validFacs.Contains(x.FacilityId)).ToList();
        _db.DestinationFacilities.RemoveRange(removeFacs);
        foreach (var id in validFacs.Where(id => d.DestinationFacilities.All(x => x.FacilityId != id)))
            d.DestinationFacilities.Add(new DestinationFacility { FacilityId = id });
    }

    private async Task<(string? path, string? error)> SaveImageAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0) return (null, null);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExt.Contains(ext) || !file.ContentType.StartsWith("image/"))
            return (null, "Format foto harus JPG, PNG, atau WebP.");
        if (file.Length > 5 * 1024 * 1024)
            return (null, "Ukuran foto maksimal 5 MB.");

        var dir = Path.Combine(_env.WebRootPath, "images", "destinasi");
        Directory.CreateDirectory(dir);
        var name = $"{Guid.NewGuid():N}{ext}";
        await using var fs = System.IO.File.Create(Path.Combine(dir, name));
        await file.CopyToAsync(fs);
        return ($"/images/destinasi/{name}", null);
    }
}