namespace OopAndArchitecture.Interfaces {
    /// <summary>
    /// Интерфейс для получения колбека от пользовательских событий.
    /// Эффект такой же как и у IManagedEventListener, но есть возможность передать/получить параметр.
    /// </summary>
    public interface IManagedEventListenerWithParams {
        void EventTrigger(string eventId, IManagedEventParams param);
    }
}