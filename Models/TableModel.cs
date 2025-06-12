
using System.ComponentModel.DataAnnotations;


namespace TraNgheCore.Models
{
    public class TableModel
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100, ErrorMessage = "Table name cannot exceed 100 characters")]
        [Display(Name = "Table Name")]
        public string Name { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; } = true;


    }
}