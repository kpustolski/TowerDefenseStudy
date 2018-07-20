using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class for all classes that will be a singleton.
/// It is an abstract class. T is a generic type.
/// Generic types are assigned at runtime.
/// https://www.youtube.com/watch?v=ibOBHDgg2kg&index=11&list=PLX-uZVK_0K_4uNwvKian1bscP9mVvOp1M
/// </summary>
/// T is monobeahavior no matter what.
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour{

    private static T instance;

    public static T Instance{
        get{
            // make sure the instance is not null
            if (instance == null) {

                instance = FindObjectOfType<T> ();
            }

            return instance;
        }
    }
}
