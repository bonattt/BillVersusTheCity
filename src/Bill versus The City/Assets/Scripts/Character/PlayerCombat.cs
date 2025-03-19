
using UnityEngine;

public class PlayerCombat : MonoBehaviour  {
    /**
      * Script that does nothing but contain references to other player scripts
      */
    
    private bool _is_active = true;
    public bool is_active {
        get { return _is_active; }
        set {
            _is_active = value;
            // TODO ---
            movement.is_active = value;
            attacks.is_active = value;
            reloading.is_active = value;
            interactor.is_active = value;
        }
    }

    private PlayerMovement _movement = null;
    public PlayerMovement movement {
        get {
            if (_movement == null) {
                _movement = GetComponent<PlayerMovement>();
            }
            return _movement;
        }
    }

    private PlayerAttackController _attacks = null;
    public PlayerAttackController attacks {
        get {
            if (_attacks == null) {
                _attacks = GetComponent<PlayerAttackController>();
            }
            return _attacks;
        }
    }

    private IReloadManager _reloading = null;
    public IReloadManager reloading {
        get {
            if (_reloading == null) {
                _reloading = GetComponent<IReloadManager>();
            }
            return _reloading;
        }
    }

    private AmmoContainer _reload_ammo = null;
    public AmmoContainer reload_ammo {
        get {
            if (_reload_ammo == null) {
                _reload_ammo = GetComponent<AmmoContainer>();
            }
            return _reload_ammo;
        }
    }
    
    private CharacterController _character_controller = null;
    public CharacterController character_controller {
        get {
            if (_character_controller == null) {
                _character_controller = GetComponent<CharacterController>();
            }
            return _character_controller;
        }
    }

    private CharacterStatus _status = null;
    public CharacterStatus status {
        get {
            if (_status == null) {
                _status = GetComponent<CharacterStatus>();
            }
            return _status;
        }
    }

    [SerializeField]
    private PlayerInteractor _interactor = null;
    public PlayerInteractor interactor {
        get {
            if (_interactor == null) {
                _interactor = GetComponent<PlayerInteractor>();
            }
            return _interactor;
        }
    }

    private AmmoContainer _ammo = null;
    public AmmoContainer ammo {
        get {
            if (_ammo == null) {
                _ammo = GetComponent<AmmoContainer>();
            }
            return _ammo;
        }
    }

    void Start() {
        // add to PlayerCharacter
        status.is_player = true;
        PlayerCharacter.inst.PlayerUpdated(this);
    }
}