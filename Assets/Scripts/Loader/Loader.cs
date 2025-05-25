using UnityEngine.SceneManagement;

public static class Loader {

    public enum Scene {
        MainMenu,
        Game,
        Loading,
    }

    public static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;
        SceneManager.LoadScene(Loader.Scene.Loading.ToString());
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
