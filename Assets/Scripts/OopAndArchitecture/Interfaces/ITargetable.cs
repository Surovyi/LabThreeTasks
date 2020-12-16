using UnityEngine;

namespace OopAndArchitecture.Interfaces {
    /// <summary>
    /// Интерфейс определяющий объекты которых "можно взять в цель".
    /// Решил попробовать смешать классический IDamageable с некоторыми своими параметрами и назвать интерфейс по-другому.
    /// </summary>
    public interface ITargetable {
        string Id { get; }
        Transform Location { get; }
        void ApplyDamage(float amount);
    }
}