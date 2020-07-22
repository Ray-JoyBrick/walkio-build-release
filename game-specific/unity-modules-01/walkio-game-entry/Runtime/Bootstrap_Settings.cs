namespace JoyBrick.Walkio.Game
{
    using UniRx;
    using UniRx.Diagnostics;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public partial class Bootstrap :
        GameCommon.IGameSettingProvider
    {
        public GameCommon.GameSettings gameSettings;

        public GameCommon.GameSettings GameSettings => gameSettings;
    }
}
