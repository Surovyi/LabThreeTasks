using OopAndArchitecture.Enums;
using OopAndArchitecture.Interfaces;
using OopAndArchitecture.ScriptableObjects;
using UnityEngine;

namespace OopAndArchitecture.Data {
    /// <summary>
    /// Контейнер с данными корабля.
    /// </summary>
    [CreateAssetMenu(fileName = "Ship", menuName = "Ship/Ship")]
    public class ShipData : ScriptableObject {
        public float MaxHp { get; set; }
        public float MaxShields { get; set; }
        public float ReloadTimeModifier { get; set; } = 1f;
        public HullModule[] HullModules { get; private set; }
        public WeaponModule[] WeaponModules { get; private set; }

        
        public string Id;
        public float HP;
        public float Shields;
        public float ShieldRecoveryAmount;
        public float ShieldRecoverySpeed;
        public int HullModulesCount;
        public int WeaponModulesCount;
        

        /// <summary>
        /// Стартовая инициализация корабля
        /// </summary>
        public void InitModules() {
            MaxHp = HP;
            MaxShields = Shields;
            
            HullModules = new HullModule[HullModulesCount];
            WeaponModules = new WeaponModule[WeaponModulesCount];
        }

        /// <summary>
        /// Восстановить здоровье и щиты корабля до максимума
        /// </summary>
        public void RestoreToFull() {
            HP = MaxHp;
            Shields = MaxShields;
        }
        
        /// <summary>
        /// Возвращает установленный на корабль модуль вооружения
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public IWeaponModule GetWeaponModuleAt(ISlot slot) {
            if (slot.Type != SlotType.WEAPON) {
                Debug.LogError("Ship.GetWeaponModuleAt => Incorrect slot type");
                return null;
            }
            return WeaponModules[slot.Index];
        }

        /// <summary>
        /// Возвращает установленный на корабль модуль корпуса
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public IHullModule GetHullModuleAt(ISlot slot) {
            if (slot.Type != SlotType.HULL) {
                Debug.LogError("Ship.GetHullModuleAt => Incorrect slot type");
                return null;
            }
            return HullModules[slot.Index];
        }
    }
}