using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Essentials.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonScrollCorrector : AbstractScrollCorrector<Button>
    {
        protected override void ReleaseTargetInteraction(PointerEventData eventData)
        {
            Target.OnPointerUp(eventData);
        }
    }
}