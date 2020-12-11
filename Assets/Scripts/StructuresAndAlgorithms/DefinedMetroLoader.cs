using System.Collections.Generic;
using UnityEngine;

namespace StructuresAndAlgorithms {
    /// <summary>
    /// Процедурно сгенерированная сеть метро.
    /// Тут можно протестировать работу алгоритма.
    ///
    /// Для демонстрации использовать сцену 2_StructuresAndAlgorithms.
    /// Нужно ее открыть и нажать Play в редакторе, чтобы увидеть результат в консоли.
    /// </summary>
    public class DefinedMetroLoader : MonoBehaviour {
        public void Awake() {
            
            // Ниже будет вручную созданная сеть станций и веток метро.
            // На самом деле "получить" ее можно разными способами.
            // Помимо такого грубого процедурного подхода, можно использовать сериализованные форматы данных,
            // типа JSON, XML, либо же Scriptable Object для описания сети метро.
            // Можно даже в рантайме генерировать сетки метро.
            // Можно также выделить интерфейс, типа IMetroSystem и реализовать его для того же JSON, XML, Scriptable Object,
            // чтобы можно было более гибко добавлять новые способы загрузить сеть метро в приложение.
            // Например, динамически подгрузив такую сеть метро через интернет.
            
            
            // Список всех станций метро из рисунка с тестовым заданием
            MetroStation a = new MetroStation("A");
            MetroStation b = new MetroStation("B");
            MetroStation c = new MetroStation("C");
            MetroStation d = new MetroStation("D");
            MetroStation e = new MetroStation("E");
            MetroStation f = new MetroStation("F");
            MetroStation n = new MetroStation("N");
            MetroStation l = new MetroStation("L");
            MetroStation j = new MetroStation("J");
            MetroStation o = new MetroStation("O");
            MetroStation h = new MetroStation("H");
            MetroStation g = new MetroStation("G");
            MetroStation k = new MetroStation("K");
            MetroStation m = new MetroStation("M");
            
            // Красная ветка метро из рисунка с тестовым заданием
            MetroLine red = new MetroLine("Red", a, b);
            red.AddStation(b, c);
            red.AddStation(d, c);
            red.AddStation(d, e);
            red.AddStation(f, e);
            
            // Синяя ветка метро из рисунка с тестовым заданием
            MetroLine blue = new MetroLine("Blue", n, l);
            blue.AddStation(l, d);
            blue.AddStation(d, j);
            blue.AddStation(j, o);
            
            // Черная ветка метро из рисунка с тестовым заданием
            MetroLine black = new MetroLine("Black", b, h);
            black.AddStation(h, j);
            black.AddStation(j, f);
            black.AddStation(f, g);
            
            // Зеленая ветка метро из рисунка с тестовым заданием
            MetroLine green = new MetroLine("Green", c, j);
            green.AddStation(j, e);
            green.AddStation(e, m);
            green.AddStation(m, l);
            green.AddStation(l, k);
            green.AddStation(k, c);
            
            
            // Теперь, когда сеть метро сгенерирована, есть возможность выполнить поставленную задачу.
            // К слову, тут можно тоже поступить гибко, и выделить интерфес типа IShortestPath.
            // Таким образом, появится гибкость в независимости кода от реализации.
            // Не решился абстрагировать систему, так как не хочется тратить много времени на тестовое.
            

            // Преображаем наше представление в граф
            MetroGraph metroGraph = GenerateGraphFromMetroLines(14, new[] {red, blue, black, green});

            // Теперь, когда система метро полностью задана, можно работать с алгоритмом поиска кратчайшего пути
            
            // ВНИМАНИЕ:
            // Текущая реализация алгоритма НЕ УЧИТЫВАЕТ "лишние" пересадки на новые ветки при поиске кратчайшего пути.
            // Не стал тратить время на эту логику, да и в тестовом задании указано что
            // "Кратчайшим путем считается маршрут с наименьшим количеством промежуточных станций". И ничего про пересадки.

            // Найти кратчайший путь: от станции B к станции G
            PathResults shortest = metroGraph.GetShortestPath("B", "G");
            Debug.Log(shortest);
            // Найти кратчайший путь: от станции L к станции H
            shortest = metroGraph.GetShortestPath("L", "H");
            Debug.Log(shortest);
            // Найти кратчайший путь: от станции A к станции O
            shortest = metroGraph.GetShortestPath("A", "O");
            Debug.Log(shortest);
            // Найти кратчайший путь: от станции C к станции E
            shortest = metroGraph.GetShortestPath("C", "E");
            Debug.Log(shortest);
            // Найти кратчайший путь: от станции F к станции K
            shortest = metroGraph.GetShortestPath("F", "K");
            Debug.Log(shortest);
            
            // Найти кратчайший путь: от станции F к станции K
            shortest = metroGraph.GetShortestPath("D", "D");
            Debug.Log(shortest);
        }


        private MetroGraph GenerateGraphFromMetroLines(int stationsCount, MetroLine[] lines) {
            MetroGraph metroGraph = new MetroGraph(stationsCount);

            foreach (MetroLine line in lines) {
                LinkedListNode<MetroStation> a = line.Stations.First;
                LinkedListNode<MetroStation> b = a.Next;
                
                while (b != null) {
                    metroGraph.AddStation(a.Value);
                    metroGraph.AddStation(b.Value);
                    metroGraph.AddConnection(a.Value.Id, b.Value.Id, line.Id);

                    a = b;
                    b = a.Next;
                }
            }

            return metroGraph;
        }
    }
}