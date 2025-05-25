using UnityEngine;

public class LoaderCallback : MonoBehaviour {
    bool isFirstUpdate = false;

    void Update()
    {
        if (!isFirstUpdate)
        {
            isFirstUpdate = true;
            Loader.LoaderCallback();
        }

    }
}
