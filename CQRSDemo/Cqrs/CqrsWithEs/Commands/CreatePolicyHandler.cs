using CqrsWithEs.Domain.Offer;
using CqrsWithEs.Domain.Policy;
using MediatR;

namespace CqrsWithEs.Commands
{
    public class CreatePolicyHandler : IRequestHandler<CreatePolicyCommand, CreatePolicyResult>
    {
        private readonly IOfferRepository offerRepository;
        private readonly IPolicyRepository policyRepository;
        public CreatePolicyHandler(IOfferRepository offerRepository, IPolicyRepository policyRepository)
        {
            this.policyRepository = policyRepository;
            this.offerRepository = offerRepository;
        }
        public async Task<CreatePolicyResult> Handle(CreatePolicyCommand request, CancellationToken cancellationToken)
        {
            var offer = await offerRepository.WithNumber(request.OfferNumber);
            var policy = Policy.BuyOffer(offer, request.PurchaseDate, request.PolicyStartDate);
            policyRepository.Save(policy, -1);
            return new CreatePolicyResult
            {
                PolicyId = policy.Id,
                PolicyNumber = policy.PolicyNumber
            };
        }
    }
}
