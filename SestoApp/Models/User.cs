using System;
using System.Collections.Generic;

namespace SestoApp.Models
{
    public class User
    {
        public string FirebaseId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Provider { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime CreatedAt { get; set; }

        // public virtual ICollection<Bookmark> Bookmarks { get; set; }

    }
}
