using AutoMapper;
using CqrsWithEs.Domain.Offer;
using CqrsWithEs.Domain.Policy;
using MediatR;
using System.Collections.ObjectModel;

namespace CqrsWithEs.Commands
{
    public class OfferListHandler : IRequestHandler<OfferListCommand, ReadOnlyCollection<OfferVm>>
    {
        private readonly IOfferRepository offerRepository;
        private readonly IMapper mapper;
        public OfferListHandler(IOfferRepository offerRepository,IMapper mapper)
        {
            this.offerRepository = offerRepository;
            this.mapper = mapper;
        }
        public async Task<ReadOnlyCollection<OfferVm>> Handle(OfferListCommand request, CancellationToken cancellationToken)
        {
            var res = await offerRepository.All();
           return this.mapper.Map<ReadOnlyCollection<OfferVm>>(res);
        }
    }
}