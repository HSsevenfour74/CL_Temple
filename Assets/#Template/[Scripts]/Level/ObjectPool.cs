using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ConceptGames.ConceptLineOrion.Level
{
    public class ObjectPool<T> where T : Object
    {
        private readonly Queue<T> pool = new Queue<T>();
        private int size;

        public int Size
        {
            get => size;
            set => size = value;
        }

        public void ClearAll()
        {
            foreach (var t in pool) Object.Destroy(t.GameObject());
            pool.Clear();
        }

        public void Add(T t)
        {
            pool.Enqueue(t);
        }

        public T First()
        {
            return pool.Dequeue();
        }

        public bool Full => pool.Count >= size;
    }
}