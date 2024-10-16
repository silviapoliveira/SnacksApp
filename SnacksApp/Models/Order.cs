namespace SnacksApp.Models
{
    public class Order
    {
        public string? Address { get; set; }

        public decimal Total { get; set; }

        public int UserId { get; set; }
    }
}
