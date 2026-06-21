

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Proj21
{
    public class TechNodeUiCntPfb : MonoBehaviour
    {
        public State awailableState;
        public State notAwailableState;
        public State researchedState;
        
        public RectTransform linesRootTr;
        public GameObject root;

        public List<State> AllStates => new()
        {
            awailableState, notAwailableState, researchedState,
        };

        [Serializable]
        public class State
        {
            public GameObject root;
            public TMP_Text nameText;
            public Button btn;
        }
    }
}