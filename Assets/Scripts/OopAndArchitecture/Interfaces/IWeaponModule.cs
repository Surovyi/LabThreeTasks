using OopAndArchitecture.Weapons;

namespace OopAndArchitecture.Interfaces {
    /// <summary>
    /// Интерфейс модуля вооружения корабля.
    /// Абстрагирует реализацию этих модулей от логики работы самого корабля.
    /// </summary>
    public interface IWeaponModule : IModule {
        string ShipId { get; }
        float ReloadTime { get; }
        float ReloadTimeModifier { get; }
        float Damage { get; }
        Weapon Weapon { get; }

        void SetReloadTimeModifier(float newModifier);
    }
}