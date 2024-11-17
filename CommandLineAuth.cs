using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLARiNET
{
    internal class CommandLineAuth
    {
        public static string UsernameOption(string userName)
        {
            // Username
            if (String.IsNullOrEmpty(userName))
            {
                Console.WriteLine("Enter the username:\n");
                userName = Console.ReadLine().Trim();
                Console.WriteLine("");
            }
            return userName;
        }

        public static string PasswordOption(string password)
        {
            if (String.IsNullOrEmpty(password))
            {
                Console.WriteLine("Enter the password (will not be displayed):\n");
                password = PasswordPrompt();
            }
            else
            {
                password = Crypto.Unprotect(password);
            }
            return password;
        }

        public static string PasswordPrompt()
        {
            string pass = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    Console.Write("\b \b");
                    pass = pass[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    pass += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);
            return pass;
        }

        public static void OptionsEncrypt()
        {
            Console.WriteLine("Enter a password to encrypt:\n");
            string pass = CommandLineAuth.PasswordPrompt();
            string encPass = "";
            try
            {
                encPass = Crypto.Protect(pass);
            }
            // Perform a retry
            catch
            {
                encPass = Crypto.Protect(pass);
            }
            Console.WriteLine("\n\n" + encPass);
            Console.WriteLine("\n");
        }
    }
}
