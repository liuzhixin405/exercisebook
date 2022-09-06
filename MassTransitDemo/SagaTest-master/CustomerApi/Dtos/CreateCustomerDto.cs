namespace CustomerApi.Dtos
{
    public class CreateCustomerDto
    {
        public string FullName { get; set; }
        /// <summary>
        /// 信用
        /// </summary>
        public decimal Credit { get; set; }
    }
}
