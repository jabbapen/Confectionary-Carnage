using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializationTest : MonoBehaviour
{
    LevelSerializer serializer;
    [SerializeField] GameObject serializationTarget;

    // Start is called before the first frame update
    void Start()
    {
        serializer = GetComponent<LevelSerializer>();
        RunTest();
    }

    void ClearObject(GameObject target)
    {
        foreach (Transform child in target.transform)
        {
            // Destroy each child GameObject
            Destroy(child.gameObject);
        }
    }

    public void RunTest()
    {
        // Save a string for testing 
        string output = serializer.SerializeLevel(serializationTarget);
        // Clear our level
        ClearObject(serializationTarget);
        serializer.LoadField(output, serializationTarget, serializationTarget.transform);
        string nout = serializer.SerializeLevel(serializationTarget); 
        if (nout.Equals(output))
        {
            Debug.Log($"Serialization test passed! string: {output}");
        }
        else
        {
            Debug.LogError("Serialization test failed! Serializing and deserializing a string does not return the same value!");
        }
    }
}
