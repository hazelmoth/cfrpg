using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Executor : MonoBehaviour
{
    private Node running;
    
    public void Execute(Node root)
    {
        running = root;
    }

    private IEnumerator UpdateCoroutine()
    {
        running.Update();
        yield return null;
    }
}
