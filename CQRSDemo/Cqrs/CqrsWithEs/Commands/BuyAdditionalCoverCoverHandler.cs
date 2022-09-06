using CqrsWithEs.Domain.Policy;
using MediatR;

namespace CqrsWithEs.Commands
{
    public class BuyAdditionalCoverCoverHandler : IRequestHandler<BuyAdditionalCoverCommand, BuyAdditionalCoverCoverResult>
    {
        private readonly IPolicyRepository policyRepository;
        public BuyAdditionalCoverCoverHandler(IPolicyRepository policyRepository)
        {
            this.policyRepository = policyRepository;
        }
        public Task<BuyAdditionalCoverCoverResult> Handle(BuyAdditionalCoverCommand request, CancellationToken cancellationToken)
        {
            var policy = policyRepository.GetById(request.PolicyId);
            policy.ExtendCoverage(
                request.EffectiveDateOfChange,
                new Domain.Offer.CoverPrice(request.NewCoverCode, request.NewCoverPrice, request.NewCoverPriceUnit));
            policyRepository.Save(policy, policy.Version);
            return Task.FromResult(new BuyAdditionalCoverCoverResult { PolicyNumber = policy.PolicyNumber,
            VersionWithAdditionalCoversNumber = policy.Versions.Last().VersionNumber});
        }
    }
}
