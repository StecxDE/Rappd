using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rappd.CQRS.AspNet.Sample.Requests;

namespace Rappd.CQRS.AspNet.Sample.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Message { get; set; }

        public async Task OnGetAsync()
            => Message = await GetMessage.SendAsync();
        public async Task OnPostAsync()
            => await SetMessage.SendAsync(Message);
    }
}