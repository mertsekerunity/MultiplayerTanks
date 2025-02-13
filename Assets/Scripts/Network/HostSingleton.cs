using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    static HostSingleton instance;

    public HostGameManager GameManager { get; private set; }

    public static HostSingleton Instance
    {
        get 
        {
            if(instance!=null) { return instance; }

            instance = FindObjectOfType<HostSingleton>();

            if(instance==null)
            {
                Debug.LogWarning("No HostSingleton found in the scene.");
            }

            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}
