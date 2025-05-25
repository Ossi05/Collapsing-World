using System;
using System.Collections;
using UnityEngine;

public class Collectable : BaseGridItem {

    [SerializeField] float pickUpTime = 7f;
    [SerializeField] float pickUpFailedDelay = 0.3f;

    public event EventHandler OnCollected;
    public event EventHandler OnPickUpFailed;

    Animator animator;

    bool collected;
    const string HIDE_TRIGGER = "Hide";

    void Awake()
    {
        animator = GetComponent<Animator>();
        Hide();
    }

    public void Show()
    {
        animator.ResetTrigger(HIDE_TRIGGER);
        collected = false;
        gameObject.SetActive(true);
        StartCoroutine(DelayHide());
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(pickUpTime);
        animator.SetTrigger(HIDE_TRIGGER);
        yield return DelayPickupFailed();
    }

    IEnumerator DelayPickupFailed()
    {
        yield return new WaitForSeconds(pickUpFailedDelay);
        OnPickUpFailed?.Invoke(this, EventArgs.Empty);
        Hide();
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected) { return; }
        collected = true;
        OnCollected?.Invoke(this, EventArgs.Empty);
    }

    public override void HandleGridParentDeath()
    {
        StartCoroutine(DelayPickupFailed());
    }

    public override void HandleGridParentIsSafeChange(bool isSafe)
    {
        if (isSafe) { return; }
        StartCoroutine(DelayPickupFailed());
    }
}
