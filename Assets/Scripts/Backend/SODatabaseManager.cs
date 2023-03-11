using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NuclearBand;

public class SODatabaseManager : MonoBehaviour
{
    // Start is called before the first frame update
    async void Awake()
    {
        await SODatabase.InitAsync(null, null);
        // await SODatabase.LoadAsync();
    }

    /*
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SODatabase.Save();
    }

    private void OnApplicationQuit()
    {
        SODatabase.Save();
    }
     */
}
