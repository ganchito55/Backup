using System;
using System.IO;

namespace Backup
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: dotnet Backup.dll sourceDir backupDir");
                return;
            }

            if (!(Directory.Exists(args[0]) && Directory.Exists(args[1])))
            {
                Console.WriteLine("Both directories (source and backup) must exist");
                return;
            }

            var software = new Backup(args[0],args[1]);
            software.Scan();
            software.Run();
            software.FixConflics();
            Console.WriteLine("Backup completed");
        }
    }
}
