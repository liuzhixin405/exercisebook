using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorAppWeb.Components
{
    public partial class Search
    {
        public string SearchTerm { get; set; }
        [Parameter]
        public EventCallback<string> OnSearchChanged { get; set; }
        private void SearchChanged()
        {
            OnSearchChanged.InvokeAsync(SearchTerm);
        }
    }
}
