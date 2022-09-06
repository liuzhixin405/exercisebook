using BlazorApp.Shared;
using BlazorAppWeb.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppWeb.Pages
{
    public class UserInfoPageBase:ComponentBase
    {
        public List<UserInfo> UserInfos = new List<UserInfo>();
        public MetaData MetaData { get; set; } = new MetaData();
        private readonly UserParameters userParameters = new UserParameters();

        [Inject]
        public IUserHttpRepository UserHttpRepository { get; set; }
        protected async override Task OnInitializedAsync()
        {
            await GetUsers();
        }
        public async Task GetUsers()
        {
            var pagingResponse = await UserHttpRepository.GetUserInfos(userParameters);
            UserInfos = pagingResponse.Items;
            MetaData = pagingResponse.MetaData;
        }
        public async Task SelectedPage(int page)
        {
            userParameters.PageNumber = page;
            await GetUsers();
        }
        public async Task SearchChanged(string searchTerm)
        {
            Console.WriteLine(searchTerm);
            userParameters.PageNumber = 1;
            userParameters.SearchTerm = searchTerm;
            await GetUsers();
        }
        public async Task DeleteUser(int id)
        {
            await UserHttpRepository.DeleteUser(id.ToString());
            await GetUsers();
        }
        public string Result { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        public async Task SayHello()
        {
            Result = await JsRuntime.InvokeAsync<string>("sayHello", "Test-Blazor");
        }
    }
}
