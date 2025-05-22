using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class ReloadUI : MonoBehaviour, IReloadSubscriber, IPlayerObserver
{
    public RectTransform reload_ui; 
    public Vector3 mouse_offset = new Vector3(50f, -75f, 0f);
    public Image progress_bar;
    private IReloadManager manager;

    public TMP_Text text;

    private bool reloading = false;
    
    void Start()
    {
        NewPlayerObject(PlayerCharacter.inst.GetPlayerCombat(this));
    }
    
    public void NewPlayerObject(PlayerCombat player) {
        if (player == null) {
            Debug.Log("new player is null!");
            return;
        }
        manager = player.reloading;    
        manager.Subscribe(this);
        ClearUI();
    }

    void OnDestroy() {
        manager.Unsubscribe(this);
        PlayerCharacter.inst.UnsubscribeFromPlayer(this);
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
        Vector3 mouse = InputSystem.current.MouseScreenPosition();
        reload_ui.position = mouse + mouse_offset;
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
