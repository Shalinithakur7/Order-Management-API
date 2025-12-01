using System.ComponentModel.DataAnnotations;

namespace OrderManagementApi.Models
{
    /// <summary>
    /// DTO used for client input (POST/PUT requests) to create or update an Order.
    /// Excludes internal properties like Id, TotalAmount (server-calculated), and UserName (server-set).
    /// </summary>
    public class OrderDto
    {
        [Required(ErrorMessage = "Product Name is required.")]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be greater than zero.")]
        public decimal UnitPrice { get; set; }
    }
}