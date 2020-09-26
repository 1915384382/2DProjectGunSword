using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    MultipleGun,
    CrazySword,
}
public class Skills : MonoBehaviour
{
    public SkillType skillType;
    public void InitSkill(SkillType _skillType) 
    {
        skillType = _skillType;
    }

    public void UseSkill(int type)
    {
        SkillManager.Instance.UseSkill((SkillType)type);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    bool isUsingSkill = false;
    // Update is called once per frame
    void Update()
    {
        if (!isUsingSkill)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                UseSkill(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                UseSkill(1);
            }
            isUsingSkill = true;
        }
    }
}
