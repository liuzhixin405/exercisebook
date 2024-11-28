using System.Text.Json.Serialization;

namespace EF.GraphQL.Demo
{
    public class Address
    {
        [Key: DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Employeeid { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        [JsonIgnore]
        public Employee Employee { get; set; }
    }
}
