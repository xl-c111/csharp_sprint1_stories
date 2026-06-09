using System;
using System.Collections.Generic;

namespace csharp_sprint1_stories.Models;

/// <summary>
/// Represents person-specific customer data linked to a shared customer record.
/// </summary>
public partial class Person
{
    public long CustomerId { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string? Occupation { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    /// <summary>
    /// Charges the same amount from every account owned by this person.
    /// </summary>
    /// <param name="amount">The amount to charge from each account.</param>
    /// <exception cref="ArgumentException">Thrown when the amount is less than or equal to zero.</exception>
    public void ChargeAllAccounts(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount should be greater than 0.");
        }

        foreach (Account account in Customer.Accounts)
        {
            if (account.SavingsAccount != null)
            {
                account.SavingsAccount.Withdraw(amount);
            }
            else
            {
                account.Withdraw(amount);
            }
        }
    }
}
