namespace JoyBrick.Walkio.Build.LevelDesignExport.Editor
{
    using System.IO;

    public class Utility
    {
        public static void CreateDirectoryIfNotExisted(string directoryPath)
        {
            var existed = Directory.Exists(directoryPath);
            if (!existed)
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
