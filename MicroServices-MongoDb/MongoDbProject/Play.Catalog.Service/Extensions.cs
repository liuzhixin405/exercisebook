using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service
{
    public static class Extensions
    {
        public static ItemDto AsDto(this Item item) => new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
    }
}
