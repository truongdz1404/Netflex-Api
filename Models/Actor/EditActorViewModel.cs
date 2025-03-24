namespace Netflex.Models.Actor
{
    public class EditActorViewModel
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Photo { get; set; }
        public string? About { get; set; }
    }
}
