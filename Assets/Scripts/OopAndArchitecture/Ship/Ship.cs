using System;
using System.Collections;
using OopAndArchitecture.Data;
using OopAndArchitecture.Enums;
using OopAndArchitecture.Events;
using OopAndArchitecture.Interfaces;
using OopAndArchitecture.Interfaces.UI;
using OopAndArchitecture.ScriptableObjects;
using OopAndArchitecture.ScriptableObjects.Managers;
using UnityEngine;
using UnityEngine.Assertions;


namespace OopAndArchitecture.Ship {
    /// <summary>
    /// Компонент-контроллер корабля.
    /// Ключевой объект для геймплея.
    /// </summary>
    [DisallowMultipleComponent]
    public class Ship : MonoBehaviour, ITargetable, IManagedEventListener, IManagedEventListenerWithParams {
        public string Id => m_shipData.Id;
        public Transform Location => transform;

        
        [Header("Data")]
        public ShipData Data;

        [Header("Events")] 
        public ManagedEventWithParams ShipDestroyed;
        public ManagedEvent StartShooting;
        public ManagedEvent StopShooting;
        
        [Header("Slots")]
        public Transform[] WeaponSpawnSlots;
        public Transform[] HullModulesSpawnSlots;


        private ShipData m_shipData;
        private IShipUi m_ui;
        private Coroutine m_shieldRecovery;
        
        
        private void Awake() {
            Assert.IsNotNull(Data, "Data != null");
            Assert.IsNotNull(ShipDestroyed, "ShipDestroyed != null");
            Assert.IsNotNull(StartShooting, "StartShooting != null");
            Assert.IsNotNull(StopShooting, "StopShooting != null");
            
            // Делаем копию скриптабл обжекта с данными корабля. Работаем дальше с ней.
            m_shipData = Instantiate(Data);
            m_shipData.InitModules();
            
            // Сразу подписываемся на события
            StartShooting.AddListener(this);
            StopShooting.AddListener(this);
            ShipDestroyed.AddListener(this);
        }
        

        /// <summary>
        /// Позволяет привязать реализацию интерфейса к логике корабля
        /// </summary>
        /// <param name="shipUi">Реализация интерфейса</param>
        public void AssignShipUi(IShipUi shipUi) {
            m_ui = shipUi;
            
            FlashShipUiStats();
        }

        /// <summary>
        /// Присоединяет модуль вооружения в соответствующий слот корабля
        /// </summary>
        /// <param name="slot">Слот</param>
        /// <param name="weaponModule">Вооружение</param>
        public void AttachWeaponModuleToSlot(ISlot slot, WeaponModule weaponModule) {
            IWeaponModule attached = m_shipData.GetWeaponModuleAt(slot);
            attached?.Remove();

            m_shipData.WeaponModules[slot.Index] = weaponModule;
            if (weaponModule != null) {
                weaponModule.Spawn(m_shipData,GetSpawnPoint(slot));
                weaponModule.SetReloadTimeModifier(m_shipData.ReloadTimeModifier);
            }
        }

        /// <summary>
        /// Присоединяет модуль модификации корпуса в соответствующий слот корабля
        /// </summary>
        /// <param name="slot">Слот</param>
        /// <param name="hullModule">Модификация корпуса</param>
        public void AttachHullModuleToSlot(ISlot slot, HullModule hullModule) {
            IHullModule attached = m_shipData.GetHullModuleAt(slot);
            if (attached != null) {
                RemoveHullModifier(attached);
                attached.Remove();
            }

            m_shipData.HullModules[slot.Index] = hullModule;
            if (hullModule != null) {
                hullModule.Spawn(m_shipData, GetSpawnPoint(slot));
                ApplyHullModifier(hullModule);
            }
        }

        /// <summary>
        /// Возвращает модуль в данный момент находящийся в заданном слоте.
        /// </summary>
        /// <param name="slot">Слот</param>
        /// <returns>Null - если в слоте ничего нет</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IModule GetModuleAttachedToSlot(ISlot slot) {
            switch (slot.Type) {
                case SlotType.WEAPON:
                    return m_shipData.WeaponModules[slot.Index];
                case SlotType.HULL:
                    return m_shipData.HullModules[slot.Index];
                default:
                    throw new ArgumentOutOfRangeException(nameof(slot.Type), slot.Type, null);
            }
        }

        /// <summary>
        /// Реализация интерфейса по получению урона.
        /// </summary>
        /// <param name="amount"></param>
        public void ApplyDamage(float amount) {
            Debug.Log($"[{gameObject.name}] APPLY {amount} DAMAGE");

            // Получаем урон и обновляем значения ХП и щитов
            if (m_shipData.Shields < amount) {
                amount -= m_shipData.Shields;
                m_shipData.Shields = 0f;
                m_shipData.HP -= amount;
            } else {
                m_shipData.Shields -= amount;
            }

            // Если ХП не осталось, то корабль уничтожен
            if (m_shipData.HP <= 0f) {
                m_shipData.HP = 0f;
                
                OnMyShipDestroyed();
            }

            FlashShipUiStats();
        }
        
