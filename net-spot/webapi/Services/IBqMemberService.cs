namespace webapi.Services
{
    public interface IBqMemberService
    {
        Task<BqMember> FindByemail(string emiEmail);
        Task<List<BqMember>> FindListByemail(string emiEmail);
    }
}