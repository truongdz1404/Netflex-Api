using System;
using System.Collections.Generic;

namespace Netflex.Models;

public partial class TblNotification
{
    public Guid Id { get; set; }

    public string Content { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<TblUser> Users { get; set; } = new List<TblUser>();
}
