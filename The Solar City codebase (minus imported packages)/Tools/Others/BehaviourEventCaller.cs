using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BehaviourEventCaller : MonoBehaviour
{
    public static List<AwakeReceiver> awakeReceivers = new List<AwakeReceiver>();
    public static List<StartReceiver> startReceivers = new List<StartReceiver>();

    static BehaviourEventCaller instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning($"instance is already set");

        awakeReceivers.ForEach(r => r.Awake());
    }

    private void Start()
    {
        startReceivers.ForEach(r => r.Start());
    }


    private void OnApplicationQuit()
    {
        Rebinder.OnApplicationQuit();
    }

    public static void BeginRoutine(IEnumerator routine)
    {
        instance.StartCoroutine(routine);
    }

    public static void Delay(Action action, float? delay = null)
    {
        instance.Delay(action, delay);
    }

    /*
    void CallOnInstances<T>(string functionName)
    {
        Type[] types = GetTypesWithInterface<T>();
        foreach (Type type in types)
        {
            var method = type.GetMethod(functionName);
            if (method != null)
                method.Invoke(null, null);
        }
    }


    Type[] GetTypesWithInterface<T>()
    {
        return
            (from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where type.IsAssignableFrom(typeof(T))
             //where type.IsSubclassOf(typeof(parentClass))
             select type)
            .ToArray();
        

        //return typeof(parentClass).Assembly.GetTypes()
        //    .Where(type => type.IsSubclassOf(typeof(parentClass))).ToArray();
    }
    */

}
