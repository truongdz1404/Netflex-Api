using System;
using System.Collections.Generic;

namespace Netflex.Models;

public partial class TblRole
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public virtual ICollection<TblRoleClaim> TblRoleClaims { get; set; } = new List<TblRoleClaim>();

    public virtual ICollection<TblUser> Users { get; set; } = new List<TblUser>();
}
