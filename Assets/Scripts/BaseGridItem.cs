using UnityEngine;

public abstract class BaseGridItem : MonoBehaviour {

    public abstract void HandleGridParentDeath();
    public abstract void HandleGridParentIsSafeChange(bool isSafe);

}
