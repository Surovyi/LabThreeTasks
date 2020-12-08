using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace MeshGeneration {
    public class Flag : MonoBehaviour {

        [Header("References")]
        [SerializeField] private MeshFilter m_meshFilter;
        [SerializeField] private MeshRenderer m_meshRenderer;
        [SerializeField] private Shader m_flagShader;
        [SerializeField] private Texture[] m_FlagTextures;
        
        [Header("Parameters")]
        [SerializeField] private int m_WidthSizeInCm;
        [SerializeField] private int m_HeightSizeInCm;
        [SerializeField] private int m_horizontalVerticesCount;
        [SerializeField] private int m_verticalVerticesCount;
        
        [Header("Optional")]
        [SerializeField] private bool m_doubleSided;
        [SerializeField] private bool m_useGpuAnimation;

        private Material m_flagMaterial;
        private Coroutine m_cpuScrollTexture;
        private Coroutine m_cpuWaveFlag;
        

        /// <summary>
        /// Создаем процедурно флаг и анимируем его при помощи шейдера, либо при помощи CPU
        /// </summary>
        public void CreateFlag() {
            Clear();
            GenerateFlagMesh();
            AssignFlagTexture();
            
            if (m_useGpuAnimation == false) {
                AnimateFlagByCpu();
            }
        }

        /// <summary>
        /// Очищаем ресурсы.
        /// Пригодится при повторном нажатии на кнопку Generate.
        /// </summary>
        private void Clear() {
            if (m_cpuScrollTexture != null) {
                StopCoroutine(m_cpuScrollTexture);
                m_cpuScrollTexture = null;
            }
            if (m_cpuWaveFlag != null) {
                StopCoroutine(m_cpuWaveFlag);
                m_cpuWaveFlag = null;
            }
            if (m_meshFilter != null && m_meshFilter.mesh != null) {
                Destroy(m_meshFilter.mesh);
            }
        }

        /// <summary>
        /// Формируем меш из указанных в Инспекторе данных
        /// </summary>
        private void GenerateFlagMesh() {
            MeshGeneratorData generatorData = new MeshGeneratorData {
                HorizontalVerticesCount = m_horizontalVerticesCount,
                VerticalVerticesCount = m_verticalVerticesCount,
                WidthInCm = m_WidthSizeInCm,
                HeightInCm = m_HeightSizeInCm,
                DoubleSided = m_doubleSided
            };
            
            MeshGenerator meshGenerator = new MeshGenerator(generatorData);
            Mesh flagMesh = meshGenerator.GenerateMesh();

            m_meshFilter.mesh = flagMesh;
            m_meshFilter.mesh.Optimize();
        }
        
        /// <summary>
        /// Создаем материал из кастомного шейдера.
        /// Назначаем рандомную текстуру.
        /// </summary>
        private void AssignFlagTexture() {
            int randCountryTexture = Random.Range(0, m_FlagTextures.Length);

            m_flagMaterial = new Material(m_flagShader) {
                enableInstancing = true, 
                mainTexture = m_FlagTextures[randCountryTexture]
            };

            m_meshRenderer.material = m_flagMaterial;
        }

        /// <summary>
        /// Если выбран параметр анимации при помощи CPU, то стартуем необходимые для этого операции
        /// </summary>
        private void AnimateFlagByCpu() {
            m_flagMaterial.SetFloat("_ScrollSpeed", 0f);
            m_cpuScrollTexture = StartCoroutine(ScrollTextureByCpu());
            
            m_flagMaterial.SetFloat("_WavingAmplitude", 0f);
            m_cpuWaveFlag = StartCoroutine(WaveFlagByCpu());
        }


        /// <summary>
        /// Скрол текстуры средствами CPU.
        /// scrollSpeed можно вынести в отдельную переменную, но для чистоты оставил локальной константой.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ScrollTextureByCpu() {
            const float scrollSpeed = 0.3f;
            const string textureName = "_MainTex";
            
            while (true) {
                m_flagMaterial.SetTextureOffset(textureName, new Vector2(Time.time * scrollSpeed, 0f));
                yield return null;
            }
        }

        /// <summary>
        /// Движение волной средствами CPU.
        /// Локальные константы можно было вынести в переменные, но для чистоты оставил внутри.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaveFlagByCpu() {
            const float wavingFrequency = 1.5f;
            const float wavingAmplitude = 0.15f;
            
            List<Vector3> verticesWorWaveCache = new List<Vector3>();
            
            while (true) {
                m_meshFilter.mesh.GetVertices(verticesWorWaveCache);

                for (int i = 0; i < verticesWorWaveCache.Count; i++) {
                    Vector3 position = verticesWorWaveCache[i];
                    position.z = position.x * Mathf.Sin(Time.time * wavingFrequency + position.x) * wavingAmplitude;
                    verticesWorWaveCache[i] = position;
                }

                m_meshFilter.mesh.SetVertices(verticesWorWaveCache);
                m_meshFilter.mesh.RecalculateNormals();
                yield return null;
            }
        }
        
        
        /// <summary>
        /// Убеждаемся что все ссылки на месте
        /// </summary>
        private void Awake() {
            Assert.IsNotNull(m_meshFilter, "Flag.Awake => m_meshFilter != null");
            Assert.IsNotNull(m_meshRenderer, "Flag.Awake => m_meshRenderer != null");
            Assert.IsNotNull(m_flagShader, "Flag.Awake => m_flagShader != null");
            Assert.IsTrue(m_FlagTextures.Length > 0, "Flag.AssignFlagTexture => m_FlagTextures.Length > 0");
        }

        private void OnDestroy() {
            Clear();
        }
    }
}