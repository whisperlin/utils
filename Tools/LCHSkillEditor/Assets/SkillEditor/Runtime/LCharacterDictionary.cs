using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LCharacterDictionary<T>  {

    public Dictionary<int, T> dictionary = new Dictionary<int, T>();
    public List<T> list = new List<T>();

    public void Add(int k,T v)
    {
        if (!dictionary.ContainsKey(k))
        {
            dictionary[k] = v;
            list.Add(v);
        }
    }
    public void Remove(int k)
    {
        T v;
        if (dictionary.TryGetValue(k, out v))
        {
            dictionary.Remove(k);
            list.Remove(v);
        } 
    }
}
