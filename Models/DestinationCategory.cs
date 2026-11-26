using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TA_sem3.Models;

// Tabel penghubung many-to-many Destination <-> Category
public class DestinationCategory
{
    public int DestinationId { get; set; }
    public int CategoryId { get; set; }

    [ValidateNever] public Destination Destination { get; set; } = null!;
    [ValidateNever] public Category Category { get; set; } = null!;
}
