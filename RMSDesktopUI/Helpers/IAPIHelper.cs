using RMSDesktopUI.Models;
using System.Threading.Tasks;

namespace RMSDesktopUI.Helpers
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);
    }
}