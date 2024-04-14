using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Text;
using System.Threading.Tasks;

namespace ApproachTheForge
{
    public struct DamageData
    {
        // The amount of damage done by the attack
        public float Damage;

        // The amount of knockback given by the attack
        public Vector2 Knockback;
    }
}
