namespace OopAndArchitecture.Interfaces {
    /// <summary>
    /// Интерфейс для получения колбека от пользовательских событий.
    /// </summary>
    public interface IManagedEventListener {
        void EventTrigger(string eventId);
    }
}