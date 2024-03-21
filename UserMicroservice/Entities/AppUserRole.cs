using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace UserMicroservice.Entities
{
    public class AppUserRole:IdentityUserRole<long>
    {
        public User User { get; set; }
        public AppRole Role { get; set; }
    }
}