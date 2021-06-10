using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace sesto.api.Infastructure.Data
{
    public class User
    {
        [Key]
        public string FirebaseId { get; set; }

        [MaxLength(450)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(200)]
        public string Provider { get; set; }

        public bool IsAdmin { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Bookmark> Bookmarks { get; set; }

    }
}
