namespace Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public DateTime ReservationDate { get; set; }
        public int NumberOfPeople { get; set; }
        public string? Notes { get; set; }
        public bool HasPets { get; set; }
        public string Email { get; set; }
    }
}