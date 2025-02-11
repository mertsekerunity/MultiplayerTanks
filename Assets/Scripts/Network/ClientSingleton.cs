using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    static ClientSingleton instance;

    public ClientGameManager GameManager { get; private set; }

    public static ClientSingleton Instance
    {
        get 
        {
            if(instance != null) { return instance; }

            instance = FindObjectOfType<ClientSingleton>();

            if(instance == null)
            {
                Debug.LogWarning("No ClientSingleton found in the scene.");
                return null;
            }

            return instance;
        }
    }

    bool isClientCreated = false;

    // Start is called before the first frame update
    async void Start()
    {
        DontDestroyOnLoad(gameObject);

        Debug.Log("ClientSingleton Start method called");

        if (!isClientCreated)
        {
            isClientCreated = true;
            bool clientCreated = await CreateClient();

            Debug.Log($"Client creation result: {clientCreated}");

            if (clientCreated)
            {
                GameManager.GoToMenu();
            }
        }
    }
    
    public async Task<bool> CreateClient()
    {
        if(GameManager == null)
        {
            GameManager = new ClientGameManager();
            Debug.Log("ClientGameManager instance created");
        }

        bool initResult = await GameManager.InitAsync();

        Debug.Log($"ClientGameManager init result: {initResult}");

        return initResult;
    }
}
