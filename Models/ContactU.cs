using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace rental.Models;

public partial class ContactU
{
    public decimal ContactId { get; set; }
    [Required(ErrorMessage = "FullName is required.")]
    public string FullName { get; set; } = null!;
    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; } = null!;
    [Required(ErrorMessage = "Message is required.")]
    public string Message { get; set; } = null!;

    public DateTime ContactDate { get; set; }
    [Required(ErrorMessage = "Subject is required.")]

    public string? Subject { get; set; }
}
