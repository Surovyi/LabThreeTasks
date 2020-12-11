using System.Text;

namespace StructuresAndAlgorithms {
    /// <summary>
    /// Контейнер с результатами работы алгоритма поиска кратчайшего пути
    /// </summary>
    public struct PathResults {
        /// <summary>
        /// Стартовая станция
        /// </summary>
        private readonly string m_startStation;
        /// <summary>
        /// Конечная станция
        /// </summary>
        private readonly string m_endStation;
        /// <summary>
        /// Кол-во использованных разных веток метро
        /// </summary>
        private int m_useLinesCount;
        
        /// <summary>
        /// Кол-во пересадок между линиями метро
        /// </summary>
        private int m_transfers => m_useLinesCount - 1;

        /// <summary>
        /// Для форматирования конечного результата
        /// </summary>
        private readonly StringBuilder m_path;


        public PathResults(string start, string end) {
            m_startStation = start;
            m_endStation = end;
            m_useLinesCount = 0;
            m_path = new StringBuilder();
        }
        

        /// <summary>
        /// Добавить станцию в цепь кратчайшего пути
        /// </summary>
        /// <param name="station"></param>
        public void AddStationToResult(string station) {
            const string arrow = "->";
            
            if (m_path.Length > 0) {
                m_path.Insert(0, arrow);
            }
            m_path.Insert(0, station);
        }

        /// <summary>
        /// Увеличить число посещенных веток метро
        /// </summary>
        public void IncrementLinesCounter() {
            m_useLinesCount++;
        }

        private string GetFormatedShortPath() {
            const string notFound = "Not found";
            
            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.AppendFormat("Shortest path from '{0}' to '{1}': ", m_startStation, m_endStation);
            
            if (m_path.Length > 0) {
                resultBuilder.Append(m_path);
                resultBuilder.AppendFormat(". Transfers: {0}", m_transfers);
            } else {
                resultBuilder.Append(notFound);
            }
            
            return resultBuilder.ToString();
        }

        public override string ToString() {
            return GetFormatedShortPath();
        }
    }
}