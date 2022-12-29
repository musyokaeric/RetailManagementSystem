using RMSDesktopUI.Models;
using System.Threading.Tasks;

namespace RMSDesktopUI.Library.Api
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);
        Task GetLoggedInUserInfo(string token);
    }
}