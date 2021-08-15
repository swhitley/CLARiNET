using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace CLARiNET
{
    public class Options
    {
        [Value(index: 0,Required = false, HelpText = "Full path and file name for the CLAR")]
        public string ClarFile { get; set; }

        [Value(index: 1, Required = false, HelpText = "Cloud Collection name")]
        public string CollectionName { get; set; }

        [Value(index: 2, Required = false, HelpText = "Number associated with a Workday environment")]
        public string EnvNum { get; set; }

        [Value(index: 3, Required = false, HelpText = "Workday Tenant")]
        public string Tenant { get; set; }

        [Value(index: 4, Required = false, HelpText = "Username")]
        public string Username { get; set; }

        [Value(index: 5, Required = false, HelpText = "Password")]
        public string Password { get; set; }

        [Option('e', "environments", Required = false,
         HelpText =
             "Display the Workday environments and their associated numbers")]
        public bool PrintEnvironments { get; set; }

    }
}
