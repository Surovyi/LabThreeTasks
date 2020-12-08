namespace MeshGeneration {
    /// <summary>
    /// Контейнер с данными для генерации меша
    /// </summary>
    public struct MeshGeneratorData {
        /// <summary>
        /// Кол-во вершин по-горизонтали
        /// </summary>
        public int HorizontalVerticesCount;
        /// <summary>
        /// Кол-во вершин по-вертикали
        /// </summary>
        public int VerticalVerticesCount;
        /// <summary>
        /// Ширина флага в сантиметрах
        /// </summary>
        public float WidthInCm;
        /// <summary>
        /// Высота флага в сантиметрах
        /// </summary>
        public float HeightInCm;
        /// <summary>
        /// Будет ли сгенерированный меш двусторонний
        /// </summary>
        public bool DoubleSided;
    }
}