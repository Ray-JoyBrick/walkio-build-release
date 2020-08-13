namespace JoyBrick.Walkio.Game
{
    using UniRx;

    using GameLevel = JoyBrick.Walkio.Game.Level;
    
    public partial class Bootstrap :
        GameLevel.ILevelSelectionProvider
    {
        //
        private int _selectedLeaderIndex;
        private int _selectedMinionIndex;
        private int _selectedLevelIndex;

        //
        public int SelectedLevel => _selectedLevelIndex;
    }
}
