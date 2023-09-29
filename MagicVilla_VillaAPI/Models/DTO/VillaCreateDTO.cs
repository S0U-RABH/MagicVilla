using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
{
    public class VillaCreateDTO
    {
        
        /*
         A data transfer object (DTO) is an object that carries data between processes. 
         You can use this technique to facilitate communication between two systems (like an API and your server)
         */

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public string Details { get; set; }
        [Required]
        public double Rate { get; set; }
        public int Sqft { get; set; }
        public int Occupancy { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
        public string Amenity { get; set; }
    }
}
