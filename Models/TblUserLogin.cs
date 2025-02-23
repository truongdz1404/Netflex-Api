using System;
using System.Collections.Generic;

namespace Netflex.Models;

public partial class TblUserLogin
{
    public string LoginProvider { get; set; } = null!;

    public string ProviderKey { get; set; } = null!;

    public string? ProviderDisplayName { get; set; }

    public string UserId { get; set; } = null!;

    public virtual TblUser User { get; set; } = null!;
}
