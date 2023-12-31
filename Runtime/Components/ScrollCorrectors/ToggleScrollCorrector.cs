﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Essentials
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleScrollCorrector : AbstractScrollCorrector<Toggle>
    {
        protected override void ReleaseTargetInteraction(PointerEventData eventData)
        {
            Target.OnPointerUp(eventData);
        }
    }
}