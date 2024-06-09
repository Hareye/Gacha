using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using interfaces;
using classes;

public class Skill : MonoBehaviour
{
    private ISkill skill;

    void Awake()
    {
        skill.useSkill(gameObject);
    }

    public void setSkill(ISkill newSkill)
    {
        skill = newSkill;
    }
}
