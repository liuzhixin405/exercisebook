using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppWeb.Shared
{
    public partial class ImageUpload
    {
        private IList<string> imageDataUrls = new List<string>();
        [Parameter]
        public EventCallback<string[]> OnChange { get; set; }
		// 官网提供的上传文件只是上传图片,没有提供上传其他文件的方式
		private async Task OnInputFileChange(InputFileChangeEventArgs e)
		{
			var maxAllowedFiles = 3;
			var format = "image/png";

			foreach (var imageFile in e.GetMultipleFiles(maxAllowedFiles))
			{

				var resizedImageFile = await imageFile.RequestImageFileAsync(format,
				   100, 100);
				var buffer = new byte[resizedImageFile.Size];
				await resizedImageFile.OpenReadStream().ReadAsync(buffer);
				//直接把base64 数据提交,没有必要去上传文件,
				var imageDataUrl =
					$"data:{format};base64,{Convert.ToBase64String(buffer)}";
				imageDataUrls.Add(imageDataUrl);


			}
			await OnChange.InvokeAsync(imageDataUrls.ToArray());

		}
	}
}
