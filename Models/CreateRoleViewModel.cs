
using System.ComponentModel.DataAnnotations;

namespace TraNgheCore.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role Name")]
        [StringLength(100, ErrorMessage = "Role name cannot exceed 100 characters")]
        public string RoleName { get; set; }

    }
}