using BlazorApp.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppWeb.Components
{
    public partial class Pagination
    {
        [Parameter]
        public MetaData MetaData { get; set; }

        [Parameter]
        public int Spread { get; set; }
        [Parameter]
        public EventCallback<int> SelectedPage { get; set; }
        public List<PageLink> _links;
        protected override void OnParametersSet()
        {
            CreatePaginationLinks();
        }

        private void CreatePaginationLinks()
        {
            _links = new List<PageLink> { new PageLink(MetaData.CurrentPage - 1, MetaData.HasPrevious, "上一页") };
            for (int i = 1; i < MetaData.TotalPages; i++)
            {
                if (i >= MetaData.CurrentPage - Spread && i <= MetaData.CurrentPage + Spread)
                {
                    _links.Add(new PageLink(i, true, i.ToString()){ Active = MetaData.CurrentPage == i });
                }
            }
            _links.Add(new PageLink(MetaData.CurrentPage+1,MetaData.HasNext,"下一页"));;
        }
        private async Task OnSelectedPage(PageLink pageLink)
        {
            if (pageLink.Page == MetaData.CurrentPage || !pageLink.Enabled)
                return;
            MetaData.CurrentPage = pageLink.Page;
            await SelectedPage.InvokeAsync(pageLink.Page);
        }
    }
}
