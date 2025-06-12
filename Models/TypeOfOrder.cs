
using System.ComponentModel.DataAnnotations;

namespace TraNgheCore.Models
{
    public class TypeOfOrder
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Order type name is required")]
        [StringLength(100, ErrorMessage = "Order type name cannot exceed 100 characters")]
        [Display(Name = "Order Type Name")]
        public string Name { get; set; }



    }
}