using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

namespace ModulusFE.PaintObjects
{
    internal partial class PaintObjectsManager<T> where T : IPaintObject, new()
    {
        private readonly List<T> _paintObjects = new List<T>();
        private int _paintObjectsCount;

        public Canvas C { get; set; }
        public Action<T> NewObjectCreated { get; set; }

        public int Count
        {
            get { return _paintObjects.Count; }
        }

        public void Start()
        {
            _paintObjectsCount = 0;
        }

        public void Stop()
        {
            for (int i = _paintObjectsCount; i < _paintObjects.Count; i++)
            {
                (_paintObjects[i] as IPaintObject).RemoveFrom(C);
            }

            _paintObjects.RemoveRange(_paintObjectsCount, _paintObjects.Count - _paintObjectsCount);
        }

        public T this[int index]
        {
            get
            {
                Debug.Assert(index < _paintObjects.Count);
                return _paintObjects[index];
            }
        }

        public void Do(Action<T> action)
        {
            _paintObjects.ForEach(action);
        }

        public T GetPaintObject()
        {
            return GetPaintObject(null);
        }

        public T GetPaintObject(params object[] args)
        {
            Debug.Assert(C != null);
            if (_paintObjectsCount >= _paintObjects.Count)
            {
                T paintObject = new T();
                paintObject.SetArgs(args);
                _paintObjects.Add(paintObject);
                (paintObject as IPaintObject).AddTo(C);

                if (NewObjectCreated != null)
                {
                    NewObjectCreated(paintObject);
                }
            }

            return _paintObjects[_paintObjectsCount++];
        }

        public void RemoveAll()
        {
            foreach (var paintObject in _paintObjects)
            {
                (paintObject as IPaintObject).RemoveFrom(C);
            }

            _paintObjects.Clear();
        }
    }
}

