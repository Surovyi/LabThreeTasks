using OopAndArchitecture.Data;
using OopAndArchitecture.Interfaces;
using OopAndArchitecture.Weapons;
using UnityEngine;

namespace OopAndArchitecture.ScriptableObjects {
    [CreateAssetMenu(fileName = "WeaponModule", menuName = "Ship/Weapon AttachedModule")]
    public class WeaponModule : ScriptableObject, IWeaponModule {
        public string Id => m_id;
        public string ShipId => m_assignedShipId;
        public string Description => m_description;
        public float ReloadTime => m_reloadTime;
        public float ReloadTimeModifier => m_reloadTimeModifier;
        public float Damage => m_damage;
        public Weapon Weapon => m_spawnedWeapon;
        
        
        [SerializeField] private string m_id;
        [SerializeField] private string m_description;
        [SerializeField] private float m_reloadTime;
        [SerializeField] private float m_damage;
        [SerializeField] private Weapon m_weaponPrefab;
        
        
        private Weapon m_spawnedWeapon;
        private string m_assignedShipId;
        private float m_reloadTimeModifier;
        
        /// <summary>
        /// Физический спаун объекта
        /// </summary>
        /// <param name="shipData">Контейнер с данными о корабле на котором спаунится пушка</param>
        /// <param name="parent"></param>
        public void Spawn(ShipData shipData, Transform parent) {
            m_assignedShipId = shipData.Id;
            m_spawnedWeapon = Instantiate(m_weaponPrefab, parent);
            m_spawnedWeapon.Init(this);
        }

        public void SetReloadTimeModifier(float modifier) {
            m_reloadTimeModifier = modifier;
        }

        public void Remove() {
            if (m_spawnedWeapon != null) {
                Destroy(m_spawnedWeapon.gameObject);
                m_assignedShipId = null;
            }
        }
    }
}
