using BlazorApp.Shared;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorAppWeb.Service
{
    public class UserHttpRepository : IUserHttpRepository
    {
        private readonly HttpClient _client;
        public UserHttpRepository(HttpClient httpClient)
        {
            _client = httpClient;
        }
        public async Task<UserInfo> AddUserinfo(UserInfo userinfo)
        {
            var userInfoJson = new StringContent(JsonSerializer.Serialize(userinfo), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("user/AddUser", userInfoJson);
            if (response.IsSuccessStatusCode)
            {
                return await JsonSerializer.DeserializeAsync<UserInfo>(await response.Content.ReadAsStreamAsync());
            }
            return null;
        }

        public Task<bool> DeleteUser(string userid)
        {
            throw new NotImplementedException();
        }

        public async Task<List<DeptInfo>> GetDeptInfos()
        {
            var response = await _client.GetAsync("dept/GetDeptInfos");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<DeptInfo>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        }

        public Task<UserInfo> GetUserInfoById(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserInfo>> GetUserInfos()
        {
            var response = await _client.GetAsync("user/GetAll");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<UserInfo>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<PagingResponse<UserInfo>> GetUserInfos(UserParameters userParameters)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = userParameters.PageNumber.ToString(),
                ["searchTerm"] = userParameters.SearchTerm ?? ""

            };
            var response = await _client.GetAsync(QueryHelpers.AddQueryString("user/GetPage", queryStringParam));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var pagingResponse = new PagingResponse<UserInfo>
            {
                Items = JsonSerializer.Deserialize<List<UserInfo>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            };
            return pagingResponse;
        }

        public async Task<UserInfo> UpdateUser(UserInfo userInfo)
        {
            var userinfoJson =
                          new StringContent(
                              JsonSerializer.Serialize(userInfo),
                              Encoding.UTF8,
                              "application/json");

            var response = await _client.PostAsync(
                "user/UpdateUser", userinfoJson);
            if (response.IsSuccessStatusCode)
            {
                return await JsonSerializer.DeserializeAsync<UserInfo>
                    (await response.Content.ReadAsStreamAsync());
            }

            return null;
        }

        public async Task<string> UploadFile(MultipartFormDataContent content)
        {
            var postResult = await _client.PostAsync("upload", content);
            var postContent = await postResult.Content.ReadAsStringAsync();
            if (!postResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(postContent);
            }
            else
            {
                var path = postContent;
                return path;
            }
        }
    }
}
