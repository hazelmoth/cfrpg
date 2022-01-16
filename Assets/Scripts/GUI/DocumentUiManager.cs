using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace GUI
{
    public class DocumentUiManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;

        public void SetMessage(string message)
        {
            messageText.text = message;
        }

        [UsedImplicitly]
        public void CloseLetter()
        {
            UIManager.CloseAllMenus();
        }
    }
}
