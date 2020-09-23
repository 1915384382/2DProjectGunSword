using System;
using UnityEngine;
using System.Collections.Generic;

namespace ShawLogic
{
    public class Soldier
    {
        public Soldier soldier;
        public PropertySet propertySet;
        public List<SoldierAbility> abilitys = new List<SoldierAbility>();
        public List<SoldierBuff> buffs = new List<SoldierBuff>();
        public List<SoldierSkill> skills = new List<SoldierSkill>();

        public void InitSoldier(Soldier _soldier,List<SoldierAbility> _abilitys,List<SoldierSkill> _skills) 
        {
            soldier = _soldier;
            abilitys = _abilitys;
            skills = _skills;
        }
        public void GetProperty(SoldierPropertyType type)//���һ������
        {
            propertySet.GetProperty((int)type);
        }
        public void SetProperty(SoldierPropertyType type, float value)//���û�����һ������
        {
            propertySet.SetOrAddProperty((int)type, value);
        }
        public void SetPropertySet(PropertySet propertySet)//����ȫ������
        {
            propertySet.SetPropertySet(propertySet);
        }
        public void Attack(Soldier _target) 
        {


        }
    }
}
