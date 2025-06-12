
using System.ComponentModel.DataAnnotations;


namespace TraNgheCore.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        [Display(Name = "Category Name")]
        public string Name { get; set; }


    }
}