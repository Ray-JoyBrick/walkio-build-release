namespace JoyBrick.Walkio.CrossProject
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Cross Project Data", menuName = "Walkio/CrossProject/Cross Project Data")]
    public class CrossProjectData : ScriptableObject
    {
        [System.Serializable]
        public class CommonProjectData
        {
            //
            public string projectBaseFolderName;
            
            public string environmentModuleFolderName;
        }

        [System.Serializable]
        public class AssetHudDesignProjectData
        {
            public string baseFolderName;
            public string generationBaseFolderName;
        }

        [System.Serializable]
        public class AssetLevelDesignProjectData
        {
            public string baseFolderName;
            public string generationBaseFolderName;

            public string levelModuleFolderName;
        }

        [System.Serializable]
        public class BehaviorUnificationProjectData
        {
            public string baseFolderName;
        }

        [System.Serializable]
        public class CompleteProjectData
        {
            public string baseFolderName;
            
        }

        public CommonProjectData commonProjectData;
        public AssetHudDesignProjectData assetHudDesignProjectData;
        public AssetLevelDesignProjectData assetLevelDesignProjectData;
        public BehaviorUnificationProjectData behaviorUnificationProjectData;
        public CompleteProjectData completeProjectData;
    }
}
