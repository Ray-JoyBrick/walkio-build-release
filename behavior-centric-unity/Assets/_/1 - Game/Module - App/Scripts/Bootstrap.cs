namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;

    public class Bootstrap : MonoBehaviour
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        void Start()
        {
            
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
