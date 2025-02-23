using System;
using System.Collections.Generic;

namespace Netflex.Models;

public partial class TblUserToken
{
    public string UserId { get; set; } = null!;

    public string LoginProvider { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Value { get; set; }

    public virtual TblUser User { get; set; } = null!;
}
