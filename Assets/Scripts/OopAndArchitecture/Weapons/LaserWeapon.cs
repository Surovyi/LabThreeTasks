using System.Collections;
using OopAndArchitecture.Interfaces;
using UnityEngine;
using UnityEngine.Assertions;

namespace OopAndArchitecture.Weapons {
    /// <summary>
    /// Weapon B - в этом прототипе это Лазерное оружие.
    /// </summary>
    public class LaserWeapon : Weapon {
        [SerializeField] private LineRenderer m_lineRenderer;
        [SerializeField] private float m_laserSpawnDuration;

        private float m_laserLifetime;
        
        
        protected override void Awake() {
            base.Awake();
            Assert.IsNotNull(m_lineRenderer, "m_lineRenderer != null");
        }
        
        protected override IEnumerator Shoot() {
            const float laserMaxDistance = 50f;
            
            // Производим выстрелы с заданным временем перезарядки.
            // Стреляем пока корутину не уничтожат.
            while (true) {
                // Ждём высчитанное число времени перезарядки
                yield return new WaitForSeconds(m_data.ReloadTime * m_data.ReloadTimeModifier);
            
                Vector3 direction = m_spawnLocation.forward;
                Physics.Raycast(m_spawnLocation.position, direction, out RaycastHit hit);

                // Если на пути лазера что-то есть физическое, то "врезаемся" в это что-то
                Vector3 endPos = (m_spawnLocation.position + direction) * laserMaxDistance;
                if (hit.collider != null) {
                    endPos = hit.point;
                
                    // А если это физическое еще и реализует интерфейс ITargetable, то мы можем применить урон этому объекту
                    if (hit.transform.GetComponent(typeof(ITargetable)) is ITargetable target) {
                        target.ApplyDamage(m_data.Damage);
                    }
                }
            
                // Визуально рисуем лазер
                m_lineRenderer.SetPositions(new []{m_spawnLocation.position, endPos });
                m_lineRenderer.enabled = true;

                m_laserLifetime = m_laserSpawnDuration;
                StartCoroutine(Laser());
            }
        }

        /// <summary>
        /// Прекращаем стрельбу
        /// </summary>
        protected override void StopShooting() {
            m_lineRenderer.enabled = false;
            m_laserLifetime = 0f;
        }

        private IEnumerator Laser() {
            while (m_laserLifetime > 0f) {
                yield return null;
                m_laserLifetime -= Time.deltaTime;
            }

            m_lineRenderer.enabled = false;
        }
    }
}