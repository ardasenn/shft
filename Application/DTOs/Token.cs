using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class Token
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
