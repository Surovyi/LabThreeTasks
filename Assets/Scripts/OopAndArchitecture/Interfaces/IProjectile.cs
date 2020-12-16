namespace OopAndArchitecture.Interfaces {
    /// <summary>
    /// Интерфейс для реализации снарядов.
    /// </summary>
    public interface IProjectile {
        void SetDamageData(IWeaponModule weaponModule);
        void Fly();
        void ApplyDamage(ITargetable target);
    }
}