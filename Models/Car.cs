using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace rental.Models;

public partial class Car
{
    public decimal CarId { get; set; }

    public string CarNumber { get; set; } = null!;

    public string CarName { get; set; } = null!;

    public string CarColor { get; set; } = null!;

    public decimal CarSet { get; set; }

    public string? Imagepath { get; set; }
    [NotMapped]
    public virtual IFormFile? ImageFile { get; set; }


    public string? CarValue2 { get; set; }

    public string? CarValue3 { get; set; }

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
