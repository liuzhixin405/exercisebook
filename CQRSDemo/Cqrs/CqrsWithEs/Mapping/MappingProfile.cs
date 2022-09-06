using AutoMapper;
using CqrsWithEs.Commands;
using CqrsWithEs.Domain.Offer;

namespace CqrsWithEs.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Offer,OfferVm>().ReverseMap();
        }
    }
}
