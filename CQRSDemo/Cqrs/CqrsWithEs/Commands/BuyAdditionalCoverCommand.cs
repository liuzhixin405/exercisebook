using MediatR;

namespace CqrsWithEs.Commands
{
    public class BuyAdditionalCoverCommand:IRequest<BuyAdditionalCoverCoverResult>
    {
        public Guid PolicyId { get; set; }
        public DateTime EffectiveDateOfChange { get; set; }
        public string NewCoverCode { get; set; }
        public decimal NewCoverPrice { get; set; }
        public TimeSpan NewCoverPriceUnit { get; set; }
    }

    public class BuyAdditionalCoverCoverResult
    {
        public string PolicyNumber { get; set; }
        public int VersionWithAdditionalCoversNumber { get; set; }
    }
}
