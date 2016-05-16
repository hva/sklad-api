namespace Warehouse.Server.Models
{
    public class ProductPriceUpdate
    {
        public string Id { get; set; }
        public long NewPriceOpt { get; set; }
        public long NewPriceRozn { get; set; }
    }
}
