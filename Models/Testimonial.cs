using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace rental.Models;

public partial class Testimonial
{
    public decimal CommentId { get; set; }

    public string? UserName { get; set; }
    [Required(ErrorMessage = "Message is required.")]
    public string? Message { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatDate { get; set; }

    public decimal? UserIdfk { get; set; }

    public string? ImagePath { get; set; }
    [NotMapped]
    public virtual IFormFile? ImageFile { get; set; }

    public virtual UserAccount? UserIdfkNavigation { get; set; }
}
