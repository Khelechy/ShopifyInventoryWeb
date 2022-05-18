namespace ShopifyInventoryWebClient.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
    }
}
