namespace JoyBrick.Walkio.Game.Common.Utility
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static partial class FlowControlHelper
    {
        public static void NotifyFinishIndividualLoadingAsset(IFlowControl flowcontrol, string atPart)
        {
            flowcontrol.FinishLoadingAsset(new FlowControlContext
            {
                Name = atPart
            });
        }
    }
}
