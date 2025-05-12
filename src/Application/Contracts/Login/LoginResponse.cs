using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Login
{
    public class LoginResponse
    { 
        public string Status { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;  
    }
}
