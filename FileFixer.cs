using System;
using System.Collections.Generic;
using System.IO;

namespace MassiveAssaultScnFixer
{
    public class FileFixer
    {
        private readonly string _scnFilePath;
        private readonly string _mscFilePath;

        private bool _lastScnLineWasEmpty = false;
        private readonly Dictionary<int, int> _unitIdMapping = new Dictionary<int, int>();
        private int _unitCounter = 1;
        
        public FileFixer(string file)
        {
            var directory = Path.GetDirectoryName(file) 
                         ?? throw new ArgumentException("File is not exists", nameof(file));
            _scnFilePath = file;
            _mscFilePath = Path.Combine(directory, $"{Path.GetFileNameWithoutExtension(file)}.msc");
        }

        public void Fix()
        {
            if (!File.Exists(_mscFilePath))
            {
                Console.WriteLine($"{_scnFilePath} is a NET scenario. Skipped.");
                return;
            }
            Console.WriteLine($"Fixing: {_scnFilePath}");
            FixFile(_scnFilePath, ProcessScnLine);
            Console.WriteLine($"Fixing: {_mscFilePath}");
            FixFile(_mscFilePath, ProcessMscLine);
        }

        private void FixFile(string originalPath, Func<string?, string?> processingFunc)
        {
            var newScnFilePath = $"{originalPath}.new";
            using (var newScnFile = File.CreateText(newScnFilePath))
            using (var originalFile = File.OpenText(originalPath))
            {
                for (string? originalLine = originalFile.ReadLine(); originalLine != null; 
                    originalLine = originalFile.ReadLine())
                {
                    var fixedLine = processingFunc(originalLine);
                    if (fixedLine != null)
                        newScnFile.WriteLine(fixedLine);
                }
            }
            File.Replace(newScnFilePath, originalPath, null);
        }

        private string? ProcessScnLine(string? originalLine)
        {
            if (originalLine == null 
                || string.IsNullOrEmpty(originalLine) && _lastScnLineWasEmpty 
                || originalLine.StartsWith(';'))
                return null;

            if (originalLine == string.Empty)
            {
                _lastScnLineWasEmpty = true;
                return string.Empty;
            }

            _lastScnLineWasEmpty = false;

            var tokens = originalLine.Split(',');
            if (tokens[0].EndsWith("UNIT"))
            {
                var originalUnitId = int.Parse(tokens[1]);
                _unitIdMapping[originalUnitId] = _unitCounter;
                tokens[1] = _unitCounter.ToString();
                ++_unitCounter;
                return string.Join(',', tokens);
            }

            return originalLine;
        }
        
        private string? ProcessMscLine(string? originalLine)
        {
            if (originalLine != null && (originalLine.StartsWith("SELECT_UNIT2") || originalLine.StartsWith("UNSELECT_UNIT2")))
            {
                var tokens = originalLine.Split(' ');
                var unitId = int.Parse(tokens[1]);
                
                //fix for convoy scenario (there is typo in the Id)
                if (unitId == 41050)
                    unitId = 4105;
                if (!_unitIdMapping.ContainsKey(unitId))
                {
                    Console.WriteLine($"Unit with id {unitId} is not mapped!");
                    return null;
                }

                tokens[1] = _unitIdMapping[unitId].ToString();
                return string.Join(' ', tokens);
            }

            return originalLine;
        }
    }
}