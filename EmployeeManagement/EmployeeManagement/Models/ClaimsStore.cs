using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    //static class
    public static class ClaimsStore
    {
        //static prop
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("Create Role","Create Role" ),
            new Claim("Edit Role","Create Role" ),
            new Claim("Delete Role","Create Role" )
        };
    }
}
