using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLARiNET
{
    public static class CommandLineCommand
    {
        public static string CommandGet(string command)
        {
            if (String.IsNullOrEmpty(command))
            {
                string[] commands = Commands.COMMAND_LIST.Split("\n");
                PrintCommand(commands);
                Console.WriteLine("Enter the number that identifies the CLARiNET Command (1 - " + commands.Length + "):\n");
                command = Console.ReadLine().Trim().ToUpper();
                Console.WriteLine("");
            }
            return command;
        }

        public static string CommandValidate(string command)
        {
            // Ensure Command is uppercase.
            if (command != null)
            {
                command = command.Trim().ToUpper();
            }

            int ndx = 0;
            if (int.TryParse(command, out ndx))
            {
                string[] commands = Commands.COMMAND_LIST.Split("\n");
                command = commands[ndx - 1];
            }


            // Check for valid commands
            switch (command.ToEnum<Command>())
            {
                case Command.CLAR_UPLOAD:
                case Command.CLAR_DOWNLOAD:
                case Command.DRIVE_UPLOAD:
                case Command.DRIVE_TRASH:
                case Command.PHOTO_DOWNLOAD:
                case Command.PHOTO_UPLOAD:
                case Command.DOCUMENT_UPLOAD:
                case Command.CANDIDATE_ATTACHMENT_UPLOAD:
                    break;
                default:
                    throw new Exception("Invalid command. Please use --help for a list of valid commands.");
            }
            Console.WriteLine("\n\nCommand: " + command + "\n");

            return command;
        }

        public static void PrintCommand(string[] commands)
        {
            string[] commandName = new string[3];
            string colFormat = "{0,-35} {1,-35} {2,-35}\n";

            Console.WriteLine("-- Commands --\n");
            for (int ndx = 0; ndx < commands.Length; ndx++)
            {
                if (ndx > 0 && ndx % 3 == 0)
                {
                    Console.Write(colFormat, commandName[0], commandName[1], commandName[2]);
                    commandName = new string[3];
                }
                if (commandName[0] == null)
                {
                    commandName[0] = ndx + 1 + ") " + commands[ndx];
                }
                else
                {
                    if (commandName[1] == null)
                    {
                        commandName[1] = ndx + 1 + ") " + commands[ndx];
                    }
                    else
                    {
                        commandName[2] = ndx + 1 + ") " + commands[ndx];
                    }
                }
            }
            if (commandName[0] != null)
            {
                Console.Write(colFormat, commandName[0], commandName[1], commandName[2]);
            }
            Console.WriteLine("\n");
        }
    }
}
