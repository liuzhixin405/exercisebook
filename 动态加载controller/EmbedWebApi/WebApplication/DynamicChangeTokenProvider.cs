using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace ControllerWebApplication
{
    public class DynamicChangeTokenProvider : IActionDescriptorChangeProvider
    {
        private CancellationChangeToken _token;
        private CancellationTokenSource _source;
        public DynamicChangeTokenProvider()
        {
            _source = new CancellationTokenSource();
            _token = new CancellationChangeToken(_source.Token);
        }
        public IChangeToken GetChangeToken() => _token;

        public void NotifyChanges()
        {
            var old = Interlocked.Exchange(ref _source, new CancellationTokenSource());
            _token = new CancellationChangeToken(_source.Token);
            old.Cancel();
        }
    }
}
