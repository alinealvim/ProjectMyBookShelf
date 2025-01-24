using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MyBookShelf.Components.Pages.Account
{
    public partial class Logout
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var httpContext = HttpContextAccessor.HttpContext;
            if (httpContext!.User.Identity!.IsAuthenticated)
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                navigationManager.NavigateTo("/", forceLoad: true, replace: true);
            }
        }
    }
}