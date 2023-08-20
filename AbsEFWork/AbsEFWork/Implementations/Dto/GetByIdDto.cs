using EntityEF.Dto;

namespace AbsEFWork.Implementations.Dto
{
    public class GetByIdDto:IRequestDto
    {
        public int Id { get; set; }
    }
}
