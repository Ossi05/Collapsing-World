using UnityEngine;
using UnityEngine.UI;

public class LevelLoadButton : MonoBehaviour {

    [SerializeField] Loader.Scene scene;
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => Loader.Load(scene));
    }

}
