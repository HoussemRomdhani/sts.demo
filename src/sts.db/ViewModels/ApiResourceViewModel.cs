using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace identity.ViewModels
{
    public class ApiResourceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Scopes { get; set; }
    }
}
