using System;
using System.Collections.Generic;

namespace csharp_sprint1_stories.Models;

public partial class Person
{
    public long CustomerId { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string? Occupation { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
