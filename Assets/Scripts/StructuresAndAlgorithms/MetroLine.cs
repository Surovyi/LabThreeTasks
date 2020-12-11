using System.Collections.Generic;
using UnityEngine;

namespace StructuresAndAlgorithms {
    /// <summary>
    /// Ветка метро.
    /// Используется как набор рёбер в графе для алгоритма поиска кратчайшего пути.
    /// </summary>
    public class MetroLine {
        public readonly string Id;
        /// <summary>
        /// Является ли ветка метро зацикленной (тоесть начало и конец это одна и та же станция)
        /// </summary>
        public bool Looped;
        /// <summary>
        /// Использую связный список, так как он очень удобен в работе с "цепями" объектов
        /// </summary>
        public readonly LinkedList<MetroStation> Stations;

        public MetroLine(string id, MetroStation stationA, MetroStation stationB) {
            Id = id;
            Looped = false;
            Stations = new LinkedList<MetroStation>();
            Stations.AddFirst(stationA);
            Stations.AddLast(stationB);
        }

        /// <summary>
        /// Добавляет станцию в линию метро.
        /// </summary>
        /// <param name="from">Из какой станции идёт связь к новой станции в линии</param>
        /// <param name="to">Новая станция, которая добавляется в линию</param>
        /// <returns></returns>
        public bool AddStation(MetroStation from, MetroStation to) {
            // Если станция уже закольцована, то нельзя добавить к ней новую станцию
            if (Looped == true) {
                Debug.LogError("Already in loop! Cannot add new station to this line!");
                return false;
            }

            // Если хотя бы одной из указанных станций нет в списке уже подсоединенных к линии станций, то нельзя добавить новую станцию
            if (Stations.Contains(from) == false && Stations.Contains(to) == false) {
                Debug.LogError($"Incorrect 'parent' station! Line doesn't contain station {from.Id}");
                return false;
            }

            // Блокируем Т-образные перекрестки. Станции можно добавлять только к краям линии.
            if (Stations.First.Value.Id != from.Id) {
                if (Stations.Last.Value.Id != from.Id) {
                    if (Stations.First.Value.Id != to.Id) {
                        if (Stations.Last.Value.Id != to.Id) {
                            Debug.LogError($"Wrong 'parent' station! Station {from.Id} is in the middle of the line!");
                            return false;
                        } else {
                            Stations.AddLast(from);
                        }
                    } else {
                        Stations.AddFirst(from);
                    }
                } else {
                    Stations.AddLast(to);
                }
            } else {
                Stations.AddFirst(to);
            }
            

            // Если первая и последняя станция одинаковы - то имеем закольцованную линию метро
            if (Stations.First.Value.Id == Stations.Last.Value.Id) {
                Looped = true;
            }

            return true;
        }
    }
}
