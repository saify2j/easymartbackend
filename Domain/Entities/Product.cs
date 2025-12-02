namespace EasyMart.API.Domain.Entities
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
    }
}
