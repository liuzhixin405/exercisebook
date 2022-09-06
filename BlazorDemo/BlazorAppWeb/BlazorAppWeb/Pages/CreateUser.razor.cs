using BlazorApp.Shared;
using BlazorAppWeb.Service;
using BlazorAppWeb.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppWeb.Pages
{
    public partial class CreateUser
    {
        [Inject]
        public IUserHttpRepository UserHttpRepository { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Parameter]
        public string DepartmentId { get; set; }
        private SuccessNotification _notification;
        [Parameter]
        public int UserId { get; set; }
        public int DeptId { get; set; }

        public UserInfo UserInfo { get; set; } = new UserInfo();
        public List<DeptInfo> DeptInfos { get; set; } = new List<DeptInfo>();

        public string Message { get; set; }
        public bool Saved { get; set; }
        public string CssClass { get; set; }


        protected override async Task OnInitializedAsync()
        {
            if (UserId != 0)
            {
                UserInfo = await UserHttpRepository.GetUserInfoById(UserId);
                DeptId = UserInfo.DeptId;
            }
            DeptInfos = await UserHttpRepository.GetDeptInfos();
        }
        public async Task HandleValidSubmit()
        {
            if (UserId > 0)
            {
                await UserHttpRepository.UpdateUser(UserInfo);
                Saved = true;
                Message = "修改成功";
                CssClass = "alert alert-success";
            }
            else
            {
                var userInfo = await UserHttpRepository.AddUserinfo(UserInfo);
                if (userInfo != null)
                {
                    _notification.Show();
                }
                else
                {
                    Saved = false;
                    Message = "新增失败";
                    CssClass = "alert alert-danger";
                }
            }
        }

        public void HandleInvalidSubmit()
        {
            CssClass = "alert alert-danger";
            Message = "表单验证失败";
        }
        [Inject]
        public IJSRuntime Js { get; set; }

        private async Task<bool> DeleteEmployee()
        {
            var confirmed = await Js.InvokeAsync<bool>("confirm", $"确定要删除用户?");
            if (confirmed)
            {
                if (UserId != 0)
                {
                    await UserHttpRepository.DeleteUser(UserId.ToString());
                }
                Saved = true;
                Message = "删除成功";
                CssClass = "alert alert-success";
            }
            return false;
        }
        private void GoBack()
        {
            NavigationManager.NavigateTo("/userspage");
        }
        private void AssignImageUrl(string[] imgUrls)
        {

        }

        private void AssignFileUrl(string path)
        {

        }
    }
}
