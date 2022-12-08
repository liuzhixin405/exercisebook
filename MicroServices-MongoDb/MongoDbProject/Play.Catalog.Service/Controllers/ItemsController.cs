using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Entities;
using Play.Common;
using static Play.Catalog.Contracts.Contrtacts;

namespace Play.Catalog.Service.Controllers
{
    [Route("items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;
        private readonly IPublishEndpoint publishEndpoint;

        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
        {
            this.itemsRepository = itemsRepository;
            this.publishEndpoint = publishEndpoint;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            var items = (await itemsRepository.GetAllAsync())
                        .Select(item => item.AsDto());

            return Ok(items);
        }

        // GET /items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item.AsDto();
        }

        // POST /items
        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await itemsRepository.CreateAsync(item);

            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }

        // PUT /items/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await itemsRepository.GetAsync(id);

            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await itemsRepository.UpdateAsync(existingItem);

            await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

            return NoContent();
        }

        // DELETE /items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            await itemsRepository.RemoveAsync(item.Id);

            await publishEndpoint.Publish(new CatalogItemDeleted(id));

            return NoContent();
        }
    }
}
