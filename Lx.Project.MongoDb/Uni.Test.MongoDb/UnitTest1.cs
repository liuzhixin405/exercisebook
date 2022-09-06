using MongoDB.Bson.IO;
using MongoDB.Driver;
using Uni.MongoDb;
using Xunit;

namespace Uni.Test.MongoDb;
public class UnitTest1
{
    [Fact]
    public async Task Test1Async()
    {
        var user = new User
        {
            Name = "md",
            Email = "test@gmail.com",

        };
        UserRepository userRepository = new UserRepository();

        var (total, list) = await userRepository.GetListAsync(c => true, 1, 1000);

        var res = await userRepository.InsertOneAsync(user);

        Assert.Equal(res, user);
    }

}


public class User : BaseEntity
{
    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Json { get; set; }
}

public class UserRepository : MongoRepository<User>
{
    public override MongoClient Client => new MongoClient("mongodb://localhost:27017");
    public override string DbName => "md";
}