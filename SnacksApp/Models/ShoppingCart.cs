namespace SnacksApp.Models
{
    public class ShoppingCart
    {
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal TotalValue { get; set; }

        public int ProductId { get; set; }

        public int ClientId { get; set; }
    }
}
