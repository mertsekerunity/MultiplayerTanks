using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] ClientSingleton clientPrefab;
    [SerializeField] HostSingleton hostPrefab;
    [SerializeField] ServerSingleton serverPrefab;

    // Start is called before the first frame update
    async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }
    
    async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            ServerSingleton serverSingleton = Instantiate(serverPrefab);

            await serverSingleton.CreateServer();

            await serverSingleton.GameManager.StartGameServerAsync();

        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost();

            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            await clientSingleton.CreateClient();

            //logic to naviate to the main menu
        }
    }
}
