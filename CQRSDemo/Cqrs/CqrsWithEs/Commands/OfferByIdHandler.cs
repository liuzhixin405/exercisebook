using AutoMapper;
using CqrsWithEs.Domain.Offer;
using MediatR;
using System.Collections.ObjectModel;

namespace CqrsWithEs.Commands
{
    public class OfferByIdHandler : IRequestHandler<OfferByIdCommand, OfferVm>
    {
        private readonly IOfferRepository offerRepository;
        private readonly IMapper mapper;
        public OfferByIdHandler(IOfferRepository offerRepository, IMapper mapper)
        {
            this.offerRepository = offerRepository;
            this.mapper = mapper;
        }
        public async Task<OfferVm> Handle(OfferByIdCommand request, CancellationToken cancellationToken)
        {
            var res = await offerRepository.GetById(request.Id);
            return this.mapper.Map<OfferVm>(res);
        }
    }
}
