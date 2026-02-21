using System.IO;

namespace HelpGenerator.Powershell.Parsing
{
    internal static class PowerShellFileScanner
    {
        public static IReadOnlyList<string> GetPowerShellFiles(string modulePathOrFile)
        {
            if (File.Exists(modulePathOrFile))
            {
                var ext = Path.GetExtension(modulePathOrFile);
                if (!ext.Equals(".ps1", StringComparison.OrdinalIgnoreCase) &&
                    !ext.Equals(".psm1", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException($"Unsupported file type: {modulePathOrFile}");
                }

                return new[] { Path.GetFullPath(modulePathOrFile) };
            }

            if (!Directory.Exists(modulePathOrFile))
            {
                throw new DirectoryNotFoundException($"ModulePath not found: {modulePathOrFile}");
            }

            return Directory.EnumerateFiles(modulePathOrFile, "*.*", SearchOption.AllDirectories)
                .Where(p => p.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase)
                         || p.EndsWith(".psm1", StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
