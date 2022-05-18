namespace ShopifyInventoryWebClient.Models
{
    public class Item
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string? SKU { get; set; }
        public decimal Cost { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsSoftDelete { get; set; }
        public string? Comment { get; set; }
    }
}
