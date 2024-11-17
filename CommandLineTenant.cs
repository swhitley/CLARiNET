using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLARiNET
{
    public static class CommandLineTenant
    {
        public static string TenantOption(string tenant)
        {
            // Tenant
            if (String.IsNullOrEmpty(tenant))
            {
                Console.WriteLine("Enter the tenant:\n");
                tenant = Console.ReadLine().Trim();
                Console.WriteLine("");
            }
            return tenant;
        }
    }
}
