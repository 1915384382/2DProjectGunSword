using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private static SkillManager minstance;
    public static SkillManager Instance
    {
        get {
            if (minstance == null)
            {
                return new SkillManager();
            }
            return minstance;
        }
    }
    public List<Skills> AllSkill = new List<Skills>();



    public void SetSkillButton(int skillCode,string buttonCode) 
    {
        if (buttonCode.Equals("1"))
        {
            skillCode = 1;
        }
        if (buttonCode.Equals("2"))
        {
            skillCode = 2;
        }
        if (buttonCode.Equals("3"))
        {
            skillCode = 3;
        }
        if (buttonCode.Equals("4"))
        {
            skillCode = 4;
        }
        if (buttonCode.Equals("5"))
        {
            skillCode = 5;
            Skills crazy = SkillsGet.Instance.GetImpact(SkillType.CrazySword);
            AllSkill.Add(crazy);
            AllSkill.Add(new CrazySword());
        }
    }
    public KeyCode GetButtonCode(int skillCode)
    {
        if (skillCode == 1)
        {
            return KeyCode.Alpha1;
        }
        return KeyCode.None;
    }
    public void UseSkill(int skillCode)
    {
        UseSkill(GetSkillType(skillCode));
    }
    SkillType GetSkillType(int skillCode) 
    {
        if (skillCode == 1)
            return SkillType.CrazySword;
        else if (skillCode == 2)
            return SkillType.MultipleGun;
        return SkillType.CrazySword;
    }
    public void UseSkill(SkillType type) 
    {
        switch ((SkillType)type)
        {
            case SkillType.MultipleGun:

                break;
            case SkillType.CrazySword:
                break;
            default:
                break;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
