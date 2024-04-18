using ApproachTheForge.Utility.Upgrade;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApproachTheForge.Utility
{
    public partial class UpgradeManager : Node2D
    {
        private List<IUpgrade> Upgrades { get; set; } = new List<IUpgrade>();

        public override void _Ready()
        {
            base._Ready();
        }

        public void AddUpgrade(IUpgrade upgrade)
        {
            Upgrades.Add(upgrade);
        }

        public List<T> GetUpgrades<T>()
            where T : IUpgrade
        {
            return Upgrades.OfType<T>().ToList();
        }
    }
}
