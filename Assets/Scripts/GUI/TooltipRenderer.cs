using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GUI
{
    /// A MonoBehaviour that tracks the cursor position and renders tooltips when it is
    /// over a tooltip trigger.
    public class TooltipRenderer : MonoBehaviour
    {
        [SerializeField] private GameObject tooltipPrefab;
        [SerializeField] private Canvas tooltipParent;
        private readonly Vector2 tooltipOffset = Vector2.one * 10;
        private GameObject currentTooltipObject;
        private string currentText;
        private bool renderThisFrame;

        /// Renders the specified text as a tooltip next to the specified screen position, for
        /// only the current frame. Adjusts the orientation of the tooltip when near the sides
        /// of the screen (TODO).
        private void RenderTooltip(Vector2 screenPosition, string text)
        {
            if (currentTooltipObject == null)
                currentTooltipObject = Instantiate(tooltipPrefab, tooltipParent.transform);

            currentTooltipObject.transform.position = screenPosition + tooltipOffset;
            if (currentText != text)
                currentTooltipObject.GetComponentInChildren<TextMeshProUGUI>().text = text;
            renderThisFrame = true;
        }

        private void Update()
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);
            foreach (RaycastResult rr in raycastResults)
            {
                if (!rr.gameObject.TryGetComponent(out ITooltipTrigger tooltipObject)) continue;
                if (!tooltipObject.DoShowTooltip) continue;

                Vector3 tooltipPos = Input.mousePosition;
                RenderTooltip(tooltipPos, tooltipObject.GetText());
                break;
            }
        }

        private void LateUpdate()
        {
            if (currentTooltipObject == null) return;
            currentTooltipObject.SetActive(renderThisFrame);
            renderThisFrame = false;
        }
    }
}
