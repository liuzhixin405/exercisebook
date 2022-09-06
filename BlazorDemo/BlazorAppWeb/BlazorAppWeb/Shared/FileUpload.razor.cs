using BlazorAppWeb.Service;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Tewr.Blazor.FileReader;

namespace BlazorAppWeb.Shared
{
    public partial class FileUpload
    {
        private ElementReference _input;
        [Inject]
        public IUserHttpRepository UserHttpRepository { get; set; }
        [Parameter]
        public EventCallback<string> OnChange { get; set; }
        [Inject]
        public IFileReaderService FileReaderService { get; set; }
        public List<string> FileDataUrls = new List<string>();

        private async Task HandleSelected()
        {
            foreach (var file in await FileReaderService.CreateReference(_input).EnumerateFilesAsync())
            {
                if(file != null)
                {
                    var fileInfo = await file.ReadFileInfoAsync();
                    await using var ms = await file.CreateMemoryStreamAsync();
                    var content = new MultipartFormDataContent();
                    content.Add(new StreamContent(ms, Convert.ToInt32(ms.Length)), fileInfo.Name, fileInfo.Name);
                    var path = await UserHttpRepository.UploadFile(content);
                    FileDataUrls.Add(path);
                    await OnChange.InvokeAsync(path);

                }
            }
        }

    }
}
