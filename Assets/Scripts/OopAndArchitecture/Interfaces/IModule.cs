using OopAndArchitecture.Data;
using UnityEngine;

namespace OopAndArchitecture.Interfaces {
    /// <summary>
    /// Интерфейс модуля корабля.
    /// Является базовым для более специализированных модулей.
    /// По правилам текущей реализации, любой модуль должен быть заспаунен и может быть убрать.
    /// </summary>
    public interface IModule {
        string Id { get; }
        string Description { get; }

        void Spawn(ShipData shipData, Transform parent);
        void Remove();
    }
}