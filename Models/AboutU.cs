using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace rental.Models;

public partial class AboutU
{
    public decimal Id { get; set; }

    public string? Text { get; set; }

    public string? Imagepath { get; set; }
    [NotMapped]
    public virtual IFormFile? ImageFile { get; set; }

}
