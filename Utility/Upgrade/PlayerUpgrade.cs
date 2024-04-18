using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ApproachTheForge.Utility.Upgrade
{
    public class PlayerUpgrade : IUpgrade
    {
        public EntityStatistics UpgradedStat { get; set; }

        public double UpgradeValue { get; set; }

    }
}
