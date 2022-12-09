using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorDemo.Models;
namespace RazorDemo.Pages.Customers;
public class IndexModel : PageModel
{
    private readonly RazorDemo.Data.CustomerDbContext _context;

    public IndexModel(RazorDemo.Data.CustomerDbContext context)
    {
        _context = context;
    }

    public IList<Customer>? Customers{get;set;}

    public async Task OnGetAsync()
    {
        Customers = await _context.Customer.ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var contact = await _context.Customer.FindAsync(id);
        if(contact!=null){
            _context.Customer.Remove(contact);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}