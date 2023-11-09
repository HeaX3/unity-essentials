using UnityEngine;

namespace Essentials
{
    [AddComponentMenu("Misc/README Info")]
    public class Readme : MonoBehaviour
    {
        [TextArea(10,1000)]
        public string Comment = "Information Here.";
    }
}