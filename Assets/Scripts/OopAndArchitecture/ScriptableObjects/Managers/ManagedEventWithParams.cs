using System.Collections.Generic;
using OopAndArchitecture.Interfaces;
using UnityEngine;

namespace OopAndArchitecture.ScriptableObjects.Managers {
    /// <summary>
    /// Пользовательское событие с параметрами.
    /// Единственное отличие от ManagedEvent это наличие параметра, который можно передать слушателям события
    /// </summary>
    [CreateAssetMenu(fileName = "GameEventWithParams_", menuName = "Managers/Game Event With Params")]
    public class ManagedEventWithParams : ScriptableObject {
        public string Id;
        
        private readonly List<IManagedEventListenerWithParams> m_listeners = new List<IManagedEventListenerWithParams>();

        /// <summary>
        /// Добавить слушателя события
        /// </summary>
        /// <param name="listener"></param>
        public void AddListener(IManagedEventListenerWithParams listener) {
            m_listeners.Add(listener);
        }

        /// <summary>
        /// Убрать из слушателей события
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveListener(IManagedEventListenerWithParams listener) {
            m_listeners.Remove(listener);
        }
        
        /// <summary>
        /// Вызвать событие
        /// </summary>
        public void Trigger(IManagedEventParams eventParams) {
            for (int i = m_listeners.Count - 1; i >= 0; i--) {
                m_listeners[i]?.EventTrigger(Id, eventParams);
            }
        }
    }
}