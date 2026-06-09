using System;
using System.Collections.Generic;

namespace csharp_sprint1_stories.Models;

/// <summary>
/// Represents checking-account-specific data linked to a base account record.
/// </summary>
public partial class CheckingAccount
{
    public long AccountId { get; set; }

    public int NextCheckNumber { get; set; }

    public decimal OverdraftLimit { get; set; }

    public virtual Account Account { get; set; } = null!;

    /// <summary>
    /// Returns the current check number and advances the sequence for the next check.
    /// If the stored sequence is invalid, it is reset to 1 before issuing a number.
    /// </summary>
    /// <returns>The check number issued for the current request.</returns>
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
