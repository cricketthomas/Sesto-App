using System;
namespace SestoApp.Models
{
    public class Profile
    {
        public string FirebaseId { get; set; }
        /// <summary>
        /// This is the auth provider, anonymous or generic email and password auth. 
        /// </summary>
        public string ProviderId { get; set; }
#nullable enable
        public string? Email { get; set; }

        public bool IsAnonymous { get; set; }
        public bool IsEmailVerified { get; set; }
        public string DisplayName { get; set; }


    }
}
