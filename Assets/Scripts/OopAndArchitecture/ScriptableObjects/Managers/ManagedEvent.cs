using System.Collections.Generic;
using OopAndArchitecture.Interfaces;
using UnityEngine;

namespace OopAndArchitecture.ScriptableObjects.Managers {
    /// <summary>
    /// Пользовательское событие.
    /// Решил попробовать формат сериализованных событий для общения между объектами.
    /// Тем самым в абсолюте избавиться от синглтонов или каких-то контролирующих менеджеров. Благо, ТЗ позволяет :)
    /// </summary>
    [CreateAssetMenu(fileName = "GameEvent_", menuName = "Managers/Game Event")]
    public class ManagedEvent : ScriptableObject {
        /// <summary>
        /// Id события. Указывается пользователем. Используется для сравнения и определения типа события.
        /// NOTE: Можно запариться и избавиться от этого параметра, но для этого прототипа это будет слишком смело.
        /// NOTE: Так как тот способ который знаю я, требует применения скриптов редактора. А я не хотел "уплотнять" код еще и скриптами для редактора.
        /// </summary>
        public string Id;
        
        private readonly List<IManagedEventListener> m_listeners = new List<IManagedEventListener>();

        /// <summary>
        /// Добавить слушателя события
        /// </summary>
        /// <param name="listener"></param>
        public void AddListener(IManagedEventListener listener) {
            m_listeners.Add(listener);
        }

        /// <summary>
        /// Убрать из слушателей события
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveListener(IManagedEventListener listener) {
            m_listeners.Remove(listener);
        }
        
        /// <summary>
        /// Вызвать событие
        /// </summary>
        public void Trigger() {
            for (int i = m_listeners.Count - 1; i >= 0; i--) {
                m_listeners[i]?.EventTrigger(Id);
            }
        }
    }
}