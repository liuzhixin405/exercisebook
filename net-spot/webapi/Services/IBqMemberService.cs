namespace webapi.Services
{
    public interface IBqMemberService
    {
        BqMember FindByemail(string emiEmail);
        List<BqMember> FindListByemail(string emiEmail);
    }
}