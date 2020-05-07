namespace JoyBrick.Walkio.Game.AI
{
    using System.Collections;
    using System.Collections.Generic;
    using Pathfinding;
    using UnityEngine;

    public class Wonder : MonoBehaviour
    {
        public AILerp aiLerp;
        
        void Start()
        {
            StartCoroutine(TimedPickDest());
        }

        private IEnumerator TimedPickDest()
        {
            while (true)
            {
                if (aiLerp != null)
                {
                    aiLerp.destination = new Vector3(
                        Random.Range(0, 100.0f),
                        0,
                        Random.Range(0, 100.0f));
                }
                
                yield return new WaitForSeconds(5.0f);
            }
        }
    }
}
