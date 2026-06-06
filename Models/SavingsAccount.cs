using System;
using System.Collections.Generic;

namespace csharp_sprint1_stories.Models;

public partial class SavingsAccount
{
    public long AccountId { get; set; }

    public decimal InterestRate { get; set; }

    public decimal MinimumBalance { get; set; }

    public virtual Account Account { get; set; } = null!;
}
