using System;
using UnityEngine;

using IntType = ProtectInt;


namespace ShawLogic
{
    public class SoldierAbility : SoldierABSBase
    {
        public AbilityType abilityType;
        public void Init(Soldier _soldier, AbilityType _abilityType)
        {
            soldier = _soldier;
            abilityType = _abilityType;
            id = GameManager.Instance.GetInstance();
        }



    }
}
