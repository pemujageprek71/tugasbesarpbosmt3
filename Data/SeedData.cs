using TA_sem3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TA_sem3.Data;

public static class SeedData
{
    public const string AdminRole = "Admin";
    public const string AdminEmail = "admin@jelajahgunung.id";
    public const string AdminPassword = "Admin@12345";   // GANTI setelah login pertama / sebelum deploy

    public static async Task InitializeAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();

        // ---- Role & akun admin ----
        var roles = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var users = sp.GetRequiredService<UserManager<IdentityUser>>();
        if (!await roles.RoleExistsAsync(AdminRole))
            await roles.CreateAsync(new IdentityRole(AdminRole));

        if (await users.FindByEmailAsync(AdminEmail) == null)
        {
            var admin = new IdentityUser { UserName = AdminEmail, Email = AdminEmail, EmailConfirmed = true };
            var result = await users.CreateAsync(admin, AdminPassword);
            if (result.Succeeded) await users.AddToRoleAsync(admin, AdminRole);
        }

        // ---- Kategori & fasilitas ----
        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Category { CategoryName = "Hiking & Pendakian" },
                new Category { CategoryName = "Camping" },
                new Category { CategoryName = "Wisata Kawah" },
                new Category { CategoryName = "Sunrise & Panorama" },
                new Category { CategoryName = "Rekreasi Pegunungan" });
        }
        if (!await db.Facilities.AnyAsync())
        {
            db.Facilities.AddRange(
                new Facility { FacilityName = "Toilet" },
                new Facility { FacilityName = "Mushola" },
                new Facility { FacilityName = "Area Parkir" },
                new Facility { FacilityName = "Warung / Kantin" },
                new Facility { FacilityName = "Camping Ground" },
                new Facility { FacilityName = "Pos Pendakian" });
        }
        await db.SaveChangesAsync();

        // ---- Destinasi contoh (harga, jam buka, fasilitas sengaja dikosongkan: belum diverifikasi) ----
        if (!await db.Destinations.AnyAsync())
        {
            var cat = await db.Categories.ToDictionaryAsync(c => c.CategoryName, c => c.CategoryId);

            Destination Make(string name, string region, int height, string desc, params string[] cats) => new()
            {
                Name = name, Region = region, Height = height, Description = desc, IsActive = true,
                DestinationCategories = cats.Select(c => new DestinationCategory { CategoryId = cat[c] }).ToList()
            };

            db.Destinations.AddRange(
                Make("Gunung Patuha (Kawah Putih)", "Kabupaten Bandung", 2434,
                    "Kawasan kawah vulkanik di Ciwidey dengan danau berwarna putih kehijauan dan udara sejuk pegunungan.",
                    "Wisata Kawah", "Rekreasi Pegunungan"),
                Make("Gunung Papandayan", "Garut", 2665,
                    "Gunung api aktif dengan kawah belerang, hutan mati, dan area camping di Pondok Saladah.",
                    "Hiking & Pendakian", "Camping", "Wisata Kawah"),
                Make("Gunung Gede", "Cianjur", 2958,
                    "Gunung populer di kawasan Taman Nasional Gunung Gede Pangrango dengan padang edelweis Alun-Alun Suryakencana.",
                    "Hiking & Pendakian", "Camping", "Sunrise & Panorama"),
                Make("Gunung Tangkuban Perahu", "Bandung Barat", 2084,
                    "Kawah vulkanik yang mudah diakses dengan panorama khas kawasan Lembang.",
                    "Wisata Kawah", "Rekreasi Pegunungan"),
                Make("Gunung Cikuray", "Garut", 2821,
                    "Gunung di Garut dengan jalur pendakian menanjak dan panorama luas dari puncak.",
                    "Hiking & Pendakian", "Sunrise & Panorama"),
                Make("Gunung Ciremai", "Kuningan", 3078,
                    "Gunung tertinggi di Jawa Barat, berada di kawasan Taman Nasional Gunung Ciremai.",
                    "Hiking & Pendakian", "Camping"));
            await db.SaveChangesAsync();
        }
    }
}
