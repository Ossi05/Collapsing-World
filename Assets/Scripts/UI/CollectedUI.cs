using System;
using TMPro;
using UnityEngine;

public class CollectedUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI collectedText;
    [SerializeField] bool showOnlyWhenGamePlaying = true;
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
        int collectedAmt = CollectableManager.Instance.GetCollectedAmt();
        collectedText.text = $"{collectedAmt}/{CollectableManager.Instance.GetMaxCollectAmt()}";
    }

    void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        UpdateText();
        if (!showOnlyWhenGamePlaying) { return; }
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
