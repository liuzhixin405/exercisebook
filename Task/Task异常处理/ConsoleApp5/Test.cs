using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp5
{
    public class Test
    {
        public static AsyncCache<string, string> _cache = new AsyncCache<string, string>(DownLoadPageAsync); 

        private static async Task<string> DownLoadPageAsync(string url)
        {
          var response = await new HttpClient().GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
