namespace JoyBrick.Walkio.Game.Common
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class DoneLoadingAssetWaitAttribute : Attribute
    {
        public string FlowName { get; set; }

        public DoneLoadingAssetWaitAttribute(string flowName)
        {
            FlowName = flowName;
        }
    }
}
