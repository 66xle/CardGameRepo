using System;
using System.Collections.Generic;
using UnityEngine;

namespace config {
    [Serializable]
    public class CardPlayConfig {
        [SerializeField]
        public List<GameObject> playArea;
        
        [SerializeField]
        public bool destroyOnPlay;

    }
}
