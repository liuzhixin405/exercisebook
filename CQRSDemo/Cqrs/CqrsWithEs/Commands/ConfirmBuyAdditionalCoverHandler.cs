using CqrsWithEs.Domain.Policy;
using MediatR;

namespace CqrsWithEs.Commands
{
    public class ConfirmBuyAdditionalCoverHandler : IRequestHandler<ConfirmBuyAdditionalCoverCommand, ConfirmBuyAdditionalCoverResult>
    {
        private readonly IPolicyRepository policyRepository;
        public ConfirmBuyAdditionalCoverHandler(IPolicyRepository policyRepository)
        {
            this.policyRepository = policyRepository;
        }
        public Task<ConfirmBuyAdditionalCoverResult> Handle(ConfirmBuyAdditionalCoverCommand request, CancellationToken cancellationToken)
        {
            var policy = policyRepository.GetById(request.PolicyId);
            policy.ConfirmCoverageExtension();
            policyRepository.Save(policy,policy.Version);
            return Task.FromResult(new ConfirmBuyAdditionalCoverResult
            {
                PolicyId = request.PolicyId,
                VersionConfirmed = policy.Versions.LastActive().VersionNumber
            }) ;
        }
    }
}
