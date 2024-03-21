using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.DTOs
{
    public class AdminDto
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}