namespace OopAndArchitecture.Interfaces.UI {
    /// <summary>
    /// Интерфейс для реализации UI корабля.
    /// Используется как Dependency Inversion и убирает зависимость у Ship от реализации его UI.
    /// </summary>
    public interface IShipUi {
        void ChangeHp(float maxValue, float value);
        void ChangeShields(float maxValue, float value);
        void ShowDestroyedShipMessage();
    }
}