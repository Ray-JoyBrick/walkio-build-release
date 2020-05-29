namespace JoyBrick.Walkio.Build.HudDesignExport.Editor
{
    using System.IO;
    using I2.Loc;
    using UnityEditor;
    using UnityEngine;

    public partial class LocalizationCreator
    {
        private static void ImportLocalization(string localizationDataDirectory, string fromDataDirectory, string hudDirectory)
        {
            //
            var dataDirectory = Path.Combine(localizationDataDirectory, fromDataDirectory);
            var hasDataDirectory = Directory.Exists(dataDirectory);
            if (!hasDataDirectory)
            {
                Debug.LogError($"Localization data directory at: {dataDirectory} is not presented");
                return;
            }
            
            //
            var languageAsset = Path.Combine("Assets", "_", "1 - Game - Hud Design", hudDirectory, "Extension - I2", "I2Languages.asset");
            var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(languageAsset);
            var languageSourceAsset = (so as LanguageSourceAsset);
            if (languageSourceAsset == null)
            {
                Debug.LogError($"I2 Language Asset at: {languageAsset} is not presented or not valid");
                return;
            }

            var csvFiles = Directory.EnumerateFiles(dataDirectory, "*.csv");
            var encoding = System.Text.Encoding.UTF8;
            var source = languageSourceAsset.mSource;

            foreach (var csvFile in csvFiles)
            {
                var fileName = csvFile;
                var CSVstring = LocalizationReader.ReadCSVfile (fileName, encoding);
                    
                var separator = ',';
                var errorMessage= source.Import_CSV( string.Empty, CSVstring, eSpreadsheetUpdateMode.Merge, separator);
                var hasError = !string.IsNullOrEmpty(errorMessage);
                if (hasError)
                {
                    Debug.LogError($"Converting from {fileName} has result error {errorMessage}");
                }
            }
        }

        #if ASSET_HUD_DESIGN
        
        [MenuItem("Assets/Walkio/Import/Localization")]
        public static void Perform()
        {
            var localizationDataDirectory = Path.Combine(
                Application.dataPath,
                "_",
                "1 - Game - Hud Design",
                "Preprocess Assets",
                "external-data",
                "data-assets",
                "Localization");

            ImportLocalization(localizationDataDirectory, "app", "Module - Hud - App");
            ImportLocalization(localizationDataDirectory, "preparation", "Module - Hud - Preparation");
            ImportLocalization(localizationDataDirectory, "stage", "Module - Hud - Stage");
        }
        
        #endif
    }
}
