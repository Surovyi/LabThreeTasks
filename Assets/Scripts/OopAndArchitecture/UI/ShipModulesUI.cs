using System;
using System.Collections.Generic;
using OopAndArchitecture.Enums;
using OopAndArchitecture.Interfaces;
using OopAndArchitecture.ScriptableObjects;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace OopAndArchitecture.UI {
    /// <summary>
    /// Интерфейс позволяющий устанавливать и убирать модули на корабль.
    /// По сути, это конфигуратор корабля, который работает до того как игрок нажмет на "Начать битву".
    /// </summary>
    public class ShipModulesUI : MonoBehaviour {
        public Shop.Shop Shop;
        public Ship.Ship Ship;

        public RectTransform ShipSlotsParent;
        public RectTransform ShipModulesParent;
        public RectTransform ShipStatsParent;
        public SlotUI SlotItemPrefab;
        public ModuleUI ModuleItemPrefab;
        public ShipModulesStatUI ShipModulesStatPrefab;
        public Text ModuleTypeText;
        
        private readonly List<ModuleUI> m_generatedModules = new List<ModuleUI>();
        private readonly List<SlotUI> m_generatedSlots = new List<SlotUI>();
        private readonly List<ShipModulesStatUI> m_generatedStats = new List<ShipModulesStatUI>();
        private ISlot m_activeSlot;
        

        private void Start() {
            Assert.IsNotNull(Ship, "Ship != null");
            Assert.IsNotNull(Shop, "Shop != null");
            Assert.IsNotNull(ShipSlotsParent, "ShipSlotsParent != null");
            Assert.IsNotNull(ShipModulesParent, "ShipModulesParent != null");
            Assert.IsNotNull(ShipStatsParent, "ShipStatsParent != null");
            Assert.IsNotNull(SlotItemPrefab, "SlotItemPrefab != null");
            Assert.IsNotNull(ModuleItemPrefab, "ModuleItemPrefab != null");
            Assert.IsNotNull(ShipModulesStatPrefab, "ShipModulesStatPrefab != null");
            Assert.IsNotNull(ModuleTypeText, "ModuleTypeText != null");
            
            GenerateSlots();
        }

        /// <summary>
        /// Генерируем слоты, доступные для модификации корабля.
        /// </summary>
        private void GenerateSlots() {
            for (int i = 0; i < Ship.Data.WeaponModulesCount; i++) {
                SlotUI item = Instantiate(SlotItemPrefab, ShipSlotsParent);
                item.gameObject.name = $"WEAPON_slot_{i}";
                
                item.Init(SlotType.WEAPON, i, GenerateModules);
                m_generatedSlots.Add(item);
            }
            for (int i = 0; i < Ship.Data.HullModulesCount; i++) {
                SlotUI item = Instantiate(SlotItemPrefab, ShipSlotsParent);
                item.gameObject.name = $"HULL_slot_{i}";
                
                item.Init(SlotType.HULL, i, GenerateModules);
                m_generatedSlots.Add(item);
            }
        }

        /// <summary>
        /// Генерируем модули доступные для модификации корабля, в зависимости от выбранного слота.
        /// </summary>
        /// <param name="slot"></param>
        private void GenerateModules(ISlot slot) {
            ClearModulesList();
            RefreshSlotsColor(slot);

            m_activeSlot = slot;
            ModuleTypeText.text = m_activeSlot.Type.ToString();
            
            switch (slot.Type) {
                case SlotType.WEAPON:
                    GenerateModules(Shop.GetModules(SlotType.WEAPON));
                    break;
                case SlotType.HULL:
                    GenerateModules(Shop.GetModules(SlotType.HULL));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Генерируем стат-информацию в зависимости от установленого в слот модуля
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="module"></param>
        private void GenerateStats(ISlot slot, IModule module) {
            ShipModulesStatUI shipModulesStat = m_generatedStats.Find(x => x.Compare(slot));
            if (shipModulesStat != null) {
                if (module != null) {
                    shipModulesStat.SetSlotModuleDescription(slot, module);
                } else {
                    m_generatedStats.Remove(shipModulesStat);
                    Destroy(shipModulesStat.gameObject);
                }
            } else {
                ShipModulesStatUI item = Instantiate(ShipModulesStatPrefab, ShipStatsParent);
                item.gameObject.name = module.Description;
                item.SetSlotModuleDescription(slot, module);
            
                m_generatedStats.Add(item);
            }
        }

        /// <summary>
        /// Генерируем модули из массива модулей
        /// </summary>
        /// <param name="modules"></param>
        private void GenerateModules(IModule[] modules) {
            IModule moduleAttachedToShip = Ship.GetModuleAttachedToSlot(m_activeSlot);
            
            foreach (IModule weaponModule in modules) {
                ModuleUI item = Instantiate(ModuleItemPrefab, ShipModulesParent);
                item.gameObject.name = weaponModule.Description;
                
                bool isAttachedToShip = moduleAttachedToShip?.Id == weaponModule.Id;
                item.Init(weaponModule, isAttachedToShip, AttachModuleToActiveSlot);
                m_generatedModules.Add(item);
            }
        }

        /// <summary>
        /// Обновляем цвета в списке слотов
        /// </summary>
        /// <param name="slot"></param>
        private void RefreshSlotsColor(ISlot slot) {
            foreach (SlotUI generatedSlot in m_generatedSlots) {
                generatedSlot.ActivateByColor(generatedSlot.Type == slot?.Type && generatedSlot.Index == slot.Index);
            }
        }
        
        /// <summary>
        /// Очищаем весь список модулей.
        /// Необходимо когда игрок выбирает другой слот.
        /// </summary>
        private void ClearModulesList() {
            foreach (ModuleUI module in m_generatedModules) {
                Destroy(module.gameObject);
            }
            m_generatedModules.Clear();
            m_activeSlot = null;
        }

        /// <summary>
        /// Присоединяем модуль к активному в данный момент слоту.
        /// Условно, подтверждаем "покупку" модуля.
        /// Даём команду на физическую установку модуля на корабль.
        /// </summary>
        /// <param name="module"></param>
        private void AttachModuleToActiveSlot(IModule module) {
            switch (m_activeSlot?.Type) {
                case SlotType.WEAPON:
                    Ship.AttachWeaponModuleToSlot(m_activeSlot, (WeaponModule)module);
                    GenerateStats(m_activeSlot, module);
                    break;
                case SlotType.HULL:
                    Ship.AttachHullModuleToSlot(m_activeSlot, (HullModule)module);
                    GenerateStats(m_activeSlot, module);
                    break;
                case null:
                    Debug.LogError("Active slot is null!");
                    break;
                default:
                    Debug.LogError($"Slot type is not recognized! Type: {m_activeSlot.Type}");
                    break;
            }

            GenerateModules(m_activeSlot);
        }
        
    }
}