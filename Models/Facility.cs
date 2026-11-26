using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TA_sem3.Models;

public class Facility
{
    public int FacilityId { get; set; }

    [Required, StringLength(60)]
    public string FacilityName { get; set; } = string.Empty;

    [ValidateNever] public ICollection<DestinationFacility> DestinationFacilities { get; set; } = new List<DestinationFacility>();
}
