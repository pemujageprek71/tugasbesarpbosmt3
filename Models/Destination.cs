using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TA_sem3.Models;

public class Destination
{
    public int DestinationId { get; set; }

    [Required(ErrorMessage = "Nama destinasi wajib diisi.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "Nama harus 3–150 karakter.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Deskripsi wajib diisi.")]
    [StringLength(3000, ErrorMessage = "Deskripsi maksimal 3.000 karakter.")]
    public string Description { get; set; } = string.Empty;

    [Range(1, 9000, ErrorMessage = "Ketinggian harus bilangan bulat positif (meter).")]
    public int? Height { get; set; }

    [Required(ErrorMessage = "Wilayah wajib diisi.")]
    [StringLength(100)]
    public string Region { get; set; } = string.Empty;

    [StringLength(300)]
    public string? Address { get; set; }

    [Range(0, 100000000, ErrorMessage = "Harga tiket tidak boleh negatif.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal? TicketPrice { get; set; }

    [StringLength(100)]
    public string? OpeningHours { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [Url(ErrorMessage = "Tautan Google Maps tidak valid.")]
    [StringLength(500)]
    public string? MapsUrl { get; set; }

    [StringLength(30)]
    public string? Difficulty { get; set; }

    public bool IsActive { get; set; } = true;

    [ValidateNever] public ICollection<DestinationCategory> DestinationCategories { get; set; } = new List<DestinationCategory>();
    [ValidateNever] public ICollection<DestinationFacility> DestinationFacilities { get; set; } = new List<DestinationFacility>();
}
