using System.Collections.ObjectModel;

namespace CqrsWithEs.Domain.Offer
{
    public interface IOfferRepository
    {
        Task<Offer> WithNumber(String number);
        Task<ReadOnlyCollection<Offer>> All();
        void Add(Offer offer);
        Task<Offer> GetById(Guid id);
    }
}
