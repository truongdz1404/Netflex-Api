using System.Collections.Generic;

namespace Netflex.Models
{
    public class ChangeRoleViewModel
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public List<string>? CurrentRoles { get; set; }
        public List<string>? AvailableRoles { get; set; }
        public List<string>? SelectedRoles { get; set; }
    }
}
