using System;
using System.Collections.Generic;

namespace Netflex.Models;

public partial class TblFollow
{
    public string FollowerId { get; set; } = null!;

    public Guid FilmId { get; set; }

    public Guid SerieId { get; set; }

    public DateTime FollowedAt { get; set; }

    public virtual TblFilm Film { get; set; } = null!;

    public virtual TblUser Follower { get; set; } = null!;

    public virtual TblSeries Serie { get; set; } = null!;
}
