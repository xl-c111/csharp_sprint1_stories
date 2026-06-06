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
}
