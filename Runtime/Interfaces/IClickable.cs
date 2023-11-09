using UnityEngine.Events;

namespace Essentials
{
    public interface IClickable
    {
        UnityEvent onClick { get; }
    }
}