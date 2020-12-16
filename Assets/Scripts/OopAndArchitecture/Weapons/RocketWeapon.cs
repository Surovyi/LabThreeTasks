using System.Collections;
using OopAndArchitecture.Interfaces;
using OopAndArchitecture.Projectiles;
using UnityEngine;
using UnityEngine.Assertions;

namespace OopAndArchitecture.Weapons {
    /// <summary>
    /// Weapon С - в этом прототипе это Ракетница с самонаводящимися ракетами.
    /// </summary>
    public class RocketWeapon : Weapon {
        [SerializeField] private Rocket m_rocketPrefab;
        [SerializeField] private float m_maxSearchTargetDistance;

        private string m_currentShipId;
        
        
        protected override void Awake() {
            base.Awake();
            Assert.IsNotNull(m_rocketPrefab, "m_rocketPrefab != null");
        }
        
        protected override IEnumerator Shoot() {
            while (true) {
                // Ждём высчитанное число времени перезарядки
                yield return new WaitForSeconds(m_data.ReloadTime * m_data.ReloadTimeModifier);
                
                // Запускаем ракету, задаем ей параметры и цель
                Rocket rocket = Instantiate(m_rocketPrefab, m_spawnLocation.position, m_spawnLocation.rotation);
                rocket.SetDamageData(m_data);
                rocket.SetTarget(SearchForTarget());
                rocket.Fly();
            }
        }

        /// <summary>
        /// Попытка найти "захватываемую" цель.
        /// </summary>
        /// <returns></returns>
        private ITargetable SearchForTarget() {
            const float castRadius = 3f;
            
            ITargetable target = null;
            RaycastHit[] hits = Physics.SphereCastAll(m_spawnLocation.position, castRadius, m_spawnLocation.forward, m_maxSearchTargetDistance);

            foreach (RaycastHit hit in hits) {
                if (hit.transform.GetComponent(typeof(ITargetable)) is ITargetable t && t.Id != m_data.ShipId) {
                    // Повезло, мы нашли цель которую можно "взять в цель"
                    target = t;
                    break;
                }
            }

            return target;
        }
    }
}