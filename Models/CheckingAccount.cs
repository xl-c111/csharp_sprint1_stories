using System;
using System.Collections.Generic;

namespace csharp_sprint1_stories.Models;

public partial class CheckingAccount
{
    public long AccountId { get; set; }

    public int NextCheckNumber { get; set; }

    public decimal OverdraftLimit { get; set; }

    public virtual Account Account { get; set; } = null!;

    public int GetNextCheckNumber()
    {
        if (NextCheckNumber < 1)
        {
            NextCheckNumber = 1;
        }

        int currentCheckNumber = NextCheckNumber;
        NextCheckNumber++;
        Account.UpdatedAt = DateTime.Now;

        return currentCheckNumber;
    }

}
