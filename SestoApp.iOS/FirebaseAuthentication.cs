using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Firebase.Auth;
using Foundation;
using SestoApp.Interfaces;
using SestoApp.iOS;
using SestoApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseAuthentication))]

namespace SestoApp.iOS
{


    public class FirebaseAuthentication : IFirebaseAuthentication
    {



        private async Task SaveAuthToken(string token)
        {
            try
            {
                await SecureStorage.SetAsync("oauth_token", token);
            }
            catch (Exception ex)
            {
                Preferences.Set("SestoAppFirebaseToken", token);
            }
        }


        public bool IsSignedIn()
        {
            var user = Auth.DefaultInstance.CurrentUser;
            return user != null;
        }

        public async Task<string> LoginWithEmailAndPassword(string email, string password)
        {
            try
            {
                var user = await Auth.DefaultInstance.SignInWithPasswordAsync(email, password);
                var token = await user.User.GetIdTokenAsync();
                await Auth.DefaultInstance.CurrentUser.ReloadAsync();
                await SaveAuthToken(token);
                return token;
            }
            catch (Foundation.NSErrorException Ex)
            {
                Debug.WriteLine(Ex.Message);
                throw new Exception(Ex.Error.LocalizedDescription);
            }

        }


        public async Task<string> LoginAnonymously()
        {
            var user = await Auth.DefaultInstance.SignInAnonymouslyAsync();
            var token = await user.User.GetIdTokenAsync();
            await Auth.DefaultInstance.CurrentUser.ReloadAsync();
            await SaveAuthToken(token);
            return token;

        }

        public async Task<bool> SignOut()
        {
            try
            {
                _ = Auth.DefaultInstance.SignOut(out NSError error);
                try
                {
                    await SecureStorage.SetAsync("oauth_token", string.Empty);
                }
                catch (Exception ex)
                {
                    Preferences.Set("SestoAppFirebaseToken", string.Empty);
                }

                return error == null;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task RefreshAuthToken(bool force = false)
        {

            if (IsSignedIn())
            {

                //var token = await Auth.DefaultInstance.CurrentUser.GetIdTokenAsync(forceRefresh: force);
                try
                {
                    var tokenResult = await Auth.DefaultInstance.CurrentUser.GetIdTokenResultAsync(force);

                    // We have already minted a token for this session, so check if we need a new one 
                    if (tokenResult != null)
                    {
                        // Check to see if current token is expired or will expire soon
                        var idTokenExpiration = tokenResult.ExpirationDate;
                        if ((DateTime)idTokenExpiration <= DateTime.Now.AddMinutes(-5))
                        {
                            Debug.WriteLine("Force Refreshed");
                            tokenResult = await Auth.DefaultInstance.CurrentUser.GetIdTokenResultAsync(forceRefresh: true);
                        }
                    }
                    await SaveAuthToken(tokenResult.Token);
                }
                catch
                {
                    await SignOut();
                }
            }
        }

        public async Task<string> CreateUserAndSignIn(string email, string password)
        {
            try
            {
                var user = await Auth.DefaultInstance.CreateUserAsync(email, password);

                var token = await LoginWithEmailAndPassword(email, password);
                await SaveAuthToken(token);
                return token;
            }
            catch (Foundation.NSErrorException Ex)
            {
                Debug.WriteLine(Ex.Message);
                throw new Exception(Ex.Error.LocalizedDescription);
            }

        }

        public async Task<Profile> GetProfile()
        {
            //await Auth.DefaultInstance.CurrentUser.ReloadAsync();
            var user = Auth.DefaultInstance.CurrentUser;
            if (user != null)
            {
                return new Profile
                {
                    FirebaseId = user.Uid,
                    IsAnonymous = user.IsAnonymous,
                    IsEmailVerified = user.IsEmailVerified,
                    ProviderId = user.ProviderId,
                    DisplayName = user.DisplayName,
                    Email = user.Email
                };
            }
            return null;
        }


        public void ResetPasswordForUser(string email = null)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                Auth.DefaultInstance.SendPasswordResetAsync(email);
                return;
            }
            var user = Auth.DefaultInstance.CurrentUser;
            if (user != null)
            {
                Auth.DefaultInstance.SendPasswordResetAsync(user.Email);
            }
        }


        public async Task UpdateFirebaseDisplayName(string displayName)
        {
            Firebase.Auth.User user = Auth.DefaultInstance.CurrentUser;
            if (user != null)
            {
                var profileUpdate = user.ProfileChangeRequest();
                profileUpdate.DisplayName = displayName;
                await profileUpdate.CommitChangesAsync();
            }

        }

        public async Task ConvertToEmailAndPasswordAccount(string email, string password)
        {
            Firebase.Auth.User user = Auth.DefaultInstance.CurrentUser;
            if (user != null && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(email))
            {
                try
                {
                    var credential = EmailAuthProvider.GetCredentialFromPassword(email, password);
                    await user.LinkAsync(credential);
                }
                catch (Foundation.NSErrorException Ex)
                {
                    Debug.WriteLine(Ex.Message);
                    throw new Exception(Ex.Error.LocalizedDescription);
                }

            }
        }

    }
}
