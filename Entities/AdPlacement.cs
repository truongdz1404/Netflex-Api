namespace Netflex.Entities;

public class AdPlacement
{
    public Guid AdId { get; set; }
    public Guid PlacementId { get; set; }
    public int Priority { get; set; }
}