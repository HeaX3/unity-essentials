﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Essentials
{
    [RequireComponent(typeof(InputField))]
    public class InputFieldScrollCorrector : AbstractScrollCorrector<InputField>
    {
        protected override void ReleaseTargetInteraction(PointerEventData eventData)
        {
            Target.OnPointerUp(eventData);
        }
    }
}