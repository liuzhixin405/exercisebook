using CqrsWithEs.Domain.Offer;
using CqrsWithEs.Init;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace CqrsWithEs.DataAccess
{
    public class InMemoryOfferRepository : IOfferRepository
    {
        private readonly IDictionary<string, Offer> offers = new ConcurrentDictionary<string, Offer>();
        public InMemoryOfferRepository()
        {
            var offer001 = SampleOfferData.SampleOffer("OFF001");
            offers.Add(offer001.Number, offer001);
        }

        public Task<Offer> WithNumber(string number)
        {
            return Task.FromResult(offers[number]);
        }

        public void Add(Offer offer)
        {
            offers.Add(offer.Number,offer);
        }

        public Task<ReadOnlyCollection<Offer>> All()
        {
            return Task.FromResult(offers.Values.ToList().AsReadOnly());
        }

        public Task<Offer> GetById(Guid id)
        {
            var result = offers.Values.Where(x => x.Id == id).FirstOrDefault();           
            return Task.FromResult(result);
        }
    }
}
