using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class CreateReservationDto
    {


        [Required]
        [StringLength(100, ErrorMessage = "El nombre es muy largo.")]
        public string CustomerName { get; set; }

        [Required]
        [Phone(ErrorMessage = "El teléfono no es válido.")]
        public string CustomerPhone { get; set; }

        [Required]
        [FutureDate(ErrorMessage = "La fecha debe ser futura.")]
        public DateTime ReservationDate { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Debe haber al menos una persona.")]
        public int NumberOfPeople { get; set; }

        [StringLength(250)]
        public string? Notes { get; set; }

        public bool HasPets { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "El email no es válido.")]
        public string Email { get; set; }
    }

    // Custom validation attribute for future date
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime date)
            {
                return date > DateTime.Now;
            }
            return false;
        }
    }
}