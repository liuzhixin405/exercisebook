using CqrsWithEs.Domain.Common;
using CqrsWithEs.Domain.Offer;
using MediatR;
using NodaMoney;
using System.Collections.ObjectModel;

namespace CqrsWithEs.Commands
{
    public class OfferListCommand:IRequest<ReadOnlyCollection<OfferVm>>
    {
    }
    public class OfferVm
    {
        public Guid Id { get; private set; }
        public string Number { get; private set; }
        public OfferStatus Status { get; private set; }
        public string ProductCode { get; private set; }
        public Person Customer { get; private set; }
        public Car Car { get; private set; }
        public Person Driver { get; private set; }
        public TimeSpan CoverPeriod { get; private set; }
        public Money TotalCost { get; private set; }
        public DateTime CreationDate { get; private set; }
        public DateTime ValidityDate => CreationDate.AddDays(30);
    }
}
