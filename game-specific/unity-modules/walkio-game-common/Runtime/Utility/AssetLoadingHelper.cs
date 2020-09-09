namespace JoyBrick.Walkio.Game.Common.Utility
{
    using System.Threading.Tasks;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    public static partial class AssetLoadingHelper
    {
        public static async Task<T> GetAsset<T>(string addressName)
        {
            var handle = Addressables.LoadAssetAsync<T>(addressName);
            var r = await handle.Task;
        
            return r;
        }
        
        public static async Task<SceneInstance> GetScene(string addressName)
        {
            var handle = Addressables.LoadSceneAsync(addressName, LoadSceneMode.Additive);
            var r = await handle.Task;

            return r;
        }
    }
}
