namespace WebApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal Price { get; set; }  
        public ShipmentState ShipmentState { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }=DateTime.Now;
    }

    
}
