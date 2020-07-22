namespace JoyBrick.Walkio.Game.FlowControl
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class DoneSettingAssetWaitAttribute : Attribute
    {
        public string FlowName { get; set; }

        public DoneSettingAssetWaitAttribute(string flowName)
        {
            FlowName = flowName;
        }
    }
}
