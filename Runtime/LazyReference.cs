using UnityEngine;

namespace Essentials
{
    public class LazyReference<T> where T : MonoBehaviour
    {
        private T _item;
        
        public T item
        {
            get
            {
                if (_item) return _item;
                _item = Object.FindObjectOfType<T>(true);
                return _item;
            }
        }
    }
}