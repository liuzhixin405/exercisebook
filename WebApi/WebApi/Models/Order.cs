namespace WebApi.Models
{
    public class Order:IEntity<int>
    {
        public int Id { get; set; }
        public int ProductId { get; set; }  
        public ShipmentState ShipmentState { get; set; }
        public DateTime CreateTime { get; set; }=DateTime.Now;
       
    }

    
}
