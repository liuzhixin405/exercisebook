using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class HttpContextAccessor
    {
        private static readonly AsyncLocal<HttpContextHolder> _httpContextCurrent = new AsyncLocal<HttpContextHolder>();

        public HttpContext? HttpContext
        {
            get
            {
                return _httpContextCurrent.Value?.Context;
            }
            set
            {
                var holder = _httpContextCurrent.Value;
                if (holder != null)
                {
                    _httpContextCurrent.Value = new HttpContextHolder(){ Context = value};
                }
            }
        }

        private class HttpContextHolder
        {
            public HttpContext? Context;
        }
    }


}
