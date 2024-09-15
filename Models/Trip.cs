using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace rental.Models;

public partial class Trip
{
    public decimal TripId { get; set; }
    [Required(ErrorMessage = "Start time is required.")]
    public DateTime? StartTime { get; set; }
    [Required(ErrorMessage = "End time is required.")]
    public DateTime? EndTime { get; set; }
    [Required(ErrorMessage = "Pickup location is required.")]
    public string? LocationPickup { get; set; }
    [Required(ErrorMessage = "Drop-off location is required.")]
    public string? LocationDrop { get; set; }

    public decimal? UserIdfk { get; set; }

    public decimal? CarIdfk { get; set; }

    public string? Status { get; set; } 
    public virtual Car? CarIdfkNavigation { get; set; }

    public virtual UserAccount? UserIdfkNavigation { get; set; }
}
