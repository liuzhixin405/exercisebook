using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Shared
{
    public class PageLink
    {
        public string Text { get; set; }
        public int Page { get; set; }
        public bool Enabled { get; set; }
        public bool Active { get; set; }

        public PageLink(int page,bool enabled,string text)
        {
            Text = text;
            Page = page;
            Enabled = enabled;

        }
    }
}
