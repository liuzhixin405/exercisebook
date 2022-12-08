using MassTransit;
using Play.Common;
using Play.Inventory.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Play.Catalog.Contracts.Contrtacts;

namespace Play.Inventory.Service.Consumers
{
    internal class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated>
    {
        private readonly IRepository<CatalogItem> _repository;
        public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository)
        {
            _repository = repository;
        }
        public async Task Consume(ConsumeContext<CatalogItemCreated> context)
        {
            var message = context.Message;
            var item = await _repository.GetAsync(message.ItemId);
            if (item != null)
            {
                return;
            }

            item = new CatalogItem
            {
                Id = message.ItemId,
                Name = message.Name,
                Description = message.Description
            };

            await _repository.CreateAsync(item);
        }
    }
}
