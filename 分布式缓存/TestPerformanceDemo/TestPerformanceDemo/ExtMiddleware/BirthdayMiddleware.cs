using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace TestPerformanceDemo.ExtMiddleware
{
    public class BirthdayMiddleware
    {
        private readonly RequestDelegate _next;

        public BirthdayMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        /// <summary>
        /// https://localhost:44357/home/index?firstName=liu&lastName=zhixin&month=12&day=1
        /// </summary>
        /// <param name="context"></param>
        /// <param name="builderPool"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, ObjectPool<StringBuilder> builderPool)
        {
            if(context.Request.Query.TryGetValue("firstName",out var firstName) &&
            context.Request.Query.TryGetValue("lastName", out var lastName) &&
            context.Request.Query.TryGetValue("month", out var month) &&
            context.Request.Query.TryGetValue("day", out var day) &&
            int.TryParse(month, out var monthOfYear) &&
            int.TryParse(day, out var dayOfMonth))
            {
                var now = DateTime.UtcNow;
                var stringBuilder = builderPool.Get();

                try
                {
                    stringBuilder.Append("Hi ")
                    .Append(firstName).Append(" ").Append(lastName).Append(". ");
                    var encoder = context.RequestServices.GetRequiredService<HtmlEncoder>();
                    if (now.Day == dayOfMonth && now.Month == monthOfYear)
                    {
                        stringBuilder.Append("Happy birthday!!!");

                        var html = encoder.Encode(stringBuilder.ToString());
                        await context.Response.WriteAsync(html);

                    }
                    else
                    {
                        var thisYearsBirthday = new DateTime(now.Year, monthOfYear,
                                                                   dayOfMonth);

                        int daysUntilBirthday = thisYearsBirthday > now
                            ? (thisYearsBirthday - now).Days
                            : (thisYearsBirthday.AddYears(1) - now).Days;

                        stringBuilder.Append("There are ")
                            .Append(daysUntilBirthday).Append(" days until your birthday!");

                        var html = encoder.Encode(stringBuilder.ToString());
                        await context.Response.WriteAsync(html);
                    }
                }
                catch(Exception ex)
                {
                    await context.Response.WriteAsync(ex.Message);
                }
                finally // Ensure this runs even if the main code throws.
                {
                    // Return the StringBuilder to the pool.
                    builderPool.Return(stringBuilder);
                }

                return;
            }
            await _next(context);
        }
    }
}
