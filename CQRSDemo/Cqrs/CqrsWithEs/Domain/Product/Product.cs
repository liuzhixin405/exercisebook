using Newtonsoft.Json;

namespace CqrsWithEs.Domain.Product
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        [JsonProperty]
        private List<Cover> covers = new List<Cover>();
        [JsonProperty]
        public IEnumerable<Cover> Covers=>covers.AsReadOnly();
        public Product(Guid id,string code,string name,IList<Cover> covers)
        {

            Id = id;
            Code = code;
            Name = name;
            foreach (var cover in covers)
            {
                this.covers.Add(cover);
            }
        }
        public Product() { }
        public Cover CoverWithCode(string coverCode)=>covers.FirstOrDefault(c=>c.Code==coverCode);
    }
}
