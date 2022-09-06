using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using TestPerformanceDemo.Models;

namespace TestPerformanceDemo.Controllers
{
    public static class CacheKeys
    {
        public static string Entry { get { return "_Entry"; } }
        public static string CallbackEntry { get { return "_Callback"; } }
        public static string CallbackMessage { get { return "_CallbackMessage"; } }
        public static string Parent { get { return "_Parent"; } }
        public static string Child { get { return "_Child"; } }
        public static string DependentMessage { get { return "_DependentMessage"; } }
        public static string DependentCTS { get { return "_DependentCTS"; } }
        public static string Ticks { get { return "_Ticks"; } }
        public static string CancelMsg { get { return "_CancelMsg"; } }
        public static string CancelTokenSource { get { return "_CancelTokenSource"; } }
    }

    public class MyMemoryCache
    {
        public MemoryCache Cache { get; set; }

        public MyMemoryCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }
    }
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMemoryCache _cache;
        private MemoryCache _myCache;
        private static readonly string MyKey = "_MyKey";

        private readonly IDistributedCache _distributedCache;
        public HomeController(ILogger<HomeController> logger,IMemoryCache cache,MyMemoryCache myCache,IDistributedCache distributedCache)
        {
            _logger = logger;
            _cache = cache;
            _myCache = myCache.Cache;
            _distributedCache = distributedCache;
        }

        public string CachedTimeUTC { get; set; }
        
        public async Task OnGetAsync()
        {
            CachedTimeUTC = "Cached Time Expired";

            var encodedCachedTimeUTC = await _distributedCache.GetAsync("cachedTimeUTC");
            if (encodedCachedTimeUTC != null)
            {
                CachedTimeUTC = Encoding.UTF8.GetString(encodedCachedTimeUTC);
            }
        }

        public ContentResult Test()
        {
            return Content("test");
        }

        public async Task TestTask()
        {
            CachedTimeUTC = "Cached Time Expired";

            var encodedCachedTimeUTC = await _distributedCache.GetAsync("cachedTimeUTC");
            if (encodedCachedTimeUTC != null)
            {
                CachedTimeUTC = Encoding.UTF8.GetString(encodedCachedTimeUTC);
            }
            await HttpContext.Response.WriteAsync(CachedTimeUTC);
        }  
        
        public string CacheTest { get; set; }
        public async Task GetTestCache()
        {
            CacheTest = "接收缓存数据";

            var getTest = await _distributedCache.GetAsync("cachedTest");
            if (getTest!=null)
            {
                CacheTest = Encoding.UTF8.GetString(getTest);
            }
            HttpContext.Response.ContentType = "text/json";
            await HttpContext.Response.WriteAsync(CacheTest);
        }
        public async Task<IActionResult> OnPostResetCachedTime()
        {
            var currentTimeUTC = DateTime.UtcNow.ToString();
            byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(currentTimeUTC);
            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(20));
            await _distributedCache.SetAsync("cachedTimeUTC", encodedCurrentTimeUTC, options);
            return View("./Index");
        }
        [TempData]
        public string DatetTime_Now { get; set; }

   
        public IActionResult OnGet()
        {
            if(_myCache.TryGetValue(MyKey,out string cacheEntry))
            {
                cacheEntry = DateTime.Now.TimeOfDay.ToString();

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1).SetSlidingExpiration(TimeSpan.FromSeconds(3));

                _cache.Set(MyKey, cacheEntry, cacheEntryOptions);
            }
            DatetTime_Now = cacheEntry;
            return RedirectToAction("./Index");
        }

        public IActionResult CreateDependentEntries()
        {
            var cts = new CancellationTokenSource();

            _cache.Set(CacheKeys.DependentCTS, cts);
            using(var entry = _cache.CreateEntry(CacheKeys.Parent))
            {
                entry.Value = DateTime.Now;
                entry.RegisterPostEvictionCallback(DependentEvictionCallback, this);
                _cache.Set(CacheKeys.Child, DateTime.Now, new CancellationChangeToken(cts.Token));
            }
            return RedirectToAction("GetDependentEntries");
        }

        public IActionResult GetDependentEntries()
        {
            return View("Dependent", new DependentViewModel {
                ParentCachedTime = _cache.Get<DateTime?>(CacheKeys.Parent),
                ChildCachedTime = _cache.Get<DateTime?>(CacheKeys.Child),
                Message = _cache.Get<string>(CacheKeys.DependentMessage)
            });
        }

        public IActionResult RemoveChildEntry()
        {
            _cache.Get<CancellationTokenSource>(CacheKeys.DependentCTS).Cancel();
            return RedirectToAction("GetDependentEntries");
        }
        private void DependentEvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            var message = $"Parent entry was evicted. Reason: {reason}.";
            ((HomeController)state)._cache.Set(CacheKeys.DependentMessage, message);
        }

        public IActionResult CacheAutoExpiringTryGetValueGet()
        {
            DateTime cacheEntry;

            if(!_cache.TryGetValue(CacheKeys.Entry,out cacheEntry))
            {
                cacheEntry = DateTime.Now;
            }
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var cacheEntryOptions = new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(cts.Token));

            _cache.Set(CacheKeys.Entry, cacheEntry, cacheEntryOptions);

            return View("Cache", cacheEntry);
        }
        public IActionResult CacheTryGetValueSet()
        {
            DateTime cacheEntry;

            if(!_cache.TryGetValue(CacheKeys.Entry,out cacheEntry))
            {
                cacheEntry = DateTime.Now;

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(3));
                _cache.Set(CacheKeys.Entry, cacheEntry, cacheEntryOptions);
            }
            return View("Cache", cacheEntry);
        }

        public async Task<IActionResult> CacheGetOrCreate()
        {
            var cacheEntry = await _cache.GetOrCreateAsync<DateTime>(CacheKeys.Entry, entry => {

                entry.SlidingExpiration = TimeSpan.FromSeconds(3);
               
                  return Task.FromResult(DateTime.Now);
               
            });
            return View("Cache", cacheEntry);
        }

        public IActionResult CahceGet()
        {
            var cacheEentry = _cache.Get<DateTime>(CacheKeys.Entry);
            return View("Cache", cacheEentry);
        }
        public IActionResult CreateCallbackEntry()
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove)
                .RegisterPostEvictionCallback(callback: EvictionCallback, state: this);
            _cache.Set(CacheKeys.CallbackEntry, DateTime.Now, cacheEntryOptions);

            return RedirectToAction("GetCallbackEntry");
        }
        public IActionResult GetCallbackEntry()
        {
            return View("Callback", new CallbackViewModel
            {
                CachedTime = _cache.Get<DateTime>(CacheKeys.CallbackEntry),
                Message = _cache.Get<string>(CacheKeys.CallbackMessage)
            });
        }
        public IActionResult RemoveCallbackEntry()
        {
            _cache.Remove(CacheKeys.CallbackEntry);
            return RedirectToAction("GetCallbackEntry");
        }
        private void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            var message = $"Entry waas evicted. Reason: {reason}.";
            ((HomeController)state)._cache.Set(CacheKeys.CallbackMessage, message);
        }

        public async Task<IActionResult> CacheGetOrCreateAbs()
        {
            var cacheEntry =await _cache.GetOrCreate(CacheKeys.Entry, entry => {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                return Task.FromResult(DateTime.Now);
            });
            return View("Cache", cacheEntry);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<int> GetHttpClient(string url)
        {
            using(var httpClient = new HttpClient())
            {
                var result = await httpClient.GetAsync(url);
                return (int)result.StatusCode;
            }
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "Default30")]
        public ActionResult<string> GetBigString()
        {
            return new string('x', 10 * 1024);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    internal class DependentViewModel
    {
        public DateTime? ParentCachedTime { get; set; }
        public DateTime? ChildCachedTime { get; set; }
        public string Message { get; set; }
    }

    internal class CallbackViewModel
    {
        public DateTime CachedTime { get; set; }
        public string Message { get; set; }
    }
}
