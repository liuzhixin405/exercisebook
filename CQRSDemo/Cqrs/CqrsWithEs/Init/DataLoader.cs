﻿using CqrsWithEs.Domain.Offer;

namespace CqrsWithEs.Init
{
    public class DataLoader
    {
        private readonly IOfferRepository offerRepository;
        public DataLoader(IOfferRepository offerRepository)
        {
            this.offerRepository = offerRepository; 
        }
        public async Task Seed()
        {
            var existing = await offerRepository.WithNumber("OFF001");
            if (existing == null)
                offerRepository.Add(SampleOfferData.SampleOffer("OFF001"));
        }
    }
}
