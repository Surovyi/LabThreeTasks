using UnityEngine;

namespace MeshGeneration {
    /// <summary>
    /// Контейнер с данными для генерации меша.
    /// </summary>
    public class MeshData {
        public readonly Vector3[] Vertices;
        public readonly int[] Triangles;
        public readonly Vector2[] UVs;

        public readonly int VerticesOneSideCount;
        public readonly int TrianglesOneSideCount;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="horizontalVerticesCount">Кол-во вершин по горизонтали</param>
        /// <param name="verticalVerticesCount">Кол-во вершин по вертикали</param>
        /// <param name="doubleSided">Меш будет двухсторонним?</param>
        public MeshData(int horizontalVerticesCount, int verticalVerticesCount, bool doubleSided) {
            VerticesOneSideCount = horizontalVerticesCount * verticalVerticesCount;
            Vertices = new Vector3[doubleSided ? VerticesOneSideCount * 2 : VerticesOneSideCount];

            UVs = new Vector2[doubleSided ? VerticesOneSideCount * 2 : VerticesOneSideCount];
            
            TrianglesOneSideCount = (horizontalVerticesCount - 1) * (verticalVerticesCount - 1) * 2 * 3;
            Triangles = new int[doubleSided ? TrianglesOneSideCount * 2 : TrianglesOneSideCount];
        }

        /// <summary>
        /// По индексу добавляем позицию вертекса
        /// </summary>
        /// <param name="index"></param>
        /// <param name="position"></param>
        public void AddVertex(int index, Vector3 position) {
            Vertices[index] = position;
        }

        /// <summary>
        /// По индексу добавляем позицию одного треугольника
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pointA"></param>
        /// <param name="pointB"></param>
        /// <param name="pointC"></param>
        public void AddTriangle(int index, int pointA, int pointB, int pointC) {
            Triangles[index] = pointA;
            Triangles[index + 1] = pointB;
            Triangles[index + 2] = pointC;
        }

        /// <summary>
        /// По индексу добавляем UV координату
        /// </summary>
        /// <param name="index"></param>
        /// <param name="uv"></param>
        public void AddUV(int index, Vector2 uv) {
            UVs[index] = uv;
        }
    }
}