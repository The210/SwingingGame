using System.Collections;   
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ObjectManager
{
    public List<GameObject> objectList;

    public void AddGameObject(GameObject objectToAdd)
    {
        GameObject otherObject = objectToAdd;
        objectList.Add(otherObject);
        Debug.Log("Player joined");
    }
}
