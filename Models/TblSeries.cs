using System;
using System.Collections.Generic;

namespace Netflex.Models;

public partial class TblSeries
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? About { get; set; }

    public string? Poster { get; set; }

    public int ProductionYear { get; set; }

    public Guid? AgeCategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual TblAgeCategory? AgeCategory { get; set; }

    public virtual ICollection<TblEpisode> TblEpisodes { get; set; } = new List<TblEpisode>();

    public virtual ICollection<TblFollow> TblFollows { get; set; } = new List<TblFollow>();

    public virtual ICollection<TblReview> TblReviews { get; set; } = new List<TblReview>();

    public virtual ICollection<TblActor> Actors { get; set; } = new List<TblActor>();

    public virtual ICollection<TblCountry> Countries { get; set; } = new List<TblCountry>();

    public virtual ICollection<TblGenre> Genres { get; set; } = new List<TblGenre>();
}
