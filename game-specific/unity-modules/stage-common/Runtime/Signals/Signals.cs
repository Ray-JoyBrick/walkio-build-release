namespace JoyBrick.Walkio.Game.Stage.Common.Main
{
    public class AllModuleSetupDoneSignal
    {
    }

    public class ModuleSetupDoneSignal
    {
        public string Id { get; set; }
    }

    public class PlayCutSceneSignal
    {
        public string Id { get; set; }
    }

    public class LoadAreaSignal
    {
        public string Id { get; set; }
    }
}