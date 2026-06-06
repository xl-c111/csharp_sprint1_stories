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
}
