using System;
using System.Collections.Generic;

namespace rental.Models;

public partial class Home
{
    public decimal Id { get; set; }

    public string? Imagepath { get; set; }

    public string? Logopath { get; set; }

    public string? WebsiteName { get; set; }

    public string? Paragraph { get; set; }

    public string? Websitephone { get; set; }

    public string? Wensiteemail { get; set; }
}
