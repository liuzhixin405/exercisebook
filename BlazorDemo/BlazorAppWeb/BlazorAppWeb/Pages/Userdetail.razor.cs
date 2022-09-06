using BlazorApp.Shared;
using BlazorAppWeb.Service;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppWeb.Pages
{
    public partial class UserDetail
    {
        [Parameter]
        public int UserId { get; set; }
        public UserInfo UserInfo = new UserInfo();
        public IUserHttpRepository UserHttpRepository { get; set; }

        protected async override Task OnInitializedAsync()
        {
            UserInfo = await UserHttpRepository.GetUserInfoById(UserId);
        }
        public void Button_Click()
        {
            UserInfo.UserName = "clay";
        }
    }
}
