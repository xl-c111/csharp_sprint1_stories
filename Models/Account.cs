using System;
using System.Collections.Generic;
namespace csharp_sprint1_stories.Models;

/// <summary>
/// Represents a bank account owned by a customer.
/// This base record stores shared account data for both checking and savings accounts.
/// </summary>
public partial class Account
{
    public long AccountId { get; set; }

    public long CustomerId { get; set; }

    public string AccountType { get; set; } = null!;

    public decimal Balance { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual CheckingAccount? CheckingAccount { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual SavingsAccount? SavingsAccount { get; set; }

    /// <summary>
    /// Deposits money into the account.
    /// </summary>
    /// <param name="amount">The amount to deposit.</param>
    /// <exception cref="ArgumentException">Thrown when the amount is less than or equal to zero.</exception>
    /// <summary>
    /// Withdraws money from the account. Base account withdrawals can overdraw the balance.
    /// </summary>
    /// <param name="amount">The amount to withdraw.</param>
    /// <returns>The amount withdrawn.</returns>
    /// <exception cref="ArgumentException">Thrown when the amount is less than or equal to zero.</exception>
    public virtual decimal Withdraw(decimal amount)
    {
        ValidateAmount(amount);

        Balance -= amount;
        UpdatedAt = DateTime.Now;

        return amount;
    }

    public void Deposit(decimal amount)
    {
        ValidateAmount(amount);

        Balance += amount;
        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// Replaces the account balance with a specific value.
    /// </summary>
    /// <param name="amount">The new balance value.</param>
    /// <exception cref="ArgumentException">Thrown when a savings account is assigned a negative balance.</exception>
    public void CorrectBalance(decimal amount)
    {
        if (AccountType == "Savings" && amount < 0)
        {
            throw new ArgumentException("Savings accounts cannot have a negative balance.");
        }

        Balance = amount;
        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// Marks the account as inactive.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// Validates that an amount is greater than zero.
    /// </summary>
    /// <param name="amount">The amount to validate.</param>
    /// <exception cref="ArgumentException">Thrown when the amount is less than or equal to zero.</exception>
    protected void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount should be greater than 0.");
        }
    }
}
