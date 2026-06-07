using System;
using System.Collections.Generic;

namespace csharp_sprint1_stories.Models;

public partial class Customer
{
    public long CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string CustomerType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual Company? Company { get; set; }

    public virtual Person? Person { get; set; }

    /// <summary>
    /// Adds an account to this customer's account list.
    /// </summary>
    /// <param name="account">The account to add.</param>
    public void AddAccount(Account account)
    {
        if (account == null)
        {
            throw new ArgumentNullException(nameof(account));
        }

        Accounts.Add(account);
        account.Customer = this;
        account.CustomerId = CustomerId;
    }

    /// <summary>
    /// Removes an account from this customer's account list.
    /// </summary>
    /// <param name="account">The account to remove.</param>
    public void RemoveAccount(Account account)
    {
        if (account == null)
        {
            throw new ArgumentNullException(nameof(account));
        }

        Accounts.Remove(account);
    }

    /// <summary>
    /// Charges all accounts owned by the customer.
    /// This method should be implemented by Person or Company.
    /// </summary>
    /// <param name="amount">The amount to charge.</param>
    public virtual void ChargeAllAccounts(decimal amount)
    {
        throw new NotImplementedException("ChargeAllAccounts should be implemented by Person or Company.");
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
