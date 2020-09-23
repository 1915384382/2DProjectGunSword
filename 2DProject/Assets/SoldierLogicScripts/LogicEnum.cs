using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ShawLogic
{
    public enum AbilityType
    {

    }
    public enum BuffType
    {

    }
    public enum SkillType
    {

    }
    public enum SoldierPropertyType
    {
        Attack,
        Defence,
        MagicDefence,
        MoveSpeed,
        RunSpeed,
        SlowMoveSpeed,
        JumpHeight,

        MaxHp = 1,
        MaxHpRate = 2,
        PhysicalAttack = 3,     //物理攻击力
        PhysicalAttackRate = 4, //物理攻击增加率
        PhysicalDefense = 5,    //物理防御
        PhysicalDefenseRate = 6,//物理防御增加率
        PhysicalDefenseSubRate = 7,//减对方物理防御
        PhysicalDamageAdd = 8,  //物理增伤
        PhysicalDamageSub = 9,  //物理减伤
        MagicDefense = 10,      //魔法防御
        MagicDefenseRate = 11,  //魔法防御增加率
        MagicDefenseSubRate = 12, //减对方法术防御
        MagicDamageAdd = 13, //魔法增伤
        MagicDamageSub = 14, //魔法减伤
        Dodge = 15,

        Hit = 17,
    }
    public enum ParamType
    {
        probabilityParam,
        intParam,
        rateParam,
        countParam,
        buffParam,
        buffParamEx,
        roundParam,
        overlyingParam,
        propertyParam,
        rangeParam,
        targetParam,
        removeParam,
        flagParam,
    }
    public enum PropertyAtkType
    {
        Physical = 0,
        Magic = 1,
    }
}
public class AllEnum 
{
    


}
