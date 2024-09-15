using System;
using System.Collections.Generic;

namespace rental.Models;

public partial class Creditcard
{
    public decimal Id { get; set; }


    public string Name { get; set; } = null!;

    public string Cvv { get; set; } = null!;

    public string Cardnumber { get; set; } = null!;

    public DateTime Expiredate { get; set; }

    public decimal? Balance { get; set; }

    public string Email { get; set; } = null!;

    public decimal? Useracountfk { get; set; }

    public virtual UserAccount? UseracountfkNavigation { get; set; }
}
