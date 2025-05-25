using System;
using System.Collections;
using UnityEngine;

public class StartingGrid : MonoBehaviour {
    float hideDelay = 10;
    Rigidbody rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            rb.isKinematic = false;
            StartCoroutine(HideObject());
        }
    }

    IEnumerator HideObject()
    {
        yield return new WaitForSeconds(hideDelay);
        gameObject.SetActive(false);
    }
}
