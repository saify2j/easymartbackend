namespace EasyMart.API.Application.DTOs.Product
{
    public class ProductAddRequest
    {
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; } = default!;
    }
}
