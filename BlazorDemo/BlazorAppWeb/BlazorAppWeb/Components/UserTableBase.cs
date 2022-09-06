using BlazorApp.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppWeb.Components
{
    public class UserTableBase:ComponentBase
    {
        [Parameter]
        public int Count { get; set; }
		[Parameter]
		public List<UserInfo> UserInfos { get; set; } = new List<UserInfo>();

		[Inject]
		public IJSRuntime Js { get; set; }


		[Parameter]
		public EventCallback<int> OnDeleted { get; set; }

		public async Task Delete(int id)
		{
			var user = UserInfos.FirstOrDefault(p => p.UserID.Equals(id));
			// 调用js的方法
			var confirmed = await Js.InvokeAsync<bool>("confirm", $"确定要删除用户 {user.UserName}?");
			if (confirmed)
			{
				await OnDeleted.InvokeAsync(id);
			}

		}
	}
}
