namespace JoyBrick.Walkio.Game.Common
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static partial class Utility
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
