

using System.ComponentModel.DataAnnotations;

namespace API.Data.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string username { get; set; }
        [Required] public string KnownAs { get; set; }

        [Required] public string Gender { get; set; }
        [Required] public DateOnly? DateOfBirth { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string password { get; set; }
    }
}