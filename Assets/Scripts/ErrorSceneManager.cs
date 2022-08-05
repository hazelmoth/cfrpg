using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class ErrorSceneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TMP_InputField bodyText;
    
    private static string title;
    private static string body;
    
    public static void SetDisplayedError(string title, string body)
    {
        ErrorSceneManager.title = title;
        ErrorSceneManager.body = body;
    }

    private void Start()
    {
        titleText.text = title;
        bodyText.text = body;
    }
    
    [UsedImplicitly]
    public void GoToMainMenuButton()
    {
        SceneChangeActivator.GoToMainMenu();
    }
}
