using System.Collections;
using OopAndArchitecture.Projectiles;
using UnityEngine;
using UnityEngine.Assertions;

namespace OopAndArchitecture.Weapons {
    /// <summary>
    /// Weapon B - в этом прототипе это Пулемёт. Космический пулемёт, который стреляет одной пулей :)
    /// </summary>
    public class MachineGunWeapon : Weapon {
        [SerializeField] private MachineGunBullet m_bulletPrefab;
        
        
        protected override void Awake() {
            base.Awake();
            Assert.IsNotNull(m_bulletPrefab, "m_bulletPrefab != null");
        }

        protected override IEnumerator Shoot() {
            while (true) {
                // Ждём высчитанное число времени перезарядки
                yield return new WaitForSeconds(m_data.ReloadTime * m_data.ReloadTimeModifier);
                
                // Стреляем пулей
                MachineGunBullet bullet = Instantiate(m_bulletPrefab, m_spawnLocation.position, m_spawnLocation.rotation);
                bullet.SetDamageData(m_data);
                bullet.Fly();
            }
        }
    }
}