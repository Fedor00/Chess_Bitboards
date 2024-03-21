using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace UserMicroservice.Entities
{
    public class AppRole:IdentityRole<long>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}