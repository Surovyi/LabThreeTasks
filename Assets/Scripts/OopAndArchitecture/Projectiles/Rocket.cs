using System.Collections;
using OopAndArchitecture.Interfaces;
using OopAndArchitecture.ScriptableObjects.Managers;
using UnityEngine;
using UnityEngine.Assertions;

namespace OopAndArchitecture.Projectiles {
    /// <summary>
    /// Реализация снаряда для "Weapon C".
    /// </summary>
    [DisallowMultipleComponent]
    public class Rocket : MonoBehaviour, IProjectile, IManagedEventListener, IManagedEventListenerWithParams {
        [Header("Events")]
        [SerializeField] private ManagedEvent m_resetGame;
        [SerializeField] private ManagedEventWithParams m_shipDestroyed;
        [Header("Params")]
        [SerializeField] private float m_rocketSpeed;
        [SerializeField] private float m_rocketMaxFlyDistance;
        [SerializeField] private AnimationCurve m_accelerationCurve;

        
        private ITargetable m_target;
        private IWeaponModule m_weaponModule;
        private bool m_targetAquired;
        private float m_traveledDistance;
        private Vector3 m_tangetBezier;
        private Vector3 m_startLocation;
        private Vector3 m_endLocation;

        /// <summary>
        /// Сохраняем данные для нанесения урона в будущем
        /// </summary>
        /// <param name="weaponModule"></param>
        public void SetDamageData(IWeaponModule weaponModule) {
            m_weaponModule = weaponModule;
        }

        /// <summary>
        /// Предполагаем что это ракета с самонаведением.
        /// Сохраняем зафиксированную для атаки цель.
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(ITargetable target) {
            m_target = target;
            m_targetAquired = m_target != null;
        }

        /// <summary>
        /// Команда - старт полета
        /// </summary>
        public void Fly() {
            const float tangentDistance = 4f;
            
            // Подписываемся на перезапуск игры и уничтожение одного из кораблей
            m_resetGame.AddListener(this);
            m_shipDestroyed.AddListener(this);
            
            // Определяем все необходимые для полета переменные
            m_startLocation = transform.position;
            m_endLocation = m_targetAquired ? m_target.Location.position : m_startLocation + transform.forward * m_rocketMaxFlyDistance;
            
            if (m_targetAquired == true) {
                Vector3 randomnez = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), tangentDistance);
                m_tangetBezier = transform.position + randomnez * transform.forward.z;
            } else {
                m_tangetBezier = (m_startLocation + m_endLocation) / 2f;
            }

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
        /// Обрабатываем пользовательские события с параметрами
        /// </summary>
        private IEnumerator Flying() {
            float t = 0f;
            float flyTime = 0f;
            float endTime = (m_endLocation - m_startLocation).magnitude / m_rocketSpeed;

            while (t < 1f) {
                
                // Применяем посчитанную позицию к объекту
                Vector3 newPosition = BezierInterpolation(t);
                transform.forward = newPosition - transform.position;
                transform.position = newPosition;

                yield return null;
                // Интерполируем время
                flyTime += Time.deltaTime;
                t = flyTime / endTime;
            }
            
            // По достижению цели, уничтожаем снаряд
            DestroyObject();
        }

        /// <summary>
        /// Интерполяция при помощи кривой Безье с одним тангентом
        /// </summary>
        /// <param name="t">Нормализованное время полета</param>
        /// <returns></returns>
        private Vector3 BezierInterpolation(float t) {
            t = Mathf.Clamp01(t);

            if (m_accelerationCurve != null) {
                t = m_accelerationCurve.Evaluate(t);
            }
            
            return (1f - t) * (1f - t) * m_startLocation + 2 * t * (1f - t) * m_tangetBezier + t * t * m_endLocation;
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