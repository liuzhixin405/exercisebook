using Lx.Project.Business;
using Lx.Project.UniDapper;
using Xunit;

namespace Lx.TestProject.Business;
public class UnitTest1
{
    private readonly DapperRepository _dapperRepository;
    public UnitTest1(DapperRepository dapperRepository)
    {
        _dapperRepository = dapperRepository;
    }
    [Fact]
    public async Task Test1Async()
    {
        //ÆÕÍ¨
        int id = 1;
        await _dapperRepository.ExecuteScalarAsync<int>($"SELECT Id FROM Table WHERE Id='{id}'");
        EmailQueue emailQueue = new EmailQueue();
        await _dapperRepository.UpdateAsync<EmailQueue>(emailQueue);

        //ÊÂÎñ
        var trans = await _dapperRepository.BeginTransaction();

        try
        {
            await _dapperRepository.InsertAsync<EmailQueue>(emailQueue, trans);
            await _dapperRepository.InsertAsync<EmailQueue>(emailQueue, trans);
            trans.Commit();
            trans.Connection?.Close();
            trans.Dispose();
        }
        catch
        {
            trans.Rollback();
            trans.Connection?.Close();
            trans.Dispose();
            throw;
        }
    }
}
public class EmailQueue : BaseEntity
{

}