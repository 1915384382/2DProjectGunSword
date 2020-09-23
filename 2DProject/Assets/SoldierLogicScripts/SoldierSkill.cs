using System;
using UnityEngine;

namespace ShawLogic
{
    public class SoldierSkill : SoldierABSBase
    {
        public SkillType skillType;
        public void InitSkill(Soldier _soldier) 
        {
            InitABS(_soldier);
        }
        public void Init(Soldier _soldier,SkillType _skillType)
        {
            soldier = _soldier;
            skillType = _skillType;
            id = GameManager.Instance.GetInstance();
        }
    }
}
