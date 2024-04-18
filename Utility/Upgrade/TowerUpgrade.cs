using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApproachTheForge.Utility.Upgrade
{
    public class TowerUpgrade : IUpgrade
    {
        public EntityStatistics UpgradedStat { get; set; }

        public double UpgradeValue { get; set; }

    }
}
