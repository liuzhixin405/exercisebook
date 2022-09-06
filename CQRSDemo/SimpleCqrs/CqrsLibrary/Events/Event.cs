using CqrsLibrary.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsLibrary.Events
{
    public class Event:Message
    {
        public int Version;
    }

    public class InventoryItemDeactivated : Event
    {
        private readonly Guid id;
        public InventoryItemDeactivated(Guid guid)
        {
            id = guid;
        }

        public Guid Id => id;
    }

    public class InventoryItemCreated : Event
    {
        private readonly Guid id;
        private readonly string name;
        public InventoryItemCreated(Guid id,string name)
        {
            this.id = id;
            this.name = name;
        }

        public Guid Id => id;

        public string Name => name;
    }

    public class InventoryItemRenamed : Event
    {
        private readonly Guid id;

        private readonly string name;
        public InventoryItemRenamed(Guid id, string newName)
        {
            this.id = id;
            this.name = newName;
        }

        public Guid Id => id;

        public string Name => name;
    }

    public class ItemsCheckedInToInventory : Event
    {
        private readonly Guid id;
        private readonly int count;
        public ItemsCheckedInToInventory(Guid id, int count)
        {
            this.id = id;
            this.count = count;
        }

        public Guid Id => id;

        public int Count => count;
    }

    public class ItemsRemovedFromInventory : Event
    {
        private readonly Guid id;
        private readonly int count;
        public ItemsRemovedFromInventory(Guid id,int count)
        {
            this.id = id;
            this.count = count;
        }

        public Guid Id => id;

        public int Count => count;
    }
}
