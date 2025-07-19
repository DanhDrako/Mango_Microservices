using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ProductAPI.Models
{
    public class OrderDetails : BaseEntity
    {
        [Key]
        public int OrderDetailsId { get; set; }

        public int ProductId { get; set; }
        [NotMapped]
        public ProductDto? Product { get; set; }

        public int Count { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }

        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public OrderHeader? OrderHeader { get; set; }
    }
}
