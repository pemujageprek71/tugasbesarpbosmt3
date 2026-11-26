using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TA_sem3.Models;

public class Category
{
    public int CategoryId { get; set; }

    [Required, StringLength(60)]
    public string CategoryName { get; set; } = string.Empty;

    [ValidateNever] public ICollection<DestinationCategory> DestinationCategories { get; set; } = new List<DestinationCategory>();
}
