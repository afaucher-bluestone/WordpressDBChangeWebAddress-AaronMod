using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace WordpressDBChangeWebAddress
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.Write(@"USAGE: {0} <inputBackupName> <outputBackupName> <originalWebsite> <newWebsite>" +
                    "\nWhere\n\t<inputBackupName> = original Backup from Wordpress" +
                    "\n\t<outputBackupName> = file to create with changed website addresses." +
                    "\n\t<originalWebsite> = Website address to change" +
                    "\n\t<newWebsite> = Website address to change it to.\n\n", System.AppDomain.CurrentDomain.FriendlyName);
            }
            else
            {
                var p = new Processor(args[0],args[1],args[2],args[3]);
                p.ProcessChanges();
				Console.WriteLine ();
				Console.WriteLine ();
				Console.WriteLine ("The utility completed");
				Console.WriteLine (p.regularReplaceCounter + " regular replacements,");
				Console.WriteLine(p.specialReplaceCounter + " special replacements,");
				Console.WriteLine("and skipped " + p.emailDetectCounter + " email addresses.");
			}
#if DEBUG
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
#endif

        }
    }
}
