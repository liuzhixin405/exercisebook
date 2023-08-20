using EntityEF.Dto;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbsEFWork.Implementations.Dto
{
    public class ResponseProductDto:IResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public double CreateTime { get; set; }
    }
}
