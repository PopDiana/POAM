using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace POAM.Models
{
    public partial class Owner
    {
        public Owner()
        {
            Apartment = new HashSet<Apartment>();
        }

        public int IdOwner { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.])[A-Za-z\d@$!%*?&.]{6,}$",
         ErrorMessage = "Password should be at least one uppercase character, " +
            "one lowercase character, one non-character and " +
            "one digit and its length should be at least 6 characters.")]
        public string Password { get; set; }
      
        public string FullName { get; set; }

        public string Telephone { get; set; }

        
        public string Email { get; set; }
        public bool? IsAdmin { get; set; }
        public string PassSalt { get; set; }

        public ICollection<Apartment> Apartment { get; set; }
    }
}
