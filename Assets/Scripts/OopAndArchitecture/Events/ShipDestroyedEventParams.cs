using OopAndArchitecture.Interfaces;

namespace OopAndArchitecture.Events {
    /// <summary>
    /// Событие - уничтожен один из кораблей
    /// </summary>
    public class ShipDestroyedEventParams : IManagedEventParams {
        public string DestroyedShipId { get; }
        
        
        public ShipDestroyedEventParams(string shipId) {
            DestroyedShipId = shipId;
        }
    }
}