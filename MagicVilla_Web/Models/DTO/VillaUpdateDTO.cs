using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Web.Models.DTO
{
    public class VillaUpdateDTO
    {
        /*
         A data transfer object (DTO) is an object that carries data between processes. 
         You can use this technique to facilitate communication between two systems (like an API and your server)
         */
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public string Details { get; set; }
        [Required]
        public double Rate { get; set; }
        [Required]
        public int Sqft { get; set; }
        [Required]
        public int Occupancy { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageLocalPath { get; set; }
        public IFormFile? Image { get; set; }
        public string Amenity { get; set; }
    }
}
