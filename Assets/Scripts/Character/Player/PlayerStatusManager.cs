using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのステータスを管理するビヘイビア
/// </summary>
public class PlayerStatusManager : StatusManager
{
    // HPUI処理イベント
    public delegate void HPBarEventHandler(float currentHealth);
    public static event HPBarEventHandler OnUpdateHPBarEvent;

    //PlayerControllerの参照
    PlayerController playerController;

    [SerializeField,Header("プレイヤーデータ")]
    PlayerStatusData playerStatusData;

    //暴走
    //****************
    [SerializeField, Header("暴走値")]
    float frenzyValue = 0;

    //デフォルトのサイズ
    UnityEngine.Vector3 defaultSize;

    float frenzyTimer = 0;

    //暴走しているか
    bool isFrenzy = false;

    //被撃したか
    bool isHit = false;
    //****************

    protected override void Awake()
    {
        base.Awake();        
    }

    private void OnEnable()
    {
        //イベントをバインドする
        OnomatoManager.OnIncreaseFrenzyEvent += IncreaseFrenzy;
    }

    private void OnDisable()
    {
        //バインドを解除する
        OnomatoManager.OnIncreaseFrenzyEvent -= IncreaseFrenzy;
    }

    public void Init(PlayerController _playerController)
    {
        playerController = _playerController;

        defaultSize = playerController.transform.localScale;
    }

    private void Update()
    {
        //暴走の仮処理
        if (frenzyValue >= playerStatusData.maxFrenzyGauge)
        {
            frenzyValue = 0;
         //   playerController.transform.localScale = 1.5f * defaultSize;

            frenzyTimer = playerStatusData.frenzyTime;
            isFrenzy = true;

            //コライダー設定
            if (playerController.RageCollider)
            {
                playerController.RageCollider.CanHit = true;
            }
        }

        if (frenzyTimer > 0 && isFrenzy)
        {
            frenzyTimer -= Time.deltaTime;
        }

        //暴走終了
        if (frenzyTimer <= 0 && isFrenzy)
        {
            playerController.transform.localScale = defaultSize;
            isFrenzy = false;

            //コライダーリセット
            if (playerController.RageCollider)
            {
                playerController.RageCollider.Reset();
                playerController.RageCollider.CanHit = false;
            }
           
        }
    }

    public override void TakeDamage(float _damage)
    {
        base.TakeDamage(_damage);
        //HPUI更新イベントを送信
        OnUpdateHPBarEvent?.Invoke(currentHealth);
    }

    /// <summary>
    /// 暴走ゲージを溜める
    /// </summary>
    public void IncreaseFrenzy(float _amount)
    {
        if(!isFrenzy)frenzyValue += _amount;
    }

    public new PlayerStatusData StatusData
    {
        get => playerStatusData;
    }

    public float FrenzyValue
    {
        get => frenzyValue;
    }

    public bool IsFrenzy
    {
        get => isFrenzy;
    }

    public bool IsHit
    {
        get => isHit;
        set { isHit = value; }
    }

    public float FrenzyTimer
    {
        get => frenzyTimer;
    }
}
