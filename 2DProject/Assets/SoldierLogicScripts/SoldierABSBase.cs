using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShawLogic
{
    public class SoldierABSBase
    {
        public Soldier soldier;
        public long id;
        public PropertySet propertySet = new PropertySet();
        public ParamSet paramSet = new ParamSet();
        public void InitProperty(PropertySet set) 
        {
            SetPropertySet(set);
        }
        public void InitABS(Soldier _soldier)
        {
            soldier = _soldier;
        }
        public virtual void GetProperty(SoldierPropertyType type)//获得一条属性
        {
            propertySet.GetProperty((int)type);
        }
        public virtual void SetProperty(SoldierPropertyType type,float value)//设置或增加一条属性
        {
            propertySet.SetOrAddProperty((int)type,value);
        }
        public virtual void SetPropertySet(PropertySet propertySet)//设置全部属性
        {
            propertySet.SetPropertySet(propertySet);
        }
        public virtual void GetParam(ParamType type)//获得一个数据
        {
            paramSet.GetParamSet((int)type);
        }
        public virtual void SetParam(ParamType type,int _value)//设置一个数据 
        {
            paramSet.SetParamSet((int)type,_value);
        }
        public virtual void ClearParam()//清除全部param
        {
            paramSet.ClearParamSet();
        }


    }
}