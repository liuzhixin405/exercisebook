using BlazorAppWeb.Service;
using BlazorApp.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorAppWeb.Pages
{
	public partial class UserInfoList
	{
		[Inject]
		public HttpClient _client { get; set; }

		[Inject]
		public IUserHttpRepository userHttpRepository { get; set; }

		public List<UserInfo> UserInfos = new List<UserInfo>();

		protected override async Task OnInitializedAsync()
		{
			UserInfos = await userHttpRepository.GetUserInfos();
		}
	}
}
