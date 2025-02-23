using System;
using System.Collections.Generic;

namespace Netflex.Models;

public partial class TblEpisode
{
    public Guid Id { get; set; }

    public int Number { get; set; }

    public string Title { get; set; } = null!;

    public string? About { get; set; }

    public string Path { get; set; } = null!;

    public TimeOnly HowLong { get; set; }

    public Guid SerieId { get; set; }

    public virtual TblSeries Serie { get; set; } = null!;
}
