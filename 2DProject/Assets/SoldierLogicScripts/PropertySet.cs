using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertySet
{
    public Dictionary<int, float> propertys = new Dictionary<int, float>();

    public PropertySet() 
    {

    }
    public float GetProperty(int _index) 
    {
        float value;
        if (propertys.TryGetValue(_index, out value))
            return value;
        else
            return 0;
    }
    public void SetOrAddProperty(int _index,float _value ) 
    {
        float value;
        if (propertys.TryGetValue(_index, out value))
        {
            propertys[_index] = _value;
        }
        else
        {
            propertys[_index] = _value;
        }
    }
    public void Clear() 
    {
        propertys.Clear();
    }
    public void SetPropertySet(PropertySet _set)
    {
        Clear();
        if (_set != null)
        {
            foreach (KeyValuePair<int, float> itr in _set.propertys)
            {
                SetOrAddProperty(itr.Key, itr.Value);
            }
        }
    }
}
