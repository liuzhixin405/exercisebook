using Merp.Infrastructure;
using Merp.Registry.QueryStack;
using Merp.Registry.QueryStack.Model;
using Merp.Web.UI.Areas.Registry.Models;
using Merp.Web.UI.Areas.Registry.Models.Party;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Merp.Web.UI.Areas.Registry.WorkerServices
{
    public class PartyControllerWorkerServices
    {
        public IBus Bus { get; private set; }
        public IDatabase Database { get; set; }

        public PartyControllerWorkerServices(IBus bus, IDatabase database)
        {
            if(bus==null)
            {
                throw new ArgumentNullException("bus");
            }
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            this.Bus = bus;
            this.Database = database;
        }

        public string GetDetailViewModel(int partyId)
        {
            if(Database.Parties.OfType<Company>().Where(p => p.Id == partyId).Count()==1)
            {
                return "Company";
            }
            else if (Database.Parties.OfType<Person>().Where(p => p.Id == partyId).Count() == 1)
            {
                return "Person";
            }
            else
            {
                return "Unknown";
            }
        }

        public IEnumerable<object> GetPartyNamesByPattern(string text)
        {
            var model = from p in Database.Parties
                        where p.DisplayName.StartsWith(text)
                        orderby p.DisplayName ascending
                        select new PartyInfo { Id = p.Id, OriginalId = p.OriginalId, Name = p.DisplayName };
            return model;
        }

        public PartyInfo GetPartyInfoByPattern(int id)
        {
            var model = (from p in Database.Parties
                         where p.Id == id
                         select new PartyInfo { Id = p.Id, OriginalId = p.OriginalId, Name = p.DisplayName }).Single();
            return model;
        }

        public IEnumerable<object> GetPersonNamesByPattern(string text)
        {
            var model = from p in Database.Parties.OfType<Person>()
                        where p.DisplayName.StartsWith(text)
                        orderby p.DisplayName ascending
                        select new PartyInfo { Id = p.Id, OriginalId = p.OriginalId, Name = p.DisplayName };
            return model;
        }

        public PartyInfo GetPersonInfoByPattern(int id)
        {
            var model = (from p in Database.Parties.OfType<Person>()
                         where p.Id == id
                         select new PartyInfo { Id = p.Id, OriginalId = p.OriginalId, Name = p.DisplayName }).Single();
            return model;
        }
        public IEnumerable<GetPartiesViewModel> GetParties(string query)
        {
            var model = from p in Database.Parties
                        orderby p.DisplayName ascending
                        select new GetPartiesViewModel { id = p.Id, name = p.DisplayName };
            if(!string.IsNullOrEmpty(query) && query!="undefined")
            {
                model = model.Where(p => p.name.StartsWith(query));
            }
            model = model.Take(50);
            return model;
        }
    }
}