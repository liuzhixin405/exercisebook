using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorDemo.Models;

namespace RazorDemo.Pages.Customers;

public class CreateModel:PageModel
{
    private readonly Data.CustomerDbContext _context;
    public CreateModel(Data.CustomerDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet(){
        return Page();
    }

    [BindProperty(SupportsGet =true)]
    public Customer? Customer{get;set;}
    
    public async Task<IActionResult> OnPostAsync(){
        if(!ModelState.IsValid){
            return Page();
        }
        if(Customer!=null) _context.Add(Customer);
        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}