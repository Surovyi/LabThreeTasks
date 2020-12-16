using OopAndArchitecture.Enums;

namespace OopAndArchitecture.Interfaces {
    /// <summary>
    /// Интерфейс для работы со слотами корабля в которые можно вставлять модули корабля.
    /// </summary>
    public interface ISlot {
        SlotType Type { get; }
        int Index { get; }
    }
}