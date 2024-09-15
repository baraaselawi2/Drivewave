using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;



namespace rental.Models;

public partial class UserAccount
{
    public decimal UserId { get; set; }
    [Required(ErrorMessage = "Username is required.")]
    
    public string? UserName { get; set; }
    [Required(ErrorMessage = "Phone number is required.")]
    [MinLength(10, ErrorMessage = "Phone must be at least 10 Number long.")]
    [MaxLength(10, ErrorMessage = "Phone number cannot exceed 10 digits.")]


    public string? Phone { get; set; }
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]


    public string? Email { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
  
    public string? Password { get; set; }

    public string? UserImage { get; set; }
    [NotMapped]
    public virtual IFormFile? ImageFile { get; set; }

    public DateTime? CreatDate { get; set; }
    [Required(ErrorMessage = "City is required.")]
    public string? City { get; set; }

    public string? Gender { get; set; }
    [Required(ErrorMessage = "Role is required.")]
    public string? Role { get; set; }

    public virtual ICollection<Creditcard> Creditcards { get; set; } = new List<Creditcard>();

    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
