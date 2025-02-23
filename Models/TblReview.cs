using System;
using System.Collections.Generic;

namespace Netflex.Models;

public partial class TblReview
{
    public string CreaterId { get; set; } = null!;

    public Guid FilmId { get; set; }

    public Guid SerieId { get; set; }

    public int Rating { get; set; }

    public virtual TblUser Creater { get; set; } = null!;

    public virtual TblFilm Film { get; set; } = null!;

    public virtual TblSeries Serie { get; set; } = null!;
}
