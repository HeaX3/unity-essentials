using UnityEngine;

namespace Essentials.Components
{
    public class Spinner : MonoBehaviour
    {
        [SerializeField] private float speed = 360;
        
        private new Transform transform;

        private void Awake()
        {
            transform = base.transform;
        }

        private void Update()
        {
            transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z - Time.deltaTime * speed);
        }
    }
}