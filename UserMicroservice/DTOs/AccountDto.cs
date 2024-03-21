using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserMicroservice.Entities;

namespace UserMicroservice.DTOs
{
    public class AccountDto
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public long Id { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}