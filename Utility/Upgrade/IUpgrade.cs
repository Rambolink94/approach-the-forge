using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApproachTheForge.Utility.Upgrade
{
    public interface IUpgrade
    {
        public EntityStatistics UpgradedStat { get; set; }

        /// <summary>
        ///     UpgradeValue is a whole number between 1 and 100 representing a
        ///     percentage out of 100. If UpgradeValue is 5, the stat will
        ///     increase by 5%.
        /// </summary>
        public double UpgradeValue { get; set; }

    }
}
