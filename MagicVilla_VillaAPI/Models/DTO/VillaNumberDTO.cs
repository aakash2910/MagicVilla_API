using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
{
    public class VillaNumberDTO
    {
        [Required]
        public int VillaNum { get; set; }
        public string? SpecialDetails { get; set;}

        [Required]
        public int VillaId { get; set; }
    }
}
