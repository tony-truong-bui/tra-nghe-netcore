
using System.ComponentModel.DataAnnotations;


namespace TraNgheCore.Models
{
    public class CreateUserViewModel
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public List<string> Role { get; set; }
    }
}