public class CustomMiddleware
{
    private readonly RequestDelegate _next;
    public CustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        Console.WriteLine($"middleware start, {DateTime.Now.ToString()}");

        await _next.Invoke(context);

        Console.WriteLine($"middleware end, {DateTime.Now.ToString()}");
    }
}