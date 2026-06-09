using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace csharp_sprint1_stories.Models;

/// <summary>
/// Represents savings-account-specific data linked to a base account record.
/// </summary>
public partial class SavingsAccount
{
    public long AccountId { get; set; }

    public decimal InterestRate { get; set; }

    public decimal MinimumBalance { get; set; }

    public virtual Account Account { get; set; } = null!;

    /// <summary>
    /// Withdraws money from a savings account.
    /// Savings accounts cannot be overdrawn.
    /// </summary>
    /// <param name="amount">The amount to withdraw.</param>
    /// <returns>The amount withdrawn, or 0 if the withdrawal is not allowed.</returns>
    public decimal Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount should be greater than 0.");
        }

        if (amount > Account.Balance)
        {
            return 0;
        }

        Account.Balance -= amount;
        Account.UpdatedAt = DateTime.Now;

        return amount;
    }

    /// <summary>
    /// Adds interest to the account balance.
    /// Interest due = balance * interest rate / 100.
    /// </summary>
    /// <remarks>
    /// This operation updates the linked account balance directly.
    /// </remarks>
    public void AddInterest()
    {
        decimal interest = Account.Balance * InterestRate / 100;
        Account.Balance += interest;
        Account.UpdatedAt = DateTime.Now;
    }
}
