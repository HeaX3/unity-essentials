using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Essentials.UI
{
    public abstract class AbstractScrollCorrector : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        private const float BaseDragThreshold = 20;
        private const int ReferenceDpi = 210;

        private float _dragThreshold;

        private ScrollRect _scrollRect = null;
        private bool _controllingScrollRect;
        private Vector2 _pointerStartPosition;
        private bool _vertical;
        private bool _horizontal;

        protected virtual void OnEnable()
        {
            _scrollRect = GetComponentInParent<ScrollRect>();
        }

        private void Start()
        {
            if (!_scrollRect) _scrollRect = GetComponentInParent<ScrollRect>();
        }

        private void OnDisable()
        {
            if (!_scrollRect || !_controllingScrollRect) return;
            _scrollRect.enabled = true;
            _controllingScrollRect = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_scrollRect || !_scrollRect.enabled) return;
            _controllingScrollRect = true;
            _scrollRect.enabled = false;
            _pointerStartPosition = eventData.position;
            _dragThreshold = BaseDragThreshold * Screen.dpi / ReferenceDpi;
            _vertical = _scrollRect.vertical;
            _horizontal = _scrollRect.horizontal;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_scrollRect || !_controllingScrollRect) return;
            _controllingScrollRect = false;
            _scrollRect.enabled = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_scrollRect) return;
            if (!_scrollRect.enabled)
            {
                var delta = eventData.position - _pointerStartPosition;
                var distance = Mathf.Abs(_horizontal ? delta.x : 0) + Mathf.Abs(_vertical ? delta.y : 0);
                if (distance < _dragThreshold) return;
                _scrollRect.enabled = true;
                eventData.eligibleForClick = false;
                ReleaseTargetInteraction(eventData);
                _scrollRect.OnBeginDrag(eventData);
            }
            else
            {
                _scrollRect.OnDrag(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_scrollRect || !_scrollRect.enabled) return;
            _scrollRect.OnEndDrag(eventData);
        }

        protected abstract void ReleaseTargetInteraction(PointerEventData eventData);
    }

    public abstract class AbstractScrollCorrector<T> : AbstractScrollCorrector where T : MonoBehaviour
    {
        protected T Target { get; private set; }

        protected sealed override void OnEnable()
        {
            base.OnEnable();
            Target = GetComponentInParent<T>();
        }
    }
}