using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DiffApi.Models
{
    public class DiffResponseWithoutLocation
    {
        [Required]
        public string diffResultType { get; set; }
    }
}