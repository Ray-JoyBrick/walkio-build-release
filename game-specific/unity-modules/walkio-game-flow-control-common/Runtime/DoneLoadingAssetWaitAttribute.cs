namespace JoyBrick.Walkio.Game.FlowControl
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class DoneLoadingAssetWaitAttribute : Attribute
    {
        public string FlowName { get; set; }
        public string Category { get; set; }

        public DoneLoadingAssetWaitAttribute(string flowName, string category = "")
        {
            FlowName = flowName;
            Category = category;
        }
    }
}
