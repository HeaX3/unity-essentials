using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Essentials.UI
{
    [RequireComponent(typeof(Dropdown))]
    public class DropdownScrollCorrector : AbstractScrollCorrector<Dropdown>
    {
        protected override void ReleaseTargetInteraction(PointerEventData eventData)
        {
            Target.OnPointerUp(eventData);
        }
    }
}