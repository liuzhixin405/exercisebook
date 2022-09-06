using CqrsLibrary.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsLibrary.Commands
{
    public class InventoryCommandHandlers
    {
        private readonly IRepository<InventoryItem> repository;
        public InventoryCommandHandlers(IRepository<InventoryItem> repository)
        {
            this.repository = repository;
        }

        public void Handle(CreateInventoryItem message)
        {
            var item = new InventoryItem(message.InventoryItemId,message.Name);
            repository.Save(item, -1);
        }

        public void Handle(DeactivateInventoryItem message)
        {
            var item = repository.GetById(message.InventoryItemId);
            item.Deactivate();
            repository.Save(item, message.OriginalVersion);
        }

        public void Handle(RemoveItemsFromInventory message)
        {
            var item = repository.GetById(message.InventoryItemId);
            item.Remove(message.Count);
            repository.Save(item, message.OriginalVersion);
        }

        public void Handle(CheckInItemsToInventory message)
        {
            var item = repository.GetById(message.InventoryItemId);
            item.CheckIn(message.Count);
            repository.Save(item, message.OriginalVersion);
        }

        public void Handle(RenameInventoryItem message)
        {
            var item = repository.GetById(message.InventoryItemId);
            item.ChangeName(message.NewName);
            repository.Save(item, message.OriginalVersion);
        }
    }
}
