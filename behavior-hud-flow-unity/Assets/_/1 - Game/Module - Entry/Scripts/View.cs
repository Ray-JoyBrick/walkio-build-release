namespace JoyBrick.Walkio.Game
{
    using System;
    using UniRx;
    using UnityEngine;

    public class View :
        MonoBehaviour
    {
        public CommandProducer commandProducer;
        
        public void LoadData()
        {
            commandProducer.LoadDataCommand();
        }
    }
}
