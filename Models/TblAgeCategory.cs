using System;
using System.Collections.Generic;

namespace Netflex.Models;

public partial class TblAgeCategory
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TblFilm> TblFilms { get; set; } = new List<TblFilm>();

    public virtual ICollection<TblSeries> TblSeries { get; set; } = new List<TblSeries>();
}
