using CqrsLibrary.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsLibrary.Domain
{
    public class InventoryItem:AggregationRoot
    {
        private bool activated;
        private Guid id;

        private void Apply(InventoryItemCreated e)
        {
            this.id = e.Id;
            activated = true;
        }

        private void Apply(InventoryItemDeactivated e)
        {
            activated = false;
        }
        public void ChangeName(string newName)
        {
            if(string.IsNullOrEmpty(newName))throw new ArgumentNullException(nameof(newName));
            ApplyChange(new InventoryItemRenamed(id,newName));
        }

        public void CheckIn(int count)
        {
            if (count <= 0) throw new InvalidOperationException("must have a count greater than 0 to add to inventory");
            ApplyChange(new ItemsCheckedInToInventory(id, count));
        }
        public void Remove(int count)
        {
            if (count <= 0) throw new InvalidOperationException("cant remove negative count from inventory");
            ApplyChange(new ItemsRemovedFromInventory(id, count));
        }
        public void Deactivate()
        {
            if (!activated) throw new InvalidOperationException("already deactivated");
            ApplyChange(new InventoryItemDeactivated(id));
        }
        public override Guid Id => id;
        public InventoryItem()
        {
            //init other 
        }
        public InventoryItem(Guid id,string name)
        {
            ApplyChange(new InventoryItemCreated(id, name));
        }
    }
}
