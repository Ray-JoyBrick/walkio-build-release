namespace JoyBrick.Walkio.Game.Main
{
    using System.Collections;
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;

    public class CameraManager : MonoBehaviour
    {
        public Camera Camera;

        void Start()
        {
            // World.Active.GetOrCreateSystem<CharacterMoveSystem>().CameraTransform = Camera.transform;
        }

    }
}