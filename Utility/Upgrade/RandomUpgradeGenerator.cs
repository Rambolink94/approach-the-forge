using Godot;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace ApproachTheForge.Utility.Upgrade
{
    public class RandomUpgradeGenerator
    {

        public UpgradeWrapper<T> GenerateRandomUpgradeWrapper<T, U>(U randomStat)
            where T : IUpgrade, new()
            where U : IRandomGeneratorStrategy
        {
            ResourceType resourceType = this.GenerateRandomResourceCost();
            int resourceCost = this.GenerateRandomCost();
            // The resulting upgrade will weighted to higher and lower values based on the 
            // quantity of more rare and more common materials, respectively.
            int upgradeValue = this.GenerateRandomUpgradeValue(resourceCost * ((int)resourceType + 1));
            EntityStatistics statToUpgrade = randomStat.RandomStatToUpgrade();

            return new UpgradeWrapper<T>()
            {
                Upgrade = new T()
                {
                    UpgradeValue = upgradeValue,
                    UpgradedStat = statToUpgrade,
                },
                Cost = resourceCost,
                ResourceType = resourceType,
            };
        }

        private ResourceType GenerateRandomResourceCost()
        {
            RandomNumberGenerator random = new RandomNumberGenerator();

            int randomThree = random.RandiRange(1, 3);

            return (ResourceType)randomThree;
        }

        private int GenerateRandomCost()
        {
            RandomNumberGenerator random = new RandomNumberGenerator();

            return random.RandiRange(1, 5);
        }

        private int GenerateRandomUpgradeValue(int totalResourceCost)
        {
            RandomNumberGenerator random = new RandomNumberGenerator();

            // We want to generate a random number between 5 and 35 in intervals of 5. (5, 10, 15, etc...)
            // The closer the cost is to 15, the more likely 35 should be
            // The closer the cost is to 1, the more likely 5 should be
            
            // Project the range [1, 15] to [1,7] by dividing by the width of original range and multiplying
            // by the width of the desired range
            float projectedCost = totalResourceCost / 15.0f * 7.0f;

            // Use the projected cost as the mean of the normal distribution
            // The standard deviation of the normal distrubution will be 1 such that 68% of the time the player
            // will get an upgrade that corresponds within 1 tier of the rolled cost.
            // The upgrade value will be rounded to the nearest whole number this way the result corresponds
            // exactly to a desired upgrade value
            int upgradeValue = (int)Math.Round(random.Randfn(projectedCost, 1));

            if(upgradeValue > 7)
            {
                upgradeValue = 7;
            }
            else if(upgradeValue < 1)
            {
                upgradeValue = 1;
            }
            // Multiply the value by 5 to convert the range [1,7] to [5,35]
            return upgradeValue * 5;
        }
    }

    public interface IRandomGeneratorStrategy
    {
        public EntityStatistics RandomStatToUpgrade();
    }

    public class RandomPlayerStat : IRandomGeneratorStrategy
    {
        private Dictionary<int, EntityStatistics> IntToStatistic => new Dictionary<int, EntityStatistics>()
        {
            { 0, EntityStatistics.Health},
            { 1, EntityStatistics.Damage},
            { 2, EntityStatistics.Speed},
            { 3, EntityStatistics.Rate_Of_Fire},
        };

        public EntityStatistics RandomStatToUpgrade()
        {
            RandomNumberGenerator random = new RandomNumberGenerator();

            int randomFour = random.RandiRange(0, 3);

            return this.IntToStatistic[randomFour];
        }
    }

    public class RandomTowerStat : IRandomGeneratorStrategy
    {
        private Dictionary<int, EntityStatistics> IntToStatistic => new Dictionary<int, EntityStatistics>()
        {
            { 0, EntityStatistics.Health},
            { 1, EntityStatistics.Damage},
            { 2, EntityStatistics.Rate_Of_Fire},
        };

        public EntityStatistics RandomStatToUpgrade()
        {
            RandomNumberGenerator random = new RandomNumberGenerator();

            int randomThree = random.RandiRange(0, 2);

            return this.IntToStatistic[randomThree];
        }
    }

    public class RandomGolemStat : IRandomGeneratorStrategy
    {
        private Dictionary<int, EntityStatistics> IntToStatistic => new Dictionary<int, EntityStatistics>()
        {
            { 0, EntityStatistics.Health},
            { 1, EntityStatistics.Damage},
            { 2, EntityStatistics.Speed},
        };

        public EntityStatistics RandomStatToUpgrade()
        {
            RandomNumberGenerator random = new RandomNumberGenerator();

            int randomThree = random.RandiRange(0, 2);

            return this.IntToStatistic[randomThree];
        }
    }
}
