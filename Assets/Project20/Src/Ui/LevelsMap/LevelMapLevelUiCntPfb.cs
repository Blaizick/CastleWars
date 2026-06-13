using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Proj21
{
    public class LevelMapLevelUiCntPfb : MonoBehaviour
    {
        public State awailableState;
        public State notAwailableState;
        public State completedState;
        public List<State> AllStates => new() {awailableState, notAwailableState, completedState, };

        [Serializable]
        public class State
        {
            public TMP_Text name;
            public Button btn;
            public GameObject root;
            public LineRenderer lineRenderer;
            public Image image;
        }
    }
}