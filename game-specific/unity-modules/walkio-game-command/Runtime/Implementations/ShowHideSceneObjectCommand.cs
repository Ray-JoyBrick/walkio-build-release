namespace JoyBrick.Walkio.Game.Command
{
    public class ShowHideSceneObjectCommand : ICommand
    {
        public int Category { get; set; }
        public bool Hide { get; set; }
    }
}
