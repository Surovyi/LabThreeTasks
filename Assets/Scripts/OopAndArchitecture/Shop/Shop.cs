using System;
using OopAndArchitecture.Enums;
using OopAndArchitecture.Interfaces;
using OopAndArchitecture.ScriptableObjects;
using UnityEngine;

namespace OopAndArchitecture.Shop {
    /// <summary>
    /// Коллекция модулей приобретаемых на корабль.
    /// </summary>
    [CreateAssetMenu(fileName = "Shop", menuName = "Shop")]
    public class Shop : ScriptableObject {
        [SerializeField] private HullModule[] m_hullModules;
        [SerializeField] private WeaponModule[] m_weaponModules;


        /// <summary>
        /// Отдает копию модулей.
        /// Можно считать что этот метод "продаёт" сразу весь ассортимент определенного типа товаров.
        /// </summary>
        /// <param name="type">Тип товаров</param>
        /// <returns></returns>
        public IModule[] GetModules(SlotType type) {
            return MakeCopy(type);
        }

        private IModule[] MakeCopy(SlotType type) {
            IModule[] copy;
            switch (type) {
                case SlotType.WEAPON:
                    copy = new IModule[m_weaponModules.Length];
                    for (int i = 0; i < copy.Length; i++) {
                        copy[i] = Instantiate(m_weaponModules[i]);
                    }
                    return copy;
                case SlotType.HULL:
                    copy = new IModule[m_hullModules.Length];
                    for (int i = 0; i < copy.Length; i++) {
                        copy[i] = Instantiate(m_hullModules[i]);
                    }
                    return copy;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}