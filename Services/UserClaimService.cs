using System.Security.Claims;

namespace MyBookShelf.Services
{
    public class UserClaimService : IUserClaimService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserClaimService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserInfo GetUserInfo()
        {
            UserInfo userInfo = new()
            {
                UserId = int.Parse(GetClaimValue(ClaimTypes.NameIdentifier)),
                UserName = GetClaimValue(ClaimTypes.Name)
            };
            return userInfo;
        }        

        private string GetClaimValue(string claimType)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.Identity?.IsAuthenticated == true
                ? user?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value ?? "Desconhecido"
                : "Desconhecido";
        }
    }

    public class UserInfo
    {
        public int UserId { get; set; }
        public required string UserName { get; set; }
    }

    public interface IUserClaimService
    {
        UserInfo GetUserInfo();        
    }
}
