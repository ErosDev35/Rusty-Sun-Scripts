using UnityEngine;

namespace AYellowpaper.SerializedCollections
{
    public class Craft : MonoBehaviour
    {
        public Item itemToCraft;
        [SerializedDictionary("Resource Name", "Number")]
        public SerializedDictionary<string, int> craftNeeds;
        [SerializedDictionary("Skill Name", "Number")]
        public SerializedDictionary<string, int> skillNeeds;
    }
}
