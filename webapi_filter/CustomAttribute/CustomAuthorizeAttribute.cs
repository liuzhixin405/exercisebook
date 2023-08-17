using Microsoft.AspNetCore.Mvc;

public class CustomAuthorizeAttribute : TypeFilterAttribute 
{
    public CustomAuthorizeAttribute() : base(typeof(CustomAuthorizeFilter))
    {
    }
}
