using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AYellowpaper.SerializedCollections
{
    public class ItemCatalogue : MonoBehaviour
    {
        [SerializedDictionary("Item Name", "Index")]
        public SerializedDictionary<string,GameObject> itemPrefabDictionnary;
    }
}
