
using System.ComponentModel.DataAnnotations;


namespace TraNgheCore.Models
{
    public class ProductModel
    {

        [Key]
        public int Id { get; set; }

        
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Product Name")]
        public string Name { get; set; }


        //[StringLength(255, ErrorMessage = "Image URL cannot exceed 255 characters")]
        //[Display(Name = "Image URL")]
        //[Url(ErrorMessage = "Please enter a valid URL")]
        //public string ImageUrl { get; set; }



        [Display(Name = "Product Description")]
        public string Description { get; set; }


        [Display(Name = "Category")]
        public int Category { get; set; }

        [Display(Name = "Is Currently Served")]
        public bool IsServed { get; set; }

        
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }
    }
}