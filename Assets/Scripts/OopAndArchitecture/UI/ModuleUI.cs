using System;
using OopAndArchitecture.Interfaces;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace OopAndArchitecture.UI {
    /// <summary>
    /// Интерфейс для "модуля" корабля.
    /// Этими объектами наполняется список доступных для апгрейда модулей корабля
    /// </summary>
    public class ModuleUI : MonoBehaviour {
        [SerializeField] private Text m_text;
        [SerializeField] private Image m_image;
        [SerializeField] private Color m_attachedColor;

        private Color m_notAttachedColor;

        private bool m_isAttached;
        private Action<IModule> m_onClick;
        private IModule m_shipModule;
        
        
        /// <summary>
        /// Инициализация модуля информацией о нем
        /// </summary>
        /// <param name="module">Контейнер с данными о модуле</param>
        /// <param name="isAttachedToShip">Уже прикреплен к кораблю?</param>
        /// <param name="onClick">Колбек</param>
        public void Init(IModule module, bool isAttachedToShip, Action<IModule> onClick) {
            m_shipModule = module;
            m_isAttached = isAttachedToShip;
            m_text.text = module.Description;
            m_onClick = onClick;

            ChangeButtonColor();
        }
        
        /// <summary>
        /// Помечает что модуль прицеплен к кораблю. Имеется ввиду UI, а не физически на корабле.
        /// </summary>
        public void Attach() {
            m_isAttached = !m_isAttached;

            ChangeButtonColor();

            m_onClick?.Invoke(m_isAttached ? m_shipModule : null);
        }

        
        private void ChangeButtonColor() {
            m_image.color = m_isAttached == true ? m_attachedColor : m_notAttachedColor;
        }

        private void Awake() {
            Assert.IsNotNull(m_text, "m_text != null");
            Assert.IsNotNull(m_image, "m_image != null");
            
            m_notAttachedColor = m_image.color;
        }

        private void OnDestroy() {
            m_onClick = null;
            m_shipModule = null;
        }
    }
}