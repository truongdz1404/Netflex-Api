using System.ComponentModel.DataAnnotations;
namespace Netflex.Models.User
{
    public class EditUserViewModels
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Tên người dùng là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên người dùng không được vượt quá 100 ký tự.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? PhoneNumber { get; set; }

        public List<string>? AvailableRoles { get; set; }

        public List<string>? SelectedRoles { get; set; }
        public virtual DateTimeOffset? LockoutEnd { get; set; }
        public virtual bool LockoutEnabled { get; set; }
    }
}
