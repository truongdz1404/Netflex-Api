using System;
using System.Collections.Generic;

namespace Netflex.Models;

public partial class TblGenre
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TblFilm> Films { get; set; } = new List<TblFilm>();

    public virtual ICollection<TblSeries> Series { get; set; } = new List<TblSeries>();
}
