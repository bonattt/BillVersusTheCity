using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class ReloadUI : MonoBehaviour, IReloadSubscriber
{
    public GameObject target;
    public Image progress_bar;
    private IReloadManager manager;

    public TMP_Text text;

    private bool reloading = false;
    
    void Start()
    {
        manager = target.GetComponent<IReloadManager>();    
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
        }
    }

    private void UpdateProgress() {
        progress_bar.fillAmount = manager.reload_progress;
    }
    
    public void StartReload(IReloadManager manager, IWeapon weapon) {
        reloading = true;
        text.text = "reloading";
        UpdateProgress();
    }

    public void FinishReload(IReloadManager manager, IWeapon weapon) {
        ClearUI();
    }

    public void CancelReload(IReloadManager manager, IWeapon weapon) {
        ClearUI();
    }

    private void ClearUI() {
        reloading = false;
        progress_bar.fillAmount = 0f;
        text.text = "";
    }

}
