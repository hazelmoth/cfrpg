using System;
using TMPro;
using UnityEngine;

namespace GUI
{
    /// Updates the value of a text object every frame, using a string provider function.
    public class TextUpdater : MonoBehaviour
    {
        private TextMeshProUGUI text;
        private Func<string> supplier;

        private void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
            Debug.Assert(text != null, "TextUpdater missing text component!");
        }

        private void Update()
        {
            if (supplier == null || text == null) return;
            text.text = supplier.Invoke();
        }

        public void SetValueSupplier(Func<string> supplier)
        {
            this.supplier = supplier;
        }
    }
}
