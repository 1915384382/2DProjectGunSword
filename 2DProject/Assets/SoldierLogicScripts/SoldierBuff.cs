using System;
using UnityEngine;

namespace ShawLogic
{
    public class SoldierBuff : SoldierABSBase
    {
        public BuffType buffType;
        public void Init(Soldier _soldier, BuffType _buffType)
        {
            soldier = _soldier;
            buffType = _buffType;
            id = GameManager.Instance.GetInstance();
        }
    }
}
