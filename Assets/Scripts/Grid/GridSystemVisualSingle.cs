using System.Collections;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour {

    [SerializeField] float startPos = -20;
    [SerializeField] float endPos = -1;
    [SerializeField] float liftDuration = 1f;
    [SerializeField] GameObject visual;
    [SerializeField] Animator animator;
    [SerializeField] float hideDelay = 10f;
    WaitForSeconds hideDelayWaitForSeconds;
    [SerializeField] ObjectShaker objectShaker;
    [SerializeField] Rigidbody rb;

    const string GLOW_TRIGGER = "Glow";

    void Awake()
    {
        Show();
        hideDelayWaitForSeconds = new WaitForSeconds(hideDelay);
    }

    public void Show()
    {
        visual.SetActive(true);
        StartCoroutine(Lift());
    }

    public void ShowPreDestroyVisual()
    {
        animator.SetTrigger(GLOW_TRIGGER);
        objectShaker.StartShake();
    }

    public void ShowDestroyVisual()
    {
        rb.isKinematic = false;
        objectShaker.StopShake();
        StartCoroutine(DelayHide());
    }

    IEnumerator DelayHide()
    {
        yield return hideDelayWaitForSeconds;
        visual.SetActive(false);
    }


    IEnumerator Lift()
    {
        rb.isKinematic = true;
        Vector3 pos = transform.position;
        pos.y = startPos;
        transform.position = pos;

        Vector3 start = transform.localPosition;
        Vector3 end = new Vector3(start.x, endPos, start.z);

        float elapsed = 0f;

        while (elapsed < liftDuration)
        {
            transform.localPosition = Vector3.Lerp(start, end, elapsed / liftDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = end;
    }

}
