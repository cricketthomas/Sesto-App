using System;
namespace SestoApp.Resources
{
    public static class Validators
    {

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        public static bool IsValidPassword(string password)
        {
            try
            {
                return password.Length > 5;
            }
            catch
            {
                return false;
            }
        }

    }
}