        /// <summary>
        /// Обрабатываем пользовательские события
        /// </summary>
        /// <param name="eventId"></param>
        public void EventTrigger(string eventId) {
            // Заводим или останавливаем восстановление щитов по событиям старта и окончания стрельбы
            if (eventId == StartShooting.Id) {
                m_shieldRecovery = StartCoroutine(ShieldRecovery());
            } else if (eventId == StopShooting.Id && m_shieldRecovery != null) {
                StopCoroutine(m_shieldRecovery);
                
                // Восстанавливаем ХП и щиты
                m_shipData.RestoreToFull();
                FlashShipUiStats();
            }
        }
        
        /// <summary>
        /// Обрабатываем пользовательские события
        /// </summary>
        /// <param name="eventId"></param>
        public void EventTrigger(string eventId, IManagedEventParams param) {
            // Проверка на событие уничтожение чужого корабля
            if (eventId == ShipDestroyed.Id && ((ShipDestroyedEventParams)param).DestroyedShipId != m_shipData.Id) {
                if (m_shieldRecovery != null) {
                    StopCoroutine(m_shieldRecovery);
                }
                
                Debug.Log("OTHER SHIP WAS DESTROYED!");
            }
        }
        
        
        /// <summary>
        /// Тригерим событие уничтожения корабля
        /// </summary>
        private void OnMyShipDestroyed() {
            Debug.Log($" MY [{gameObject.name}] DESTROYED!");
            m_ui?.ShowDestroyedShipMessage();
            
            ShipDestroyed.Trigger(new ShipDestroyedEventParams(m_shipData.Id));
            StopShooting.Trigger();
        }

        /// <summary>
        /// Применить эффект который дает модуль корпуса.
        /// </summary>
        /// <param name="hullModule">Модуль корпуса</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void ApplyHullModifier(IHullModule hullModule) {
            Debug.Log($"[{gameObject.name}] MODIFIER: {hullModule.Description}");

            switch (hullModule.Effect) {
                case AppliedEffect.MODIFY_HP:
                    m_shipData.HP += hullModule.Amount;
                    m_shipData.MaxHp += hullModule.Amount;
                    break;
                case AppliedEffect.MODIFY_SHIELDS:
                    m_shipData.Shields += hullModule.Amount;
                    m_shipData.MaxShields += hullModule.Amount;
                    break;
                case AppliedEffect.MODIFY_RELOAD:
                    m_shipData.ReloadTimeModifier += hullModule.Amount;
                    foreach (IWeaponModule weaponModule in m_shipData.WeaponModules) {
                        weaponModule?.SetReloadTimeModifier(m_shipData.ReloadTimeModifier);
                    }
                    break;
                case AppliedEffect.MODIFY_SHIELDS_RECOVERY:
                    m_shipData.ShieldRecoveryAmount += hullModule.Amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            FlashShipUiStats();
        }

        /// <summary>
        /// Убрать эффект от модуля корпуса.
        /// </summary>
        /// <param name="hullModule">Модуль корпуса</param>
        private void RemoveHullModifier(IHullModule hullModule) {
            Debug.Log($"[{gameObject.name}] REMOVE MODIFIER: {hullModule.Description}");
            
            switch (hullModule.Effect) {
                case AppliedEffect.MODIFY_HP:
                    m_shipData.MaxHp -= hullModule.Amount;
                    m_shipData.HP = Mathf.Min(m_shipData.HP, m_shipData.MaxHp);
                    break;
                case AppliedEffect.MODIFY_SHIELDS:
                    m_shipData.MaxShields -= hullModule.Amount;
                    m_shipData.Shields = Mathf.Min(m_shipData.Shields, m_shipData.MaxShields);
                    break;
                case AppliedEffect.MODIFY_RELOAD:
                    m_shipData.ReloadTimeModifier -= hullModule.Amount;
                    foreach (IWeaponModule weaponModule in m_shipData.WeaponModules) {
                        weaponModule?.SetReloadTimeModifier(m_shipData.ReloadTimeModifier);
                    }
                    break;
                case AppliedEffect.MODIFY_SHIELDS_RECOVERY:
                    m_shipData.ShieldRecoveryAmount -= hullModule.Amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            FlashShipUiStats();
        }
        
        /// <summary>
        /// Возвращает точку спауна слота
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private Transform GetSpawnPoint(ISlot slot) {
            switch (slot.Type) {
                case SlotType.WEAPON:
                    return WeaponSpawnSlots[slot.Index];
                case SlotType.HULL:
                    return HullModulesSpawnSlots[slot.Index];
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Восстановление щита
        /// </summary>
        /// <returns></returns>
        private IEnumerator ShieldRecovery() {
            yield return new WaitForSeconds(m_shipData.ShieldRecoverySpeed);
            
            while (m_shipData.HP > 0f) {
                m_shipData.Shields = Mathf.Clamp(m_shipData.Shields + m_shipData.ShieldRecoveryAmount, 0f, m_shipData.MaxShields);
                
                FlashShipUiStats();
                yield return new WaitForSeconds(1f);
            }
        }
        
        private void FlashShipUiStats() {
            m_ui?.ChangeHp(m_shipData.MaxHp, m_shipData.HP);
            m_ui?.ChangeShields(m_shipData.MaxShields, m_shipData.Shields);
        }

        private void OnDestroy() {
            StartShooting.RemoveListener(this);
            StopShooting.RemoveListener(this);
            ShipDestroyed.RemoveListener(this);
        }
    }
}