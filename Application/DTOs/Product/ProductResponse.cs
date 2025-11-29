namespace EasyMart.API.Application.DTOs.Product
{
    public class ProductResponse
    {
        public string ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; } = default!;
    }
}
