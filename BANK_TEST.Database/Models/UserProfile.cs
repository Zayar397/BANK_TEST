using System;
using System.Collections.Generic;

namespace BANK_TEST.Database.Models;

public partial class UserProfile
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string MobileNo { get; set; } = null!;

    public decimal? Balance { get; set; }

    public string Pin { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public bool IsActive { get; set; }
}
