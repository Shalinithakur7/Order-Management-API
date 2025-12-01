using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagementApi.Models
{
    public class Order
    {
        public int Id { get; set; }

        // Assignment requirement: Product Name
        public string ProductName { get; set; } = string.Empty;

        // Assignment requirement: Quantity
        public int Quantity { get; set; }

        // Assignment requirement: Unit Price
        public decimal UnitPrice { get; set; }

        // Assignment requirement: Total Amount (Calculated on retrieval or saving, but defined as a field)
        // This is stored in the DB for persistence
        public decimal TotalAmount { get; set; }

        // Tracks the user who owns this order
        public string? UserName { get; set; }
    }
}