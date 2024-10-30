using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectIndex", menuName = "ScriptableObjects/ObjectIndex", order = 1)]
public class ObjectIndex : ScriptableObject
{
    [SerializeField] public List<GameObject> objects;
}
