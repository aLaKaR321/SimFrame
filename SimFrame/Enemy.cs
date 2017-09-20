using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimFrame
{
    class Enemy
    {
        public Health Health;
        public Armor Armor;
        public List<Proc> procList;
        public Enemy(string name, int baseLevel, int currentLevel, int baseHp, string healthType)
        {
            procList = new List<Proc>();
            Health = new Health(healthType, baseLevel, currentLevel, baseHp);
        }
        public Enemy(string name, int baseLevel, int currentLevel, int baseHp, string healthType, int baseArmor, string armorType)
            : this(name, baseLevel, currentLevel, baseHp, healthType)
        {
            Armor = new Armor(armorType, baseLevel, currentLevel, baseArmor);
        }

        public List<SimEvent> simShot(WeaponStats weaponStats, double activationTime, List<SimEvent> currentEvents)
        {
            List<SimEvent> eventList = new List<SimEvent>();
            var critMultiplier = weaponStats.simCritMultiplier();
            bool statusProcced = weaponStats.simStatusChance();
            string statusEffect = "";
            bool multishot = weaponStats.simMultishot();
            if (multishot)
            {
                eventList.Add(new SimShot(activationTime, this, currentEvents, weaponStats));
            }
            if (statusProcced)
            {
                statusEffect = weaponStats.simStatusProc();
                if(statusEffect == "Corrosive") //Corrosive First 
                {
                    eventList.AddRange(simNewProc(weaponStats, activationTime, statusEffect, critMultiplier, currentEvents));
                }
            }
            simWeaponDamage(weaponStats, critMultiplier);
            if (statusProcced && statusEffect != "Corrosive") //Viral Magnetic and others after
            {
                eventList.AddRange(simNewProc(weaponStats, activationTime, statusEffect, critMultiplier, currentEvents));
            }
            
            return eventList;
        }

        private List<SimProc> simNewProc(WeaponStats weaponStats, double activationTime, string statusEffect, double critMultiplier, List<SimEvent> currentEvents)
        {
            List<SimProc> procList = new List<SimProc>();
            switch (statusEffect)
            {
                case "Corrosive":
                    Armor.CurrentAmount *= 0.75;
                    procList.Add(new SimProc(activationTime + 8, this, currentEvents, "Corrosive"));
                    break;
                case "Viral":
                    bool alreadyActive = (currentEvents.Count(x => x.GetType() == typeof(SimProc) && (x as SimProc)._statusType == "Viral") > 0);
                    if (!alreadyActive)
                    {
                        Health.CurrentAmount *= 0.5;
                    }
                    procList.Add(new SimProc(activationTime + 8, this, currentEvents, "Viral"));
                    break;
                case "Toxin":
                    for(int i = 0; i<9; i++)
                    {
                        procList.Add(new DamageProc(activationTime + i, this, currentEvents, "Toxin", ( weaponStats.BaseDamage + weaponStats.DamageDictionary["Toxin"] ) / 2 * critMultiplier));
                    }
                    break;
                case "Slash":
                    for (int i = 0; i < 7; i++)
                    {
                        procList.Add(new DamageProc(activationTime + i, this, currentEvents, "Finisher", weaponStats.BaseDamage * 0.35 * critMultiplier));
                    }
                    break;
                case "Heat":
                    currentEvents.RemoveAll(x => x.GetType() == typeof(SimProc) && (x as SimProc)._statusType == "Heat");
                    for (int i = 0; i < 7; i++)
                    {
                        procList.Add(new DamageProc(activationTime + i, this, currentEvents, "Heat", (weaponStats.BaseDamage + weaponStats.DamageDictionary["Heat"]) / 2 * critMultiplier));
                    }
                    break;
                default:
                    break;
            }
            return procList;
        }

        private double simWeaponDamage(WeaponStats weaponStats, double critMultiplier)
        {
            double sum = 0;
            foreach (KeyValuePair<string, double> kvp in weaponStats.DamageDictionary)
            {
                var dmg = kvp.Value * critMultiplier;
                sum += calculateDamage(kvp.Key, dmg);
            }
            return sum;
        }
        public double calculateShot(WeaponStats weaponStats)
        {
            double sum = 0;
            sum += calculateExistingProcs(weaponStats);
            sum += calculateNewProcs(weaponStats);
            sum += calculateWeaponDamage(weaponStats);
            return sum;
        }

        private double calculateWeaponDamage(WeaponStats weaponStats)
        {
            double sum = 0;
            foreach (KeyValuePair<string, double> kvp in weaponStats.DamageDictionary)
            {
                var dmgCrit = kvp.Value * weaponStats.getAverageCritMultiplier();
                sum += calculateDamage(kvp.Key, dmgCrit);
            }
            return sum;
        }

        private double calculateExistingProcs(WeaponStats weaponStats)
        {
            double sum = 0;
            foreach (Proc proc in procList)
            {
                sum += calculateDamage(proc.damageType, proc.damagePerShot);
                proc.timeLeft -= 1 / weaponStats.ShotsPerSecond;
            }
            procList.RemoveAll(x => x.timeLeft <= 0);
            return sum;
        }
        private Proc GetProc(string damageType, WeaponStats weaponStats, double weight, double timeBetweenTicks, double timeToLive, double baseDamagePerTick, double typeDamagePerTick, out double initialDamage)
        {
            var timeBetweenShots = (1 / weaponStats.ShotsPerSecond);
            double weightedDamage = 0;
            if (typeDamagePerTick > 0)
            {
                weightedDamage = (weaponStats.BaseDamage * baseDamagePerTick + weaponStats.DamageDictionary[damageType] * typeDamagePerTick)
                                * weight
                                * weaponStats.StatusChance
                                * weaponStats.getAverageCritMultiplier();
            }
            else
            {
                weightedDamage = (weaponStats.BaseDamage * baseDamagePerTick)
                * weight
                * weaponStats.StatusChance
                * weaponStats.getAverageCritMultiplier();
            }
            initialDamage = calculateDamage(damageType, weightedDamage); //Initial Tick
            var proc = new Proc
            {
                timeLeft = timeToLive,
                damageType = damageType,
                damagePerShot = weightedDamage * (timeBetweenShots / timeBetweenTicks)
            };
            return proc;
        }
        private double calculateNewProcs(WeaponStats weaponStats)
        {
            double sum = 0;
            Dictionary<string,double> weightDict = weaponStats.getStatusWeights();
            double weightSum = weightDict.Sum(x => x.Value);
            foreach (var weight in weightDict)
            {
                double initialDamage = 0;
                switch (weight.Key)
                {
                    case "Corrosive":
                        this.Armor.CurrentAmount -= this.Armor.CurrentAmount * (weightDict[weight.Key] / weightSum) * 0.25;
                        break;
                    case "Toxin":
                        procList.Add(GetProc(damageType: weight.Key, weaponStats: weaponStats, weight: weight.Value / weightSum, timeBetweenTicks: 1, timeToLive: 8, baseDamagePerTick: 0.5, typeDamagePerTick: 0.5, initialDamage: out initialDamage));
                        break;
                    case "Slash":
                        procList.Add(GetProc(damageType: "Finisher", weaponStats: weaponStats, weight: weight.Value / weightSum, timeBetweenTicks: 1, timeToLive: 6, baseDamagePerTick: 0.35, typeDamagePerTick: 0, initialDamage: out initialDamage));
                        break;
                    case "Heat":
                        procList.Add(GetProc(damageType: weight.Key, weaponStats: weaponStats, weight: weight.Value / weightSum, timeBetweenTicks: 1, timeToLive: 6, baseDamagePerTick: 0.5, typeDamagePerTick: 0.5, initialDamage: out initialDamage));
                        break;
                    case "Electricity":
                        var procDmg = (weaponStats.BaseDamage + weaponStats.DamageDictionary[weight.Key]) / 2;
                        var finalDmg = procDmg * (weight.Value / weightSum) * weaponStats.StatusChance * weaponStats.getAverageCritMultiplier();
                        initialDamage = calculateDamage(weight.Key, finalDmg);
                        break;
                    //case "Gas":
                    //    var procDmg = (weaponStats.BaseDamage + weaponStats.DamageDictionary[weight.Key]) / 2;
                    //    var finalDmg = procDmg * (weight.Value / weightSum) * weaponStats.StatusChance * weaponStats.getAverageCritMultiplier();
                    //    initialDamage = calculateDamage(weight.Key, finalDmg);
                    //    break;
                    default:
                        break;
                }
                sum += initialDamage;
            }
            return sum;
        }
        private double calculateDamage(string damageType, double damageValue)
        {
            double remainingHealthDamage = 1;
            if (this.Armor != null && damageType != "Finisher")
            {
                double netArmor = this.Armor.CurrentAmount * (1 - DataHelper.DamageTypeDictionary[damageType][this.Armor.Type]);
                remainingHealthDamage = 1 - netArmor / (300 + netArmor);
            }
            var modifier = DataHelper.DamageTypeDictionary[damageType][this.Health.Type];
            var damage = (1 + modifier) * damageValue * remainingHealthDamage;
            this.Health.CurrentAmount -= damage;
            return damage;
        }
    }
}
