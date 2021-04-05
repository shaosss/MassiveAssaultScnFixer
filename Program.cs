using System;
using System.IO;

namespace MassiveAssaultScnFixer
{
    class Program
    {
        private const string ScenariosFolderName = "scenarios";
        private const string ScenariosBackupFolderName = "scenarios_backup";
        
        static void Main(string[] args)
        {
            var gameFolder = args[0];
            FixFilesInFolder(gameFolder);
        }

        private static void FixFilesInFolder(string gameFolder)
        {
            var scenariosFolder = Path.Combine(gameFolder, ScenariosFolderName);
            BackupScenarios(gameFolder, scenariosFolder);
            var files = Directory.GetFiles(scenariosFolder, "*.scn", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    var fixer = new FileFixer(file);
                    fixer.Fix();
                    Console.WriteLine($"Scenario is fixed: {file}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed on fixing scenario: {file}");
                    Console.WriteLine(e);
                }
            }
        }

        private static void BackupScenarios(string gameFolder, string scenariosFolder)
        {
            var backupFolder = Path.Combine(gameFolder, ScenariosBackupFolderName);
            foreach (string dirPath in Directory.GetDirectories(scenariosFolder, "*", 
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(scenariosFolder, backupFolder));
            
            foreach (string newPath in Directory.GetFiles(scenariosFolder, "*.*", 
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(scenariosFolder, backupFolder), true);
        }
    }
}