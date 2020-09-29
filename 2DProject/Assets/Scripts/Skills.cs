using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    None,
    MultipleGun,
    CrazySword,
}
public class Skills : MonoBehaviour
{
    public Player player;
    public SkillType skillType;
    public int skillCode;
    public string skillName;

    public virtual void InitSkill(Player _player, SkillType _skillType,int _skillcode,string _skillName) 
    {
        player = _player;
        skillType = _skillType;
        skillCode = _skillcode;
        skillName = _skillName;
        SkillPool.Instance.GetSkill(skillName,player.transform);
    }

    public void UseSkill(int type)
    {
        SkillManager.Instance.UseSkill((SkillType)type);
    }
    protected bool isRunning;
    public virtual void OnEnter() { isRunning = true; }
    public virtual void OnFinish() { isRunning = false; }
    public void RegistSkill(SkillType type) 
    {
        switch (type)
        {
            case SkillType.None:
                CrazySword sword = new CrazySword();
                break;
            case SkillType.MultipleGun:
                break;
            case SkillType.CrazySword:
                break;
            default:
                break;
        }
    }
}
public class CrazySword : Skills
{
    public override void InitSkill(Player _player, SkillType _skillType, int _skillcode, string _skillName)
    {
        base.InitSkill(_player, _skillType, _skillcode, _skillName);


    }
    public override void OnEnter()
    {
        base.OnEnter();

    }
    public override void OnFinish()
    {
        base.OnFinish();

    }
}
