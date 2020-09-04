using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicalBox : Item
{
    private int healValue;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _type = ItemType.MedicalBox;
        _name = "回復薬";
        healValue = 10;
    }

    public override void Use(Character character)
    {
        base.Use(character);

        character.Heal(healValue);

        Destroy(gameObject);
    }

    public override bool EnemyWhetherToUse(Enemy enemy)
    {
        if (enemy._hp < (enemy._maxHp / 2))
        {
            return true;
        }

        return false;
    }
}
