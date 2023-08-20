using EntityEF.Dto;

namespace AbsEFWork.Implementations.Dto
{
    public class CreateProductDto:IRequestDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
