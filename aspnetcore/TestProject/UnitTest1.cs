using System;
using System.Net.Http;
using Xunit;

namespace TestProject
{
    public class UnitTest1
    {
        [Fact]
        public async void Test1()
        {
           var response = await new HttpClient().GetAsync("https://localhost:5001/home/getvalue");
            var result =  await response.Content.ReadAsStringAsync();
            Assert.Equal("OK",result);
        }
        [Fact]
        public async void Test2()
        {
            var response = await new HttpClient().GetAsync("https://localhost:5001/home/getvaluet");
            var result = await response.Content.ReadAsStringAsync();
            Assert.Equal("OK", result);
        }
    }
}
