namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public partial class Bootstrap :
        Drawing.IDrawGizmos
    {
        public float gizmoMeshRadius = 0.5f;
        
        //
        private void RegisterToDrawingManager()
        {
            Drawing.DrawingManager.Register(this);
        }
        
        //
        private void OnDrawGizmos()
        {
        }

        public void DrawGizmos()
        {
            // float width = 1f;
            // float length = 1f;
            // float height = 2f;
            // float radius = 1f;
            // float radius = 0.5f;

            var hexahedronMesh = ProceduralToolkit.MeshDraft.Tetrahedron(gizmoMeshRadius).ToMesh();

            foreach (var pair in _placeholderGameObjects)
            {
                var trans = pair.Value.transform;
                //
                using (Drawing.Draw.InLocalSpace(trans))
                {
                    // Drawing.Draw.WireSphere(Vector3.zero, 0.5f, new Color(1.0f, 88/255f, 85/255f));
                    Drawing.Draw.WireMesh(hexahedronMesh);
                }
            }
        }
        
        #region Part that make realization of GameCommon.IGizmoService

        public Transform entityPlaceholderParent;
        public GameObject entityPlaceholderPrefab;
        private readonly Dictionary<Entity, GameObject> _placeholderGameObjects = new Dictionary<Entity, GameObject>();
        
        public void AddEntityToGameObject(Entity entity)
        {
            GameObject refGameObject = null;
            var result = _placeholderGameObjects.TryGetValue(entity, out refGameObject);
            if (!result)
            {
                var placeholderGameObject = GameObject.Instantiate(entityPlaceholderPrefab);
                
                //
                var placeholder = placeholderGameObject.GetComponent<EntityPlaceholder>();

                if (placeholder != null)
                {
                    placeholder.RefEntity = entity;
                }

                if (entityPlaceholderParent != null)
                {
                    placeholderGameObject.transform.SetParent(entityPlaceholderParent);
                }
                
                _placeholderGameObjects.Add(entity, placeholderGameObject);
            }
        }

        public void RemoveEntityFromGameObject(Entity entity)
        {
            GameObject refGameObject = null;
            var result = _placeholderGameObjects.TryGetValue(entity, out refGameObject);
            if (result)
            {
                GameObject.Destroy(refGameObject);
                _placeholderGameObjects.Remove(entity);
            }
        }

        #endregion
    }
}
