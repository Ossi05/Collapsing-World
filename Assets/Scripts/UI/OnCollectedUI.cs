using System;
using TMPro;
using UnityEngine;

public class OnCollectedUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI collectedNotificationText;
    [SerializeField] Animator animator;

    const string COLLECTED_ANIM_TRIGGER = "Collected";

    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        CollectableManager.Instance.OnCollected += CollectableManager_OnCollected;
        Hide();
    }

    void CollectableManager_OnCollected(object sender, EventArgs e)
    {
        UpdateText();
    }

    void UpdateText()
    {
        animator.ResetTrigger(COLLECTED_ANIM_TRIGGER);
        int collectedAmt = CollectableManager.Instance.GetCollectedAmt();
        collectedNotificationText.text = collectedAmt.ToString();
        animator.SetTrigger(COLLECTED_ANIM_TRIGGER);
    }

    void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

    void Show()
    {
        UpdateText();
        gameObject.SetActive(true);
    }
}
