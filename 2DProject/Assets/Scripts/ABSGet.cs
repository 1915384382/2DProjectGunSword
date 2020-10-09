using System.Collections.Generic;
using System;

public class ABSGet<T, S> where S : Skills
{
    protected Dictionary<T, Type> mDicType = new Dictionary<T, Type>();
    protected Dictionary<T, List<S>> mDicPool = new Dictionary<T, List<S>>();
    public virtual void Init() { }

    protected bool RegisterType(T _s, Type _type)
    {
        if (!mDicType.ContainsKey(_s))
        {
            mDicType[_s] = _type;
            return true;
        }
        return false;
    }

    public S GetImpact(T _type)
    {
        List<S> list = null;
        S ret = null;
        if (mDicPool.TryGetValue(_type, out list))
        {
            if (list.Count > 0)
            {
                ret = list[0];
                list.RemoveAt(0);
            }
        }
        if (ret == null)
        {
            Type rettype = null;
            if (mDicType.TryGetValue(_type, out rettype))
            {
                object obj = Activator.CreateInstance(rettype);
                if (obj is S)
                    ret = (S)obj;
            }
        }
        return ret;
    }

    public bool RevertImpact(T _type, S _impact)
    {
        if (_impact == null)
            return false;
        List<S> list = null;
        if (!mDicPool.TryGetValue(_type, out list))
        {
            list = new List<S>();
            mDicPool[_type] = list;
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == _impact)
                return false;
        }
        list.Add(_impact);
        return true;
    }
}

public class SkillsGet : ABSGet<SkillType, Skills>
{
    protected static SkillsGet mInstance;
    public static SkillsGet Instance
    {
        get
        {
            if (mInstance == null)
                mInstance = new SkillsGet();
            return mInstance;
        }
    }

    public static Skills GetSkills(Player _player, SkillType type, int skillCode, string skillName)
    {
        Skills impact = Instance.GetImpact(type);
        if (impact != null)
            impact.InitSkill(_player, type, skillCode, skillName);
        return impact;
    }

    public override void Init()
    {
        RegisterType(SkillType.CrazySword, typeof(CrazySword));
        RegisterType(SkillType.MultipleGun, typeof(CrazySword));
    }
}

//public class WinSkillImpactGet : WinImpactGet<SkillType, WinSkillImpact>
//    {
//        protected static WinSkillImpactGet mInstance;
//        public static WinSkillImpactGet Instance
//        {
//            get
//            {
//                if (mInstance == null)
//                    mInstance = new WinSkillImpactGet();
//                return mInstance;
//            }
//        }

//        public static WinSkillImpact GetSkillImp(WinSoldier _soldier, WinSkillData _data)
//        {
//            WinSkillImpact impact = Instance.GetImpact(_data.skillType);
//            if (impact != null)
//                impact.InitSkill(_soldier, null, _data);
//            return impact;
//        }
//        public static WinSkillImpact GetSkillImp(WinTeam _team, WinSkillData _data)
//        {
//            WinSkillImpact impact = Instance.GetImpact(_data.skillType);
//            if (impact != null)
//                impact.InitSkill(null, _team, _data);
//            return impact;
//        }

//        public override void Init()
//        {
//            RegisterType(SkillType.AddBuff, typeof(AddBuffSkillImpact));
//        }
//    }

//    public class WinBuffImpactGet : WinImpactGet<BuffType, WinBuffImpact>
//    {
//        protected static WinBuffImpactGet mInstance;
//        public static WinBuffImpactGet Instance
//        {
//            get
//            {
//                if (mInstance == null)
//                    mInstance = new WinBuffImpactGet();
//                return mInstance;
//            }
//        }

//        public static WinBuffImpact GetBuffImp(WinSoldier _soldier, WinBuffData _data, WinSoldier _creater)
//        {
//            if (_data == null)
//                return null;
//            WinBuffImpact impact = Instance.GetImpact(_data.buffType);
//            if (impact != null)
//                impact.InitBuff(_soldier, _data, _creater);
//            return impact;
//        }

//        public static WinBuffImpact GetBuffImp(WinTeam _wteam, WinBuffData _data, WinSoldier _creater)
//        {
//            WinBuffImpact impact = Instance.GetImpact(_data.buffType);
//            if (impact != null)
//                impact.InitBuff(_wteam, _data, _creater);
//            return impact;
//        }

//        public override void Init()
//        {
//            RegisterType(BuffType.ShuXing, typeof(ShuXingBuffImpact));
//        }
//    }