using System.Collections;
using OopAndArchitecture.Interfaces;
using OopAndArchitecture.ScriptableObjects;
using OopAndArchitecture.ScriptableObjects.Managers;
using UnityEngine;
using UnityEngine.Assertions;

namespace OopAndArchitecture.Weapons {
    /// <summary>
    /// Базовый класс для оружия физически установленного на корабль
    /// </summary>
    public abstract class Weapon : MonoBehaviour, IManagedEventListener {
        [SerializeField] protected Transform m_spawnLocation;
        
        [Header("Events")]
        [SerializeField] private ManagedEvent m_startShooting;
        [SerializeField] private ManagedEvent m_stopShooting;
        
        protected WeaponModule m_data;
        private Coroutine m_shooting;

        /// <summary>
        /// Инициализация оружия его параметрами
        /// </summary>
        /// <param name="weaponModule"></param>
        public virtual void Init(WeaponModule weaponModule) {
            m_data = weaponModule;
            
            // сразу подписываемся на интересующие оружие события
            m_startShooting.AddListener(this);
            m_stopShooting.AddListener(this);
        }

        /// <summary>
        /// Срабатывание пользовательского события.
        /// </summary>
        /// <param name="eventId"></param>
        public void EventTrigger(string eventId) {
            // Обрабатываем старт стрельбы
            if (eventId == m_startShooting.Id) {
                if (m_shooting == null) {
                    m_shooting = StartCoroutine(Shoot());
                }
                return;
            } 
            // Обрабатываем завершение стрельбы
            if (eventId == m_stopShooting.Id) {
                StopShooting();
                if (m_shooting != null) {
                    StopCoroutine(m_shooting);
                    m_shooting = null;
                }
            }
        }
        
        /// <summary>
        /// Любое оружие должно стрелять.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerator Shoot();

        /// <summary>
        /// Но не каждое взведенное оружие можно заставить прекратить стрелять
        /// </summary>
        protected virtual void StopShooting() { }

        protected virtual void Awake() {
            Assert.IsNotNull(m_spawnLocation, "m_spawnLocation != null");
            Assert.IsNotNull(m_startShooting, "m_startShooting != null");
            Assert.IsNotNull(m_stopShooting, "m_stopShooting != null");
        }
        
        protected virtual void OnDestroy() {
            m_startShooting.RemoveListener(this);
            m_stopShooting.RemoveListener(this);
            m_data = null;
        }
    }
}