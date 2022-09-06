using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;

namespace StandardLibrary.AttributeExt
{
    public abstract class PollyRetryAttribute:Attribute
    {
        public int Order = 0;

        public abstract Action<ISyncPolicy> Do(Action<ISyncPolicy> action);
    }

    public class CustomPollyRetryAttribute : PollyRetryAttribute
    {
        public override Action<ISyncPolicy> Do(Action<ISyncPolicy> action)
        {
            Policy retryPolicy = Policy.Handle<Exception>().Retry(retryCount: 2, (ex, count) => {
                Console.WriteLine($"执行失败! , 重试{count} 次");
                Console.WriteLine($"异常信息: {ex.Message}");
            });

            return s =>
            {
                Policy policy = null;
                if (s != null)
                {
                    policy = Policy.Wrap(s, retryPolicy);
                }
                else
                {
                    policy = retryPolicy;
                }

                action(policy);
            };
        }
    }

    public class CustomPollyFallbackAttribute : PollyRetryAttribute
    {
        public override Action<ISyncPolicy> Do(Action<ISyncPolicy> action)
        {
            var circuitBreadkerPolicy = Policy.Handle<Exception>().CircuitBreaker(
                 exceptionsAllowedBeforeBreaking: 4,             // 连续4次异常
                  durationOfBreak: TimeSpan.FromMilliseconds(10000),       // 断开1分钟
                  onBreak: (exception, breakDelay) =>
                  {
                      Console.WriteLine($"{DateTime.Now}:熔断了。。");
                      Console.WriteLine($"熔断: {breakDelay.TotalMilliseconds } ms, 异常: " + exception.Message);
                  },
                  onReset: () => //// 熔断器关闭时
                  {
                      Console.WriteLine($"{DateTime.Now}:熔断器关闭了。。。");
                  },
                  onHalfOpen: () => // 熔断时间结束时，从断开状态进入半开状态
                  {
                      Console.WriteLine($"{DateTime.Now}:熔断时间到，进入半开状态。。。");
                  });
            return s =>
            {
                Policy policy = null;
                if (s != null)
                {
                    policy = Policy.Wrap(s, circuitBreadkerPolicy);
                }
                else
                {
                    policy = circuitBreadkerPolicy;
                }

                action(policy);
            };
        }
    }
}
