using UnityEngine;

namespace AYellowpaper.SerializedCollections
{
    public class Attachement : MonoBehaviour
    {
        public string attachementType = null;
        [SerializedDictionary("Buff Type", "Buff Multiplier")]
        public SerializedDictionary<string, float> attachements;
        public GameObject attachementModel;
    }
}
