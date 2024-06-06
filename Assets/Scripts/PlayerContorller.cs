using System;
using System.Collections;
using Bases;
using Interface;
using Managers;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun, IPunObservable, IDamageable
{
    private Camera _camera;
    
    [Header("Status")]
    private Vector3 _moveDirection;
    private Vector3 _bodyDirection; //dash 및 rotation
    public float moveSpeed = 4f;
    [SerializeField] private float maxHp = 10;
    private float _hp;

    [Header("Children")]
    [SerializeField] private GameObject spinObject;
    [SerializeField] private TextMeshProUGUI playerNameTMP;
    [SerializeField] private Slider playerHpSlider;
    public GameObject particle;
    private Weapon _weapon;
    
    private Vector3 _remotePosition;
    private Quaternion _remoteRotation;
    private Rigidbody2D _rigidBody2D;
    private Collider2D _collider2D;
    private bool _isPause;
    private bool _isDash;

    #region EventHandler

    public Action PlayerDie = null;
    public Action<string> AliveTextSetting = null;

    #endregion
    
    private void Awake()
    {
        _camera = Camera.main;
        _collider2D = GetComponent<Collider2D>();
        _bodyDirection = Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward) * Vector3.up;
        spinObject.transform.localRotation = transform.rotation;
        transform.rotation = Quaternion.identity;

        _weapon = spinObject.GetComponentInChildren<Weapon>();
        _rigidBody2D = GetComponent<Rigidbody2D>();

        _hp = maxHp;
        particle.SetActive(false);
        _weapon.SetPlayer(this);
    }

    private void Start()
    {
        playerNameTMP.text = NetworkManager.GetNickname(photonView.IsMine);
    }

    private void FixedUpdate()
    {
        if(_isPause) return;
        
        Move();
        Rotate();

        playerHpSlider.value = _hp / maxHp;
    }

    #region Move

    public void SetPause(bool pause)
    {
        photonView.RPC(nameof(PauseOtherClient), RpcTarget.All, pause);
    }
    
    [PunRPC]
    private void PauseOtherClient(bool pause)
    {
        _isPause = pause;
        if(pause) _moveDirection = Vector3.zero;
    }

    private void Move()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, _remotePosition, 10 * Time.deltaTime);
        }
        else
        {
            if (_isDash) _moveDirection *= 0.5f;
            transform.position += _moveDirection * (moveSpeed * Time.deltaTime);
        }

        var pos = _camera!.WorldToViewportPoint(transform.position);
        if (pos.x < 0f) pos.x = 0f;
        if (pos.x > 1f) pos.x = 1f;
        if (pos.y < 0f) pos.y = 0f;
        if (pos.y > 1f) pos.y = 1f;
        transform.position = _camera.ViewportToWorldPoint(pos);
    }

    private void Rotate()
    {
        if (!photonView.IsMine)
        {
            spinObject.transform.rotation =
                Quaternion.Lerp(spinObject.transform.rotation, _remoteRotation, 10 * Time.deltaTime);
        }
        else
        {
            var angle = Mathf.Atan2(_bodyDirection.y, _bodyDirection.x) * Mathf.Rad2Deg;
            var targetRotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            spinObject.transform.rotation =
                Quaternion.Lerp(spinObject.transform.rotation, targetRotation, 10 * Time.deltaTime);
        }
    }
    
    #endregion
    
    
    #region PlayerInput

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine || _isPause) return;

        var inputVec = context.ReadValue<Vector2>();
        _moveDirection = new Vector3(inputVec.x, inputVec.y, 0);
        if (inputVec != Vector2.zero) _bodyDirection = _moveDirection;
    }
    
    public void OnSkill(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine || _isPause) return;
        var keyPressed = context.control.displayName;
            
        if (context.started) photonView.RPC(nameof(SkillRPC), RpcTarget.All, keyPressed);
    }
    
    [PunRPC]
    protected void SkillRPC(string keyName)
    {
        var idx = keyName switch
        {
            "Q" => 0,
            "W" => 1,
            "E" => 2,
            _ => 0
        };

        _weapon.ExecuteSkill(idx);
    }
    
    public void Dash(float dashSpeed, float dashDuration)
    {
        if (_bodyDirection == Vector3.zero) return;
        StartCoroutine(IDash(dashSpeed, dashDuration));
    }

    private IEnumerator IDash(float dashSpeed, float dashDuration)
    {
        _isDash = true;
        _moveDirection = Vector3.zero;
        particle.SetActive(true);

        var dashTime = 0f;

        while (dashTime < dashDuration)
        {
            transform.Translate(_bodyDirection * (dashSpeed * Time.deltaTime));
            dashTime += Time.deltaTime;
            yield return null;
        }

        _isDash = false;
        particle.SetActive(false);
    }
    
    #endregion

    #region PlayerDie

    private IEnumerator RotateAndShrink()
    {
        _collider2D.enabled = false;
        _isPause = true;
        float elapsedTime = 0f;
        var initialScale = transform.localScale;
        const float duration = 0.3f;
        
        PlayerDie?.Invoke();
        
        while (elapsedTime < duration)
        {
            var t = elapsedTime / duration;
            
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(100, 100, 100);
        yield return StartCoroutine(PlayerRevive());
    }

    private IEnumerator PlayerRevive()
    {
        for (int i = 5; i > 0; --i)
        {
            AliveTextSetting?.Invoke($"Alive in {i}s...");
            yield return new WaitForSeconds(1f);
        }
        AliveTextSetting?.Invoke("");
        transform.localScale = Vector3.one;
        transform.position = Vector3.zero;
        _hp = maxHp;
        _collider2D.enabled = true;

        _isPause = false;
    }

    #endregion
    
    public void Damaged(float damage, Vector3 position, bool isKnockBack=true)
    {
        
        var knockBack = (transform.position - position).normalized;
        if (isKnockBack) _rigidBody2D.AddForce(knockBack * 10f, ForceMode2D.Impulse);

        if (photonView.IsMine) return;
        photonView.RPC(nameof(DamageRPC), RpcTarget.All, damage);
    }

    [PunRPC]
    private void DamageRPC(float damage)
    {
        _hp -= damage;
        if (_hp <= 0) StartCoroutine(RotateAndShrink());
    }

    public void Healing(float amount) => photonView.RPC(nameof(HealingRPC), RpcTarget.All, amount);
    [PunRPC]
    private void HealingRPC(float amount)
    {
        _hp += amount;
        if (_hp > maxHp) _hp = maxHp;
    }

    public void Cloaking(bool isCloaking)
    {
        if (isCloaking)
        {
            gameObject.GetComponent<SpriteRenderer>().color =
                photonView.IsMine ? new Color(1, 1, 1, 0.1f) : new Color(1, 1, 1, 0f);
        }
        else gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);

        if (!photonView.IsMine)
        {
            playerHpSlider.gameObject.SetActive(!isCloaking);
            playerNameTMP.gameObject.SetActive(!isCloaking);
        }
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(spinObject.transform.rotation);
        }
        else
        {
            _remotePosition = (Vector3)stream.ReceiveNext();
            _remoteRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}