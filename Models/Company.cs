using System;
using System.Collections.Generic;

namespace csharp_sprint1_stories.Models;

public partial class Company
{
    public long CustomerId { get; set; }

    public string Abn { get; set; } = null!;

    public string Acn { get; set; } = null!;

    public string? Industry { get; set; }

    public string ContactPersonName { get; set; } = null!;

    public string ContactPersonPhone { get; set; } = null!;

    public string ContactPersonEmail { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    /// <summary>
    /// Charges all accounts owned by this company.
    /// Checking accounts are charged the normal amount.
    /// Savings accounts are charged double the amount.
    /// </summary>
    /// <param name="amount">The base amount to charge.</param>
    public void ChargeAllAccounts(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount should be greater than 0.");
        }

        foreach (Account account in Customer.Accounts)
        {
            // check whether this account has a related SavingsAccount record.
            if (account.SavingsAccount != null)
            {
                account.SavingsAccount.Withdraw(amount * 2);
            }
            else
            {
                account.Withdraw(amount);
            }
        }
    }
}
