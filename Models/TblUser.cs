using System;
using System.Collections.Generic;

namespace Netflex.Models;

public partial class TblUser
{
    public string Id { get; set; } = null!;

    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public virtual ICollection<TblBlog> TblBlogs { get; set; } = new List<TblBlog>();

    public virtual ICollection<TblFollow> TblFollows { get; set; } = new List<TblFollow>();

    public virtual ICollection<TblReview> TblReviews { get; set; } = new List<TblReview>();

    public virtual ICollection<TblUserClaim> TblUserClaims { get; set; } = new List<TblUserClaim>();

    public virtual ICollection<TblUserLogin> TblUserLogins { get; set; } = new List<TblUserLogin>();

    public virtual ICollection<TblUserToken> TblUserTokens { get; set; } = new List<TblUserToken>();

    public virtual ICollection<TblNotification> Notifications { get; set; } = new List<TblNotification>();

    public virtual ICollection<TblRole> Roles { get; set; } = new List<TblRole>();
}
