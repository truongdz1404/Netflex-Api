using System;
using System.Collections.Generic;

namespace Netflex.Models;

public partial class TblUserClaim
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    public virtual TblUser User { get; set; } = null!;
}
