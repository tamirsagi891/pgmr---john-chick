using System;
using System.Collections.Generic;
using BitStrap;
using Elad.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace Elad.Scripts
{
    public class FeathersManager : MonoBehaviour
    {
        public enum FeatherKind
        {
            White,
            Red,
            Blue
        }


        [SerializeField] private FeatherKind curFeatherKind = FeatherKind.White;

        [SerializeField] [InlineScriptableObject]
        private ArrowDataList arrowList;

        private List<ArrowData> arrowAttacksList => arrowList.arrowAttacksList;
        // [SerializeField] private List<ArrowData> arrowAttacksList;

        private Animator _animator;
        private Dictionary<FeatherKind, ArrowData> _arrowDataDic;

        public FeatherKind CurFeatherKind
        {
            get => curFeatherKind;
            set
            {
                curFeatherKind = value;
                PlayerStatus.CurrentArrowDataData = _arrowDataDic[value];
            }
        }

        private void OnEnable()
        {
            characterEvents.AddFeather.AddListener(AddFeather);
            characterEvents.RemoveFeather.AddListener(RemoveFeather);
        }

        private void OnDisable()
        {
            characterEvents.AddFeather.RemoveListener(AddFeather);
            characterEvents.RemoveFeather.RemoveListener(RemoveFeather);
        }

        private Dictionary<FeatherKind, int> _featherAmountDic;


        [Space(10)] [Header("Arrow Attack")] [SerializeField]
        private float arrowAttackCoolDownTime = 1f;

        private float _arrowAttackCoolDownTimer = 0;
        private bool _arrowAttackCoolDown = false;
        [SerializeField] private GameObject arrowInstantiatePosition;
        [SerializeField] private GameObject arrowPrefab;
        private LinkedPool<Arrow> _arrowPool;
        private int _startPoolSize = 50;
        [SerializeField] private int _maxPoolSize = 100;
        [SerializeField] private bool usePoolArrow = true;

        public bool UsePoolArrow
        {
            get => usePoolArrow;
            set => usePoolArrow = value;
        }

        private void Awake()
        {
            _animator = PlayerStatus.player.GetComponent<Animator>();
            DictionaryInit();
            SetCurrentFeather(_arrowDataDic[FeatherKind.White]);
            InitPool();
        }

        private void Update()
        {
            if (_arrowAttackCoolDown)
            {
                _arrowAttackCoolDownTimer -= Time.deltaTime;
                if (_arrowAttackCoolDownTimer < 0)
                {
                    _arrowAttackCoolDown = false;
                }
            }
        }

        private void InitPool()
        {
            if (usePoolArrow)
            {
                _arrowPool = new LinkedPool<Arrow>(() => Instantiate(arrowPrefab,
                        arrowInstantiatePosition.transform.position,
                        arrowPrefab.transform.rotation).GetComponent<Arrow>(),
                    GetArrow,
                    arrow => arrow.gameObject.SetActive(false),
                    arrow => Destroy(arrow.gameObject),
                    false,
                    _maxPoolSize
                );
            }
        }

        public void OnArrowAttack(InputAction.CallbackContext context)
        {
            if (context.started && !_arrowAttackCoolDown)
            {
                _animator.SetTrigger(AnimationStrings.arrowAttack);
                _arrowAttackCoolDownTimer = arrowAttackCoolDownTime;
                _arrowAttackCoolDown = true;
            }
        }

        private void GetArrow(Arrow arrow)
        {
            if (arrow)
            {
                arrow.gameObject.SetActive(true);
                arrow.transform.position = arrowInstantiatePosition.transform.position;
                arrow.MyArrowData = PlayerStatus.CurrentArrowDataData;
                arrow.FeathersManager = this;    
            }
            
        }

        private void DictionaryInit()
        {
            _arrowDataDic = new Dictionary<FeatherKind, ArrowData>();
            foreach (var arrow in arrowAttacksList)
            {
                _arrowDataDic[arrow.featherKind] = arrow;
            }
        }

        public void AddFeather(FeatherKind featherKind)
        {
            if (_featherAmountDic.ContainsKey(featherKind))
            {
                _featherAmountDic[featherKind] += 1;
            }

            else
            {
                _featherAmountDic[featherKind] = 1;
            }
        }

        public void RemoveFeather(FeatherKind featherKind)
        {
            int curAmount = _featherAmountDic[featherKind];
            if (curAmount > 0)
            {
                _featherAmountDic[featherKind] -= 1;
            }
        }

        public int HowMany(FeatherKind featherKind)
        {
            int curAmount = _featherAmountDic[featherKind];
            return curAmount;
        }

        public int ReturnCurDamage()
        {
            return _arrowDataDic[curFeatherKind].damage;
        }

        private void SetCurrentFeather(ArrowData curArrowDataData)
        {
            PlayerStatus.CurrentArrowDataData = curArrowDataData;
        }

        // Called from the animator
        public void FireArrow()
        {
            Arrow arrow;
            if (UsePoolArrow)
            {
                arrow = _arrowPool.Get();
                print(arrow);
            }

            else
            {
                arrow = Instantiate(arrowPrefab, arrowInstantiatePosition.transform.position,
                    arrowPrefab.transform.rotation).GetComponent<Arrow>();
            }

            if (arrow)
            {
                arrow.FeathersManager = this;
                arrow.transform.position = arrowInstantiatePosition.transform.position;
                arrow.Fire();    
            }

            
        }

        public bool ReturnArrowToPoll(Arrow arrow)
        {
            if (UsePoolArrow)
            {
                _arrowPool.Release(arrow);
                return true;
            }

            return false;
        }
    }
}