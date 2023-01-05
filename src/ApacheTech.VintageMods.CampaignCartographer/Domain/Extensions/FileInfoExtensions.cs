using System.IO;

namespace ApacheTech.VintageMods.CampaignCartographer.Domain.Extensions
{
    public static class FileInfoExtensions
    {
        public static void Rename(this FileInfo file, string newName)
        {
            var newPath = Path.Combine(file.DirectoryName!, newName);
            file.MoveTo(newPath);
        }
    }
}