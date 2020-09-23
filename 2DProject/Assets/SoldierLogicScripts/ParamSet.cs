using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShawLogic
{
    public class ParamSet
    {
        Dictionary<int, int> paramSet = new Dictionary<int, int>();
        public void ClearParamSet()
        {
            paramSet.Clear();
        }
        public void SetParamSet(int _key,int _value) 
        {
            int value;
            if (!paramSet.TryGetValue(_key, out value))
                paramSet[_key] = value;
            else
                paramSet[_key] = value;
        }
        public int GetParamSet(int _key)
        {
            int value;
            if (paramSet.TryGetValue(_key, out value))
                return value;
            else
                return 0;
        }


    }
}