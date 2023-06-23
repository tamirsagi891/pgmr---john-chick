using System;
using System.Collections.Generic;
using BitStrap;
using Elad.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace Elad.Scripts
{
    [RequireComponent(typeof(HorizontalMovement), 
        typeof(TouchingDirection))]
    public class EggsManager : MonoBehaviour
    {
        
        [Space(10)] [Header("Eggs Attack")]
        [SerializeField]
        private float eggAttackCoolDownTime = 1f;
        
        public enum EggKind
        {
            White,
            Red,
            Blue
        }


        [SerializeField] private EggKind curEggKind = EggKind.White;

        [SerializeField] [InlineScriptableObject]
        private EggDataList EggList;

        private List<EggData> EggAttacksList => EggList.eggDataList;
        
        private Dictionary<EggKind, EggData> _EggDataDic;
        public EggKind CurEggKind
        {
            get => curEggKind;
            set
            {
                curEggKind = value;
                PlayerStatus.CurrentEggData = _EggDataDic[value];
            }
        }

        

        private Dictionary<EggKind, int> _EggAmountDic;

        private float _EggAttackCoolDownTimer = 0;
        private bool _EggAttackCoolDown = false;
        [SerializeField] private GameObject EggInstantiatePosition;
        [SerializeField] private GameObject EggPrefab;
        private LinkedPool<Egg> _EggPool;
        [SerializeField] private int _maxPoolSize = 100;
        [SerializeField] private bool usePoolEgg = true;

        public bool UsePoolEgg
        {
            get => usePoolEgg;
            set => usePoolEgg = value;
        }
        
        [SerializeField] private Vector2 knockBackEgg = Vector2.zero;
        

        [Space(10)] [Header("Component's")] private TouchingDirection _touchingDirection;
        private HorizontalMovement _horizontalMovement;
        private Animator _animator;
        private CharacterJump _characterJump;
        
        
        private void OnEnable()
        {
            characterEvents.AddEgg.AddListener(AddEgg);
            characterEvents.RemoveEgg.AddListener(RemoveEgg);
        }

        private void OnDisable()
        {
            characterEvents.AddEgg.RemoveListener(AddEgg);
            characterEvents.RemoveEgg.RemoveListener(RemoveEgg);
        }
        
        
        private void Awake()
        {
            _characterJump = GetComponent<CharacterJump>();
            _horizontalMovement = GetComponent<HorizontalMovement>();
            _touchingDirection = GetComponent<TouchingDirection>();
            _animator = GetComponent<Animator>();
            DictionaryInit();
            SetCurrentEgg(_EggDataDic[EggKind.White]);
            InitPool();
        }

        private void Update()
        {
            if (_EggAttackCoolDown)
            {
                _EggAttackCoolDownTimer -= Time.deltaTime;
                if (_EggAttackCoolDownTimer < 0)
                {
                    _EggAttackCoolDown = false;
                }
            }
        }

        private void InitPool()
        {
            if (usePoolEgg)
            {
                _EggPool = new LinkedPool<Egg>(() => Instantiate(EggPrefab,
                        EggInstantiatePosition.transform.position,
                        EggPrefab.transform.rotation).GetComponent<Egg>(),
                    GetEgg,
                    Egg => Egg.gameObject.SetActive(false),
                    Egg => Destroy(Egg.gameObject),
                    false,
                    _maxPoolSize
                );
            }
        }

        
        public void OnEggAttack(InputAction.CallbackContext context)
        {
            if (context.started && !_EggAttackCoolDown && CanThrow())
            {
                // _animator.SetTrigger(AnimationStrings.eggAttack);
                _EggAttackCoolDownTimer = eggAttackCoolDownTime;
                _EggAttackCoolDown = true;
                //TODO:: make an animation and call the fire egg from animation
                FireEgg();
            }
        }

        private bool CanThrow()
        {
            var returnValue = _characterJump.IsGliding;
            return returnValue;
        }
        
        private void GetEgg(Egg Egg)
        {
            if (Egg)
            {
                Egg.gameObject.SetActive(true);
                Egg.transform.position = EggInstantiatePosition.transform.position;
                Egg.MyEggData = PlayerStatus.CurrentEggData;
                Egg.eggsManager = this;    
            }
            
        }

        private void DictionaryInit()
        {
            _EggDataDic = new Dictionary<EggKind, EggData>();
            foreach (var Egg in EggAttacksList)
            {
                _EggDataDic[Egg.eggKind] = Egg;
            }
        }

        public void AddEgg(EggKind EggKind)
        {
            if (_EggAmountDic.ContainsKey(EggKind))
            {
                _EggAmountDic[EggKind] += 1;
            }

            else
            {
                _EggAmountDic[EggKind] = 1;
            }
        }

        public void RemoveEgg(EggKind EggKind)
        {
            int curAmount = _EggAmountDic[EggKind];
            if (curAmount > 0)
            {
                _EggAmountDic[EggKind] -= 1;
            }
        }

        public int HowMany(EggKind EggKind)
        {
            int curAmount = _EggAmountDic[EggKind];
            return curAmount;
        }

        public int ReturnCurDamage()
        {
            return _EggDataDic[curEggKind].damage;
        }

        private void SetCurrentEgg(EggData curEggDataData)
        {
            PlayerStatus.CurrentEggData = curEggDataData;
        }

        // Called from the animator
        public void FireEgg()
        {
            Egg Egg;
            if (UsePoolEgg)
            {
                Egg = _EggPool.Get();
            }

            else
            {
                Egg = Instantiate(EggPrefab, EggInstantiatePosition.transform.position,
                    EggPrefab.transform.rotation).GetComponent<Egg>();
            }

            if (Egg)
            {
                Egg.eggsManager = this;
                Egg.transform.position = EggInstantiatePosition.transform.position;
                Egg.Fire();   
                _horizontalMovement.OnHit(0, knockBackEgg);
            }

            
        }

        public bool ReturnEggToPoll(Egg Egg)
        {
            if (UsePoolEgg)
            {
                _EggPool.Release(Egg);
                return true;
            }

            return false;
        }
    }
}