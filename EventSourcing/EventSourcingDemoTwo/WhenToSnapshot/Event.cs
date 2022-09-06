using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhenToSnapshot
{
    internal class Event
    {
        public string Id { get; set; }
        public int Version { get; set; }
    }

    internal class Projection
    {
        public static Projection New => new();

        public int Version { get; set; }
        public static Projection Append(Projection seed,Event @event)
        {
            return null;
        }
    }

    internal interface IStore
    {
        Task<Projection> GetDoc(string id);
        Task<Projection[]> GetDocs(string type);

        Task InsertDoc(string id, Projection data);
        Task<Event[]> GetEvents(String id);
        Task<Event[]> GetEvents(string id, int fromVersion);

        void AddDoc(string id, Projection data);
        void Append(string aggregate,params Event[] events);
        Task SaveChangesAsync();
        void UpStoreDoc(string v, Projection projection);
    }

    internal interface IQueue
    {
        Task PublishAsync(Event e);
    }
}
