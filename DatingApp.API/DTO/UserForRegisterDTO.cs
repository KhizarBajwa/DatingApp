using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.DTO
{
    public class UserForRegisterDto
    {
        [Required]
        [StringLength(50,MinimumLength = 3,ErrorMessage = "* Username must be between 3 and 50 character in length.")]
        public string Username { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "* Password must be between 3 and 50 character in length.")]
        public string Password { get; set; }        
    }
}
