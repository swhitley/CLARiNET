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
        [Value(index: 0,MetaName = "CLAR File", Required = false, HelpText = "Full path and file name for the CLAR")]
        public string ClarFile { get; set; }

        [Value(index: 1, MetaName = "Collection", Required = false, HelpText = "Cloud Collection name")]
        public string CollectionName { get; set; }

        [Value(index: 2, MetaName = "Environment Number", Required = false, HelpText = "Number associated with a Workday environment (list all with -e parameter)")]
        public string EnvNum { get; set; }

        [Value(index: 3, MetaName = "Tenant", Required = false, HelpText = "Workday Tenant")]
        public string Tenant { get; set; }

        [Value(index: 4, MetaName = "Username", Required = false, HelpText = "Username")]
        public string Username { get; set; }

        [Value(index: 5, MetaName = "Password", Required = false, HelpText = "Password (must be encrypted using the -e option)")]
        public string Password { get; set; }

        [Option('e', "encrypt", Required = false,
          HelpText =
              "Encrypt a password for use on the command line")]
        public bool Encrypt { get; set; }

        [Option('w', "wdenvironments", Required = false,
         HelpText =
             "Display the Workday environments and their associated numbers")]
        public bool PrintEnvironments { get; set; }

    }
}
