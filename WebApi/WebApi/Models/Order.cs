namespace WebApi.Models
{
    public class Order:IEntity<int>
    {
        public int Id { get; set; }
        public decimal Price { get; set; }  
        public ShipmentState ShipmentState { get; set; }
        public string Name { get; set; }
        //public DateTime DateTime { get; set; }=DateTime.Now;
        public Order(string name)
        {
            this.Name= name;
        }
    }

    
}
