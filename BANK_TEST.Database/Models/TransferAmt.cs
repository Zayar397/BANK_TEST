using System;
using System.Collections.Generic;

namespace BANK_TEST.Database.Models;

public partial class TransferAmt
{
    public int Id { get; set; }

    public string FrMobileNo { get; set; } = null!;

    public string ToMobileNo { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime CreatedDate { get; set; }
}
