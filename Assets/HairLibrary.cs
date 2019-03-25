using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairLibrary : MonoBehaviour
{
    [SerializeField] List<Hair> hairs = new List<Hair>();
    static HairLibrary instance;

    public void Start()
    {
        instance = this;
    }

    public static List<Hair> GetHairs ()
    {
        return instance.hairs;
    }
}
