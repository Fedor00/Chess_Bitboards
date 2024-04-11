using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Dtos
{
    public class AddMessageDto
    {
        [Required]
        public string GameId { get; set; }
        [Required]
        [StringLength(300, ErrorMessage = "Content cannot be longer than 300 characters.")]
        public string Content { get; set; }
        [Required]

        public long SenderId { get; set; }

    }
}