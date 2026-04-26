using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AYellowpaper.SerializedCollections
{
    public class Build : MonoBehaviour
    {
        public string buildName;
        [SerializedDictionary("Resource Name", "Number")]
        public SerializedDictionary<string, int> craftNeeds;
        public GameObject buildPrefab;
        public BuildUsage buildUsage;
        public Vector3 buildOffset = Vector3.zero;
        public string buildCategory;
    }
}
