namespace Netflex.Models.Actor
{
    public class CreateActorViewModel
    {
        public required string Name { get; set; }
        public string? Photo { get; set; }
        public string? About { get; set; }
    }
}
