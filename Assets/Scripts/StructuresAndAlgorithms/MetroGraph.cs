using System.Collections.Generic;

namespace StructuresAndAlgorithms {
    /// <summary>
    /// Класс для решения задачи поиска кратчайшего пути.
    /// Содержит представление сети метро в виде Графа.
    /// Станция - это вертекс или вершина графа.
    /// Соединение - это ребро между двумя вершинами.
    /// </summary>
    public class MetroGraph {
        private readonly MetroStation[] m_stations;
        
        private readonly string[,] m_connections;
        private readonly int m_stationsCount;
        private readonly Dictionary<string, int> m_nameToIndexMap;
        
        private int m_stationIndex;

        public MetroGraph(int stationsCount) {
            m_stationsCount = stationsCount;
            m_stations = new MetroStation[m_stationsCount];
            m_connections = new string[m_stationsCount, m_stationsCount];
            m_nameToIndexMap = new Dictionary<string, int>(m_stationsCount);
        }

        /// <summary>
        /// Добавить новую станцию в сеть метро
        /// </summary>
        /// <param name="station"></param>
        public void AddStation(MetroStation station) {
            if (m_nameToIndexMap.ContainsKey(station.Id) == true) {
                return;
            }
            
            m_stations[m_stationIndex] = station;
            m_nameToIndexMap[station.Id] = m_stationIndex;
            m_stationIndex++;
        }
        
        /// <summary>
        /// Добавить связь между двумя станции.
        /// Так формируются ветки метро.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="line"></param>
        public void AddConnection(string start, string end, string line) {
            AddConnection(m_nameToIndexMap[start], m_nameToIndexMap[end], line);
        }


        /// <summary>
        /// Поиск кратчайшего пути между двумя станциями.
        /// Это идеальный кандидат для еще одного выделения в интерфейс.
        /// Таким образом можно было бы реализовать несколько алгоритмов, без привязки к реализации в этом классе.
        /// В уме держал, но не делал так, чтобы сэкономить время.
        ///
        /// Вообще тут можно использовать любой из набора алгоритмов поиска пути. Решил попробовать волновой алгоритм.
        /// Алгоритм Ли для поиска кратчайшего пути.
        /// https://ru.wikipedia.org/wiki/%D0%90%D0%BB%D0%B3%D0%BE%D1%80%D0%B8%D1%82%D0%BC_%D0%9B%D0%B8
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public PathResults GetShortestPath(string start, string end) {
            PathResults results = new PathResults (start, end);
            
            int startIndex = m_nameToIndexMap[start];
            int endIndex = m_nameToIndexMap[end];
            
            // Алгорим работает в 3 шага: инициализация, распространение волны и восстановление пути

            // 1. Инициализация
            m_stations[startIndex].IsVisited = true;
            m_stations[startIndex].Distance = 0;

            // 2. Распространение волны
            bool isFound = Wave(startIndex, endIndex);

            // Если ничего не нашли до сих пор, то кратчайшего маршрута нет
            if (isFound == false) {
                return results;
            }

            // 3. Восстановление пути
            TraceBack(ref results, startIndex, endIndex);

            // Подчищаем за алгоритмом
            ClearTracing();
            
            return results;
        }

        /// <summary>
        /// Шаг 2. Распространение волны.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        private bool Wave(int startIndex, int endIndex) {
            int distance = 0;
            bool isFound = false;
            
            // Берем стартовый вертекс
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(startIndex);
            
            // Начинаем со стартового вертекса и движемся по его рёбрам
            while (queue.Count > 0) {
                int stationRoot = queue.Dequeue();
                int stationConnected = GetConnectedStation(stationRoot);

                // Если мы прошлись по всем вертексам с одинаковой дистанцией, то переключаемся на следующую
                if (m_stations[stationRoot].Distance > distance) {
                    distance++;
                }
            
                // Пока есть связанные с выбранной вершиной другие вершины
                while (stationConnected != -1) {
                    MetroStation connected = m_stations[stationConnected];
                    connected.IsVisited = true;
                    connected.Distance = distance + 1;

                    // Если Id станции совпал с конечным - мы сформировали путь и можно остановить волну
                    if (stationConnected == endIndex) {
                        isFound = true;
                        queue.Clear();
                        break;
                    }
                    
                    queue.Enqueue(stationConnected);
                    stationConnected = GetConnectedStation(stationRoot);
                }
            }

            return isFound;
        }

        /// <summary>
        /// Шаг 3. Восстановление пути.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        private void TraceBack(ref PathResults results, int startIndex, int endIndex) {
            
            // Действуем от обратного. Берем финишный вертекс.
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(endIndex);
            
            results.AddStationToResult(m_stations[endIndex].Id);
            
            int distance = m_stations[endIndex].Distance;
            string currentLineName = string.Empty;
            
            // Движемся по ребрам вертексов
            while (queue.Count > 0) {
                int stationRoot = queue.Dequeue();
                int stationConnected = GetConnectedStation(stationRoot, true);

                // Пока есть ранее посещенное ребро
                while (stationConnected != -1) {
                    m_stations[stationConnected].IsVisited = false;
                    
                    // Если станция с дистанцией меньше чем текущая зафиксированная, то мы на верном пути
                    if (m_stations[stationConnected].Distance < distance) {
                        // Запоминаем эту станцию и фиксируем новую дистанцию
                        results.AddStationToResult(m_stations[stationConnected].Id);
                        distance = m_stations[stationConnected].Distance;

                        // Считываем информацию о линиях метро и пересадках между ними
                        string lineName = m_connections[stationRoot, stationConnected];
                        if (string.Equals(currentLineName, lineName) == false) {
                            results.IncrementLinesCounter();
                            currentLineName = lineName;
                        }
                        
                        queue.Clear();
                        
                        // Если это была стартовая станция - путь найден!
                        if (stationConnected == startIndex) {
                            break;
                        }
                        
                        // Если станция не стартовая, то продолжаем трейсить маршрут с этой точки
                        queue.Enqueue(stationConnected);
                        break;
                    }
                    
                    // Если станция с большой дистанцией - продолжаем трейсить маршрут
                    stationConnected = GetConnectedStation(stationRoot, true);
                }
            }
        }
        
        /// <summary>
        /// Подчищаем переменные, которые применялись для поиска пути
        /// </summary>
        private void ClearTracing() {
            for (int i = 0; i < m_stations.Length; i++) {
                m_stations[i].IsVisited = false;
                m_stations[i].Distance = -1;
            }
        }
        
        private void AddConnection(int start, int end, string line) {
            m_connections[start, end] = line;
            m_connections[end, start] = line;
        }
        
        /// <summary>
        /// Найти станцию соединенную с заданной.
        /// При этом можно указать необходима любая станция или уже посещенная ранее.
        /// </summary>
        /// <param name="v">Индекс станции, чьих соседей мы ищем</param>
        /// <param name="visited">Необходимо уже посещенная ранее станция?</param>
        /// <returns></returns>
        private int GetConnectedStation(int v, bool visited = false) {
            for (int i = 0; i < m_stationsCount; i++) {
                if (string.IsNullOrEmpty(m_connections[v, i]) == false && m_stations[i].IsVisited == visited) {
                    return i;
                }
            }

            return -1;
        }
    }
}