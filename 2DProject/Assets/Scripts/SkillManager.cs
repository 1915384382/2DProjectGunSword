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
