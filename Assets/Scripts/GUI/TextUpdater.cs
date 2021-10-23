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
    
        // Start is called before the first frame update
        void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
            Debug.Assert(text != null, "TextUpdater missing text component!");
        }

        // Update is called once per frame
        void Update()
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
