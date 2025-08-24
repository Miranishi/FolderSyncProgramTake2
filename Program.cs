using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace FolderSyncProgramTake2
{
    class Program
    {
        static string logFile = "sync_log.txt";

        static void Main(string[] args)
        {
            string sourceDir;
            string replicaDir;
            int intervalSeconds;

            // Checking if values are provided by user:
            if (args.Length >= 1)
                sourceDir = args[0];
            else
                sourceDir = @"D:\Demo\Source";

            if (args.Length >= 2)
                replicaDir = args[1];
            else
                replicaDir = @"D:\Demo\Replica";

            if (args.Length >= 3)
                intervalSeconds = int.Parse(args[2]);
            else
                intervalSeconds = 10;


            Log("=========================================");
            Log($"Source: {sourceDir}");
            Log($"Replica: {replicaDir}");
            Log($"Interval: {intervalSeconds} seconds");
            Log("=========================================\n");

            while (true)
            {
                Log($"[ {DateTime.Now} ] Synchronizing...");
                SyncFolders(sourceDir, replicaDir);
                Log("Sync done.\n");

                Thread.Sleep(intervalSeconds * 1000);
            }
        }

        static void SyncFolders(string source, string replica)
        {
            // Checking if replica folder exists
            if (!Directory.Exists(replica))
                Directory.CreateDirectory(replica);

            // Coping and Updating files
            foreach (string file in Directory.GetFiles(source))
            {
                string fileName = Path.GetFileName(file);
                string destPath = Path.Combine(replica, fileName);

                File.Copy(file, destPath, true); // overwrite if changed
                Log($"Copied file: {fileName}");
            }

            // Coping subdirectories 
            foreach (string dir in Directory.GetDirectories(source))
            {
                string dirName = Path.GetFileName(dir);
                string destDir = Path.Combine(replica, dirName);

                SyncFolders(dir, destDir);
            }

            // Deleting extra files
            foreach (string file in Directory.GetFiles(replica))
            {
                string fileName = Path.GetFileName(file);
                string sourceFile = Path.Combine(source, fileName);

                if (!File.Exists(sourceFile))
                {
                    File.Delete(file);
                    Log($"Deleted extra file: {fileName}");
                }
            }

            // Deleting extra directories from replica
            foreach (string dir in Directory.GetDirectories(replica))
            {
                string dirName = Path.GetFileName(dir);
                string sourceDir = Path.Combine(source, dirName);

                if (!Directory.Exists(sourceDir))
                {
                    Directory.Delete(dir, true);
                    Log($"Deleted extra folder: {dirName}");
                }
            }
        }

        static void Log(string message)
        {
            Console.WriteLine(message);
            File.AppendAllText(logFile, message + Environment.NewLine);
        }
    }
}