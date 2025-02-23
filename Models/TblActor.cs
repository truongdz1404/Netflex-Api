using System;
using System.Collections.Generic;

namespace Netflex.Models;

public partial class TblActor
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Photo { get; set; }

    public string? About { get; set; }

    public virtual ICollection<TblFilm> Films { get; set; } = new List<TblFilm>();

    public virtual ICollection<TblSeries> Series { get; set; } = new List<TblSeries>();
}
