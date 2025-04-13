using MediatR;
using System;

namespace spot.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<CreateProductCommandResponse>
    {
        public string BaseCurrency { get; set; }
        public string QuoteCurrency { get; set; }
        public decimal BaseMinSize { get; set; }
        public decimal BaseMaxSize { get; set; }
        public decimal QuoteMinSize { get; set; }
        public decimal QuoteMaxSize { get; set; }
        public int BaseScale { get; set; }
        public int QuoteScale { get; set; }
        public double QuoteIncrement { get; set; }
    }
}