using UnityEngine;

namespace MeshGeneration {
    public class MeshGenerator {
        private readonly MeshGeneratorData m_Data;
        
        public MeshGenerator(MeshGeneratorData data) {
            m_Data = data;
        }
        
        /// <summary>
        /// Генерируем меш из заданных ранее в конструкторе параметров
        /// </summary>
        /// <returns></returns>
        public Mesh GenerateMesh(bool dynamicMesh) {
            Mesh mesh = new Mesh();
            // Помечаем меш как динамический, если он будет "шататься" на CPU.
            if (dynamicMesh == true) {
                mesh.MarkDynamic();
            }
            
            MeshData meshData = GenerateMeshData();
            ApplyMeshDataToMesh(mesh, meshData);
            
            return mesh;
        }

        /// <summary>
        /// Фирмирование всех необходимых данных для меша
        /// </summary>
        /// <returns></returns>
        private MeshData GenerateMeshData() {
            MeshData data = new MeshData(m_Data.HorizontalVerticesCount, m_Data.VerticalVerticesCount, m_Data.DoubleSided);
            
            // Вычисляем расстояние между 2 вертексами
            float scaleWidth = 0.01f * m_Data.WidthInCm / m_Data.HorizontalVerticesCount;
            float scaleHeight = 0.01f * m_Data.HeightInCm / m_Data.VerticalVerticesCount;
            
            int vertexIndex = 0;
            int triangleIndex = 0;
            
            for (int i = 0; i < m_Data.VerticalVerticesCount; i++) {
                for (int j = 0; j < m_Data.HorizontalVerticesCount; j++) {
                    
                    // Добавляем вертекс и его uv в контейнер
                    data.AddVertex(vertexIndex, new Vector3(scaleWidth * j, -scaleHeight * i));
                    data.AddUV(vertexIndex, new Vector2(j / (m_Data.HorizontalVerticesCount - 1f), 1f - i / (m_Data.VerticalVerticesCount - 1f)));

                    // Если выбран двусторонний меш, то дополнительно генерируем вертексы 
                    if (m_Data.DoubleSided == true) {
                        int doubleVertexIndex = data.VerticesOneSideCount + vertexIndex;
                        data.AddVertex(doubleVertexIndex, new Vector3(scaleWidth * j, -scaleHeight * i));
                        data.AddUV(doubleVertexIndex, new Vector2(j / (m_Data.HorizontalVerticesCount - 1f),  1f - i / (m_Data.VerticalVerticesCount - 1f)));
                    }

                    // Генерируем треугольники из вертексов. Нам не нужны последний ряд и последняя колонка.
                    if (i < m_Data.VerticalVerticesCount - 1 && j < m_Data.HorizontalVerticesCount - 1) {
                        data.AddTriangle(triangleIndex,
                            vertexIndex, 
                            vertexIndex + m_Data.HorizontalVerticesCount + 1, 
                            vertexIndex + m_Data.HorizontalVerticesCount);
                        data.AddTriangle(triangleIndex + 3,
                            vertexIndex, 
                            vertexIndex + 1,
                            vertexIndex + m_Data.HorizontalVerticesCount + 1);
                        
                        // Для двустороннего меша генерируем треугольники со сдвигом
                        if (m_Data.DoubleSided == true) {
                            int doubleTriangleIndex = data.TrianglesOneSideCount + triangleIndex;
                            
                            data.AddTriangle(doubleTriangleIndex,
                                data.VerticesOneSideCount + vertexIndex,
                                data.VerticesOneSideCount + vertexIndex + m_Data.HorizontalVerticesCount,
                                data.VerticesOneSideCount + vertexIndex + m_Data.HorizontalVerticesCount + 1);
                            data.AddTriangle(doubleTriangleIndex + 3,
                                data.VerticesOneSideCount + vertexIndex,
                                data.VerticesOneSideCount + vertexIndex + m_Data.HorizontalVerticesCount + 1,
                                data.VerticesOneSideCount + vertexIndex + 1);
                        }

                        // Увеличиваем индекс треугольников на 6 => 2 треугольника на квадрат и 3 вершины на каждый треугольник
                        triangleIndex += 6;
                    }
                    
                    vertexIndex++;
                }
            }

            return data;
        }

        /// <summary>
        /// Применяем сформированные данные о меше к самому мешу.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="data"></param>
        private void ApplyMeshDataToMesh(Mesh mesh, MeshData data) {
            mesh.SetVertices(data.Vertices);
            mesh.SetTriangles(data.Triangles, 0);
            mesh.SetUVs(0, data.UVs);

            mesh.RecalculateNormals();
        }
    }
}