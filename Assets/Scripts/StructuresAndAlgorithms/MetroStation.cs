namespace StructuresAndAlgorithms {
    /// <summary>
    /// Контейнер с данными о станции метро.
    /// Используется как вершина в графе для алгоритма поиска кратчашего пути.
    /// </summary>
    public class MetroStation {
        public readonly string Id;
        public bool IsVisited;
        public int Distance;

        public MetroStation(string id) {
            Id = id;
            IsVisited = false;
            Distance = -1;
        }
    }
}