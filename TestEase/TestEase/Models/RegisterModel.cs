using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEase.Models
{
    public class RegisterModel
    {
        public int Address { get; set; }
        public int Value {  get; set; }
        public RegisterType Type { get; set; }
        public string? Name { get; set; }
    }
}
