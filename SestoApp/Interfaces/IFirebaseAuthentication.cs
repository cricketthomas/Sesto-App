using System;
using System.Threading.Tasks;
using SestoApp.Models;

namespace SestoApp.Interfaces
{
    public interface IFirebaseAuthentication
    {
        Task<string> LoginWithEmailAndPassword(string email, string password);
        Task<string> LoginAnonymously();
        Task<bool> SignOut();
        Task RefreshAuthToken(bool force);
        bool IsSignedIn();
        Task<string> CreateUserAndSignIn(string email, string password);
        Task<Profile> GetProfile();
        void ResetPasswordForUser(string email = null);
        Task UpdateFirebaseDisplayName(string displayName);
        Task ConvertToEmailAndPasswordAccount(string email, string password);

    }
}
