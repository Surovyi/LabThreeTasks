using System.Collections;
using OopAndArchitecture.Interfaces;
using OopAndArchitecture.ScriptableObjects.Managers;
using UnityEngine;
using UnityEngine.Assertions;

namespace OopAndArchitecture.Projectiles {
    /// <summary>
    /// Реализация снаряда для "Weapon A".
    /// </summary>
    [DisallowMultipleComponent]
    public class MachineGunBullet : MonoBehaviour, IProjectile, IManagedEventListener, IManagedEventListenerWithParams {
        [Header("Events")]
        [SerializeField] private ManagedEvent m_resetGame;
        [SerializeField] private ManagedEventWithParams m_shipDestroyed;
        [Header("Params")]
        [SerializeField] private float m_bulletSpeed;
        [SerializeField] private float m_bulletMaxFlyDistance;

        private IWeaponModule m_weaponModule;
        private float m_traveledDistance;
        private Vector3 m_lastFramePosition;


        /// <summary>
        /// Сохраняем данные для нанесения урона в будущем
        /// </summary>
        /// <param name="weaponModule">Реализация модуля оружия</param>
        public void SetDamageData(IWeaponModule weaponModule) {
            m_weaponModule = weaponModule;
        }

        /// <summary>
        /// Команда - старт полета
        /// </summary>
        public void Fly() {
            m_lastFramePosition = transform.position;
            
            // Подписываемся на перезапуск игры и уничтожение одного из кораблей
            m_resetGame.AddListener(this);
            m_shipDestroyed.AddListener(this);
            
            StartCoroutine(Flying());
        }

        /// <summary>
        /// Применение урона встреченному на пути объекту, способному получать урон
        /// </summary>
        /// <param name="target"></param>
        public void ApplyDamage(ITargetable target) {
            target.ApplyDamage(m_weaponModule.Damage);
        }

        /// <summary>
        /// Обрабатываем пользовательские события
        /// </summary>
        /// <param name="eventId"></param>
        public void EventTrigger(string eventId) {
            if (eventId == m_resetGame.Id) {
                DestroyObject();
            }
        }
        
        /// <summary>
        /// Обрабатываем пользовательские события с параметрами
        /// </summary>
        /// <param name="eventId"></param>
        public void EventTrigger(string eventId, IManagedEventParams param) {
            if (eventId == m_shipDestroyed.Id) {
                DestroyObject();
            }
        }
        
        /// <summary>
        /// Обрабатываем пользовательские события с параметрами
        /// </summary>
        private IEnumerator Flying() {
            // Летим, пока нам позволяет максимальная дистанция полета
            while (m_traveledDistance < m_bulletMaxFlyDistance) {
                transform.position += transform.forward * (m_bulletSpeed * Time.deltaTime);
                m_traveledDistance += (transform.position - m_lastFramePosition).magnitude;
                m_lastFramePosition = transform.position;
                yield return null;
            }

            // Уничтожаем снаряд, если до этого он ни с чем не столкнулся
            DestroyObject();
        }

        /// <summary>
        /// Обрабатываем события коллизии с другими объектами
        /// </summary>
        /// <param name="other"></param>
        private void OnCollisionEnter(Collision other) {
            if (other.transform.GetComponent(typeof(ITargetable)) is ITargetable target) {
                ApplyDamage(target);
                DestroyObject();
            }
        }

        /// <summary>
        /// Уничтожить этот объект и отписаться от событий
        /// </summary>
        private void DestroyObject() {
            m_resetGame.RemoveListener(this);
            m_shipDestroyed.RemoveListener(this);
            Destroy(gameObject);
        }
        
        private void Awake() {
            Assert.IsNotNull(m_resetGame, "m_resetGame != null");
            Assert.IsNotNull(m_shipDestroyed, "m_shipDestroyed != null");
        }
    }
}