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
    public static Hair GetHairById(string id)
    {
		if (id == string.Empty)
		{
			return null;
		}
        foreach (Hair hair in instance.hairs)
        {
            if (hair.hairId == id)
                return hair;
        }
        Debug.Log("Hair ID \"" + id + "\" not found.");
        return null;
    }
}
