using System.Security.Claims;

namespace MyBookShelf.Services
{
    /// <summary>
    /// Serviço responsável por obter informações do utilizador autenticado com base nas Claims.
    /// </summary>
    public class UserClaimService : IUserClaimService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Construtor que inicializa o serviço com um acessor do contexto HTTP.
        /// </summary>
        /// <param name="httpContextAccessor">Acessor do contexto HTTP para acessar as informações do utilizador autenticado.</param>
        public UserClaimService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Obtém as informações do utilizador autenticado, incluindo ID e nome.
        /// </summary>
        /// <returns>Um objeto <see cref="UserInfo"/> contendo os dados do utilizador.</returns>
        public UserInfo GetUserInfo()
        {
            UserInfo userInfo = new()
            {
                // Obtém o ID do utilizador a partir da claim "NameIdentifier".
                UserId = int.Parse(GetClaimValue(ClaimTypes.NameIdentifier)),

                // Obtém o nome do utilizador a partir da claim "Name".
                UserName = GetClaimValue(ClaimTypes.Name)
            };
            return userInfo;
        }

        /// <summary>
        /// Recupera o valor de uma claim específica do utilizador autenticado.
        /// </summary>
        /// <param name="claimType">O tipo da claim a ser recuperada.</param>
        /// <returns>O valor da claim, ou "Desconhecido" se a claim não for encontrada ou o utilizador não estiver autenticado.</returns>
        private string GetClaimValue(string claimType)
        {
            var user = _httpContextAccessor.HttpContext?.User;

            // Verifica se o utilizador está autenticado e tenta obter a claim especificada.
            return user?.Identity?.IsAuthenticated == true
                ? user?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value ?? "Desconhecido"
                : "Desconhecido";
        }
    }

    /// <summary>
    /// Representa as informações do utilizador autenticado.
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Identificador único do utilizador.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Nome do utilizador.
        /// </summary>
        public required string UserName { get; set; }
    }

    /// <summary>
    /// Interface que define o contrato para o serviço de informações do utilizador.
    /// </summary>
    public interface IUserClaimService
    {
        /// <summary>
        /// Obtém as informações do utilizador autenticado.
        /// </summary>
        /// <returns>Um objeto <see cref="UserInfo"/> contendo os dados do utilizador.</returns>
        UserInfo GetUserInfo();
    }
}
