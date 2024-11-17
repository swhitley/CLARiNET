using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace CLARiNET
{
    public class Options
    {
        [Value(index: 0, MetaName = "Command", Required = false, HelpText = "CLARiNET Commands:\n\n" + CLARiNET.Commands.COMMAND_LIST)]
        public string Command { get; set; }

        [Value(index: 1, MetaName = "File or Directory", Required = false, HelpText = "Path or Path and file name")]
        public string Path { get; set; }

        [Value(index: 2, MetaName = "Parameters", Required = false, HelpText = "Parameters for the command (For CLAR_UPLOAD and CLAR_DOWNLOAD, enter the Cloud Collection)")]
        public string Parameters { get; set; }

        [Value(index: 3, MetaName = "Environment Number", Required = false, HelpText = "Number associated with a Workday environment (list all with -w parameter)")]
        public string EnvNum { get; set; }

        [Value(index: 4, MetaName = "Tenant", Required = false, HelpText = "Workday Tenant")]
        public string Tenant { get; set; }

        [Value(index: 5, MetaName = "Username", Required = false, HelpText = "Username")]
        public string Username { get; set; }

        [Value(index: 6, MetaName = "Password", Required = false, HelpText = "Password (must be encrypted using the -e option)")]
        public string Password { get; set; }

        [Option('c', "commandline", Required = false,
               HelpText =
                   "Display the commandline and exit without executing the commands.")]
        public bool PrintCommandline { get; set; }

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
