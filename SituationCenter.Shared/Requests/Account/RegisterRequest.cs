using System;
using System.Collections.Generic;
using System.Text;

namespace SituationCenter.Shared.Requests.Account
{
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthday { get; set; }
        public Sex Sex { get; set; }

        public override string ToString()
        {
            return $"email >{Email}<  pass >{Password}<  name >{Name}<  surname >{Surname}< sex >{Sex}< phone >{PhoneNumber}< birthday >{Birthday}<";
        }
    }
}
