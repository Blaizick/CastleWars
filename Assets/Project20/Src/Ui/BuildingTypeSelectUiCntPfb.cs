using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Proj21
{
    public class BuildingTypeSelectUiCntPfb : MonoBehaviour
    {
        public State selectedState;
        public State notSelectedState;

        public List<State> AllStates => new() {selectedState, notSelectedState};

        [Serializable]
        public class State
        {
            public TMP_Text nameText;
            public Image image;
            public GameObject root;
            public Button btn;
        }
    }
}