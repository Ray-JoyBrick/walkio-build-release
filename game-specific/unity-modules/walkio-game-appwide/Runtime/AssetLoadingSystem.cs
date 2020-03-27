namespace JoyBrick.Walkio.Game.Appwide
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Common = JoyBrick.Walkio.Game.Common;
    
    [DisableAutoCreation]
    public class AssetLoadingSystem :
        SystemBase,
        Common.IAssetLoadingService
    {
        protected override void OnUpdate()
        {
        }

        public async void LoadAssets<T>(string label, System.Action<IList<T>> callback)
        {
            var handle = Addressables.LoadAssetsAsync<T>(label, operation => { });
            var h = await handle.Task;

            callback(h);
        }

        public async void LoadAsset<T>(string address, System.Action<T> callback)
        {
            Debug.Log($"IAssetLoadingService - LoadAsset");
            
            var handle = Addressables.LoadAssetAsync<T>(address);
            await handle.Task;

            var asset = handle.Result;
            callback(asset);
        }
        
        public async void LoadAssets(IEnumerable<string> addresses,
            System.Action<IEnumerable<UnityEngine.Object>> callback)
        {
            Debug.Log($"IAssetLoadingService - LoadAssets");

            await Task.Delay(System.TimeSpan.FromSeconds(3.0f));
            
            // var handle = Addressables.LoadAssetAsync<UnityEngine.Object>(addresses);
            // await handle.Task;
            
            callback(new List<Object>());
        }

        public void LoadAssets(IEnumerable<string> addresses, EntityArchetype toCreate)
        {
            Debug.Log($"IAssetLoadingService - LoadAssets");
        }
    }
}