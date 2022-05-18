namespace ShopifyInventoryWebClient.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }
        public int Quantity { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
