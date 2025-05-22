using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class ReloadUIEnemy : MonoBehaviour, IReloadSubscriber
{
    public Transform follow_target;
    public RectTransform reload_ui; 
    public Vector3 offset = new Vector3(50f, -75f, 0f);
    public Image progress_bar;
    private IReloadManager manager;

    public TMP_Text text;

    private bool reloading = false;
    
    void Start()
    {
        manager = follow_target.gameObject.GetComponent<IReloadManager>();
        manager.Subscribe(this);
        ClearUI();
    }
    
    void OnDestroy() {
        manager.Unsubscribe(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (reloading) {
            UpdateProgress();
            UpdatePosition();
        }
    }

    private void UpdatePosition() {
        Vector3 new_position = Camera.main.WorldToScreenPoint(follow_target.position);
        reload_ui.position = new_position + offset;
    }

    private void UpdateProgress() {
        progress_bar.fillAmount = manager.reload_progress;
    }
    
    public void StartReload(IReloadManager manager, IFirearm weapon) {
        reloading = true;
        text.text = "reloading";
        UpdateProgress();
    }

    public void ReloadFinished(IReloadManager manager, IFirearm weapon) {
        ClearUI();
    }

    public void ReloadCancelled(IReloadManager manager, IFirearm weapon) {
        ClearUI();
    }

    private void ClearUI() {
        reloading = false;
        progress_bar.fillAmount = 0f;
        text.text = "";
    }

}
