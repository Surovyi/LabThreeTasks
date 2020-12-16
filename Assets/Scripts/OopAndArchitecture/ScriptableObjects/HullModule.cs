using OopAndArchitecture.Data;
using OopAndArchitecture.Enums;
using OopAndArchitecture.Interfaces;
using UnityEngine;

namespace OopAndArchitecture.ScriptableObjects {
    [CreateAssetMenu(fileName = "HullModule", menuName = "Ship/Hull AttachedModule")]
    public class HullModule : ScriptableObject, IHullModule {
        public string Id => m_id;
        public string Description => m_description;
        public AppliedEffect Effect => m_effect;
        public float Amount => m_amount;
        
        
        [SerializeField] private string m_id;
        [SerializeField] private string m_description;
        [SerializeField] private AppliedEffect m_effect;
        [SerializeField] private float m_amount;
        [SerializeField] private GameObject m_modulePrefab;
        

        private GameObject m_spawnedModule;
        
        /// <summary>
        /// Физический спаун объекта
        /// </summary>
        /// <param name="shipData">Контейнер с данными о корабле на котором спаунится модуль корпуса</param>
        /// <param name="parent"></param>
        public void Spawn(ShipData shipData, Transform parent) {
            m_spawnedModule = Instantiate(m_modulePrefab, parent);
        }

        /// <summary>
        /// Уничтожить заспауненный объект
        /// </summary>
        public void Remove() {
            if (m_spawnedModule != null) {
                Destroy(m_spawnedModule);
            }
        }
    }
}