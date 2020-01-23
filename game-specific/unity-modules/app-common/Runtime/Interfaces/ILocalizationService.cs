namespace JoyBrick.Walkio.Game.App.Common.Main
{
    public interface ILocalizationService
    {
        void ChangeLanguage(string language);

        string GetByKey(string key);
    }
}
