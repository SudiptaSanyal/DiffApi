using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiffApi.Models
{
    public class EndpointInput
    {
        [Required]
        public int ID { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
    }
}