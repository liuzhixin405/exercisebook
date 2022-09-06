using Lx.Project.Unicache;
using Xunit;

namespace Lx.TestProject.Caching
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            bool setValue = UniCacheManager.Set<string>("key1", "hello", 1000);
            bool hasValue = false;
            string val = string.Empty;
            UniCacheManager.Get("key1", out hasValue, out val);
            Assert.Equal("hello", val);
        }


        [Fact]
        public void Test2()
        {
            UniCacheManager.Set<ApplyContext>(typeof(ApplyContext).Name, new ApplyContext { Id = 1, Name = "С��", ApplyState = false, Hours = 8, Description = "��ٶȼ�" }, 1000);
            var result = UniCacheManager.Get<ApplyContext>(typeof(ApplyContext).Name);
            Assert.Equal("С��", result.Name);
        }
    }

    public class ApplyContext
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Hours { get; set; }
        public bool ApplyState { get; set; }


    }
}
