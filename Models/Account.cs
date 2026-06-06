using System;
using System.Collections.Generic;

namespace csharp_sprint1_stories.Models;

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
}
