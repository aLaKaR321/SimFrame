using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimFrame
{
    class WeaponStats
    {
        public Dictionary<string, double> DamageDictionary;
        public double ShotsPerSecond;
        public double MagazineSize;
        public double CurrentMagazine;

        public WeaponStats()
        {
            DamageDictionary = new Dictionary<string, double>();
        }
        public double ReloadTime { get; internal set; }
        public double StatusChance { get; internal set; }
        public double BaseDamage { get; internal set; }
        public double CritChance { get; internal set; }
        public double CritMultiplier { get; internal set; }
        public double Multishot { get; internal set; }
        public double getAverageCritMultiplier()
        {
            return (1 - CritChance) + (CritChance * CritMultiplier);
        }

        internal Dictionary<string, double> getStatusWeights()
        {
            var weightDict = new Dictionary<string, double>();
            foreach (var damageKvp in DamageDictionary)
            {
                double value = damageKvp.Value;
                if (DataHelper.physicalList.Contains(damageKvp.Key))
                {
                    value = 4 * damageKvp.Value;
                }
                weightDict.Add(damageKvp.Key, value);
            }
            return weightDict;
        }

        internal string simStatusProc()
        {
            var dict = getStatusWeights();
            var weightSum = 0;
            var weightDict = new Dictionary<string, int>();
            foreach (var item in dict)
            {
                weightSum += (int)Math.Round(item.Value, 0);
                weightDict.Add(item.Key, weightSum);
            }
            int rng = DataHelper.Random.Next(0, weightSum);
            var selectedProc = "";
            foreach (var weight in weightDict)
            {
                if (rng < weight.Value)
                {
                    selectedProc = weight.Key;
                    break;
                }
            }
            return selectedProc;
        }

        internal bool simStatusChance()
        {
            double rng = DataHelper.Random.NextDouble();
            return (rng < StatusChance);
        }

        internal bool simMultishot()
        {
            double rng = DataHelper.Random.NextDouble();
            return (rng < Multishot);
        }

        public double getTimeBetweenShots()
        {
            return 1 / ShotsPerSecond;
        }
        public double simCritMultiplier()
        {
            int critLevel = (int)Math.Floor(CritChance);
            double remaingCC = CritChance - critLevel;
            double rng = DataHelper.Random.NextDouble();
            if (rng < CritChance)
            {
                critLevel++;
            }
            var dmgMult = critLevel * ( CritMultiplier - 1 ) + 1;
            return dmgMult;
        }
    }
}
