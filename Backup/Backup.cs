using System;
using System.Collections.Generic;
using System.IO;

namespace Backup
{
    class Backup
    {
        private readonly string _sourcePath;
        private readonly string _backupPath;
        private IEnumerable<string> _sourceFiles;
        private readonly List<string> _conflicts;

        public Backup(string sourcePath, string backupPath)
        {
            _sourcePath = sourcePath;
            _backupPath = backupPath;
            _conflicts = new List<string>();
        }

        public void Scan()
        {
            _sourceFiles = Directory.EnumerateFiles(_sourcePath, "*.*", SearchOption.AllDirectories);
        }

        public void Run()
        {
            foreach (var sourceFile in _sourceFiles)
            {
                var backupFile = SourceToBackupPathTranslator(sourceFile);
                if (File.Exists(backupFile))
                {
                    _conflicts.Add(sourceFile);
                    continue;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(backupFile));
                File.Copy(sourceFile, backupFile);
            }
        }

        public void FixConflics()
        {
            bool replaceAll = false, skipAll = false;
            foreach (var conflict in _conflicts)
            {
                FileInfo sourceFileInfo = new FileInfo(conflict);
                FileInfo backupFileInfo = new FileInfo(SourceToBackupPathTranslator(conflict));

                if (replaceAll)
                {
                    sourceFileInfo.CopyTo(backupFileInfo.FullName, true);
                    continue;
                }

                if (skipAll)
                    continue;

                Console.WriteLine("There is a conflic:");
                Console.WriteLine($"Source file:{sourceFileInfo.Name} with size:{sourceFileInfo.Length}bytes and created at {sourceFileInfo.CreationTime}");
                Console.WriteLine($"Backup file:{backupFileInfo.Name} with size:{backupFileInfo.Length}bytes and created at {backupFileInfo.CreationTime}");

                bool badOption = true;
                while (badOption)
                {
                    Console.WriteLine("Replace(r)/Replace all(ra)/Skip(s)/Skip all(sa)/Cancel(c)");
                    var option = Console.ReadLine();
                    badOption = false;
                    switch (option)
                    {
                        case "c":
                            return;
                        case "sa":
                            skipAll = true;
                            break;
                        case "s":
                            break;
                        case "ra":
                            sourceFileInfo.CopyTo(backupFileInfo.FullName, true);
                            replaceAll = true;
                            break;
                        case "r":
                            sourceFileInfo.CopyTo(backupFileInfo.FullName, true);
                            break;
                        default:
                            badOption = true;
                            break;
                    }
                }
            }
        }

        private string SourceToBackupPathTranslator(string sourcePath)
        {
            return sourcePath.Replace(_sourcePath, _backupPath);
        }
    }

}
