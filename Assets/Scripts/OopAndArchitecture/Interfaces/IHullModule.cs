using OopAndArchitecture.Enums;

namespace OopAndArchitecture.Interfaces {
    /// <summary>
    /// Интерфейс модуля корпуса корабля.
    /// Абстрагирует реализацию этих модулей от логики работы самого корабля.
    /// </summary>
    public interface IHullModule : IModule {
        AppliedEffect Effect { get; }
        float Amount { get; }
    }
}