using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Android.Gms.Extensions;
using Firebase.Auth;
using SestoApp.Droid;
using SestoApp.Interfaces;
using SestoApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Firebase.Auth.FirebaseAuth;

[assembly: Dependency(typeof(FirebaseAuthentication))]

namespace SestoApp.Droid
{

    public class FirebaseAuthentication : IFirebaseAuthentication //, IIdTokenListener
    {

        public FirebaseAuthentication()
        {
            // FirebaseAuth.Instance.IdToken += Instance_IdToken;

        }

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
            var user = Firebase.Auth.FirebaseAuth.Instance.CurrentUser;
            return user != null;
        }

        public async Task<string> LoginWithEmailAndPassword(string email, string password)
        {
            try
            {
                var user = await Firebase.Auth.FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
                var token = await user.User.GetIdToken(false);
                await SaveAuthToken(token.ToString());
                await FirebaseAuth.Instance.CurrentUser.ReloadAsync();


                return token.ToString();
            }
            catch (FirebaseAuthInvalidUserException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
            catch (FirebaseAuthInvalidCredentialsException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
        }

        public async Task<bool> SignOut()
        {
            try
            {
                Firebase.Auth.FirebaseAuth.Instance.SignOut();
                try
                {
                    await SecureStorage.SetAsync("oauth_token", string.Empty);
                }
                catch (Exception ex)
                {
                    Preferences.Set("SestoAppFirebaseToken", string.Empty);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<string> LoginAnonymously()
        {
            try
            {
                var user = await Firebase.Auth.FirebaseAuth.Instance.SignInAnonymouslyAsync();
                var token = await user.User.GetIdToken(true);
                await SaveAuthToken(token.ToString());
                await FirebaseAuth.Instance.CurrentUser.ReloadAsync();

                return token.ToString();
            }
            catch (FirebaseAuthInvalidUserException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
            catch (FirebaseAuthInvalidCredentialsException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
        }

        async void Instance_IdTokenCustom(object sender, IdTokenEventArgs e)
        {
            var token = e.Auth.GetAccessToken(true);
            await SaveAuthToken(token.ToString());

        }



        public async Task RefreshAuthToken(bool force = false)
        {
            if (IsSignedIn())
            {

                try
                {
                    //TODO make this look for the expiry time and refresh if needed.
                    var tokenResult = await (FirebaseAuth.Instance.CurrentUser.GetIdToken(force).AsAsync<GetTokenResult>());
                    //// We have already minted a token for this session, so check if we need a new one 
                    if (tokenResult != null)
                    {
                        // Check to see if current token is expired or will expire soon
                        var idTokenExpiration = DateTimeOffset.FromUnixTimeSeconds(tokenResult.ExpirationTimestamp);
                        if (idTokenExpiration <= DateTime.Now.AddMinutes(-5))
                        {
                            tokenResult = await (FirebaseAuth.Instance.CurrentUser.GetIdToken(true).AsAsync<GetTokenResult>());
                        }
                    }
                    await SaveAuthToken(tokenResult.Token.ToString());

                }
                catch
                {
                    await SignOut();
                }
            }

        }

        private async void Instance_IdToken(object sender, IdTokenEventArgs e)
        {
            var token = e.Auth.GetAccessToken(true);
            await SaveAuthToken(token.ToString());
        }


        // TODO: Find exception throw here.
        public async Task<string> CreateUserAndSignIn(string email, string password)
        {
            try
            {
                var user = await Instance.CreateUserWithEmailAndPasswordAsync(email: email, password: password);
                var token = await LoginWithEmailAndPassword(email, password);

                await SaveAuthToken(token.ToString());


                return token.ToString();
            }
            catch (FirebaseAuthInvalidUserException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
            catch (FirebaseAuthInvalidCredentialsException e)
            {
                e.PrintStackTrace();
                return string.Empty;
            }
        }

        public async Task<Profile> GetProfile()
        {
            //await Firebase.Auth.FirebaseAuth.Instance.CurrentUser.ReloadAsync();
            var user = Firebase.Auth.FirebaseAuth.Instance.CurrentUser;

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
            try
            {
                if (!string.IsNullOrWhiteSpace(email))
                {
                    Instance.SendPasswordResetEmailAsync(email);
                    return;
                }

                var user = Firebase.Auth.FirebaseAuth.Instance.CurrentUser;
                if (user != null)
                {
                    Instance.SendPasswordResetEmailAsync(user.Email);
                }
            }
            catch (Exception Ex)
            {
                Debug.WriteLine(Ex);
            }
        }

        public async Task UpdateFirebaseDisplayName(string displayName)
        {
            FirebaseUser user = FirebaseAuth.Instance.CurrentUser;
            if (user != null)
            {
                UserProfileChangeRequest profileUpdates = new UserProfileChangeRequest.Builder().SetDisplayName(displayName).Build();
                await user.UpdateProfileAsync(userProfileChangeRequest: profileUpdates);
            }
        }


        public async Task ConvertToEmailAndPasswordAccount(string email, string password)
        {

            FirebaseUser user = FirebaseAuth.Instance.CurrentUser;
            if (user != null && !string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(email))
            {
                try
                {
                    AuthCredential credential = EmailAuthProvider.GetCredential(email, password);
                    await user.LinkWithCredentialAsync(credential);
                }

                catch (FirebaseAuthInvalidUserException e)
                {
                    e.PrintStackTrace();
                    // return string.Empty;
                }
                catch (FirebaseAuthInvalidCredentialsException e)
                {
                    e.PrintStackTrace();
                    //return string.Empty;
                }
                catch (FirebaseAuthEmailException e)
                {
                    e.PrintStackTrace();
                }

            }
        }
    }
}
