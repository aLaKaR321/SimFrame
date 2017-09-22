using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimFrame
{
    class helper
    {
        public static Enemy getEnemy()
        {
            return new Enemy(
                name: "Lancer",
                baseLevel: 1,
                currentLevel: 50,
                baseHp: 100,
                healthType: "Cloned Flesh",
                baseArmor: 100,
                armorType: "Ferrite Armor"
                );
        }
        public static WeaponStats getWeaponStats()
        {
            var weaponStats =  new WeaponStats
            {
                ShotsPerSecond = 2,
                MagazineSize = 20,
                ReloadTime = 3,
                CritChance = 0.1,
                CritMultiplier = 2,
                BaseDamage = 300,
                StatusChance = 0.5,
                Multishot = 0.9,
                CurrentMagazine = 2
            };
            weaponStats.DamageDictionary.Add("Slash", 100);
            weaponStats.DamageDictionary.Add("Impact", 100);
            weaponStats.DamageDictionary.Add("Puncture", 100);
            weaponStats.DamageDictionary.Add("Corrosive", 100);
            weaponStats.DamageDictionary.Add("Toxin", 100);
            weaponStats.DamageDictionary.Add("Heat", 100);
            return weaponStats;
        }
        public static void DoStuff()
        {
            var enemy = getEnemy();
            var weaponStats = getWeaponStats();
            
            List<Simulation> test = new List<Simulation>();
            DateTime start = DateTime.Now;
            for (int i = 0; i < 10000; i++)
            {
                test.Add(new Simulation(getEnemy(), getWeaponStats()));
            }
            DateTime doneWithInit = DateTime.Now;
            test.AsParallel().ForAll(x => x.Execute());
            DateTime endTime = DateTime.Now;
            var initTime = doneWithInit - start;
            var executeTime = endTime - doneWithInit;
            var totalTime = initTime + executeTime;
            //var sim = new Simulation(enemy, weaponStats);
            //sim.Execute();
            //var testResult = GetDamageModel(weaponStats, enemy);
            //var testResult = SimDamageModel(weaponStats, enemy);
        }
        public static DamageModel GetDamageModel(WeaponStats weaponStats, Enemy enemy)
        {
            DamageModel damageModel = new DamageModel();
            List<Proc> procList = new List<Proc>();
            for (damageModel.ShotsToKill = 0; enemy.Health.CurrentAmount > 0; damageModel.ShotsToKill++)
            {
                damageModel.TimeToKill += 1 / weaponStats.ShotsPerSecond;
                damageModel.ShotsFired.Add(enemy.calculateShot(weaponStats));
            }
            damageModel.MagazinesToKill = damageModel.ShotsToKill / weaponStats.MagazineSize;
            damageModel.TimeToKill += Math.Floor(damageModel.MagazinesToKill) * weaponStats.ReloadTime;
            damageModel.DamageDone = damageModel.ShotsFired.Sum();
            damageModel.DpsToKill = damageModel.DamageDone / damageModel.TimeToKill;
            return damageModel;
        }
    }
    public class Proc
    {
        public double timeLeft;
        public string damageType;
        internal double damagePerShot;
    }
    public class DamageModel
    {
        public int ShotsToKill { get; internal set; }
        public double TimeToKill { get; internal set; }
        public double MagazinesToKill { get; internal set; }
        public List<double> ShotsFired { get; internal set; }
        public double DpsToKill { get; internal set; }
        public double DamageDone { get; internal set; }
        public DamageModel()
        {
            ShotsFired = new List<double>();
        }
    }


    public class Shield
    {
        public double TotalAmount;
        public string Type;
        public double CurrentAmount;
        public Shield(string type, int baseLevel, int currentLevel, int baseValue)
        {
            Type = type;
            TotalAmount = baseValue + (1 + Math.Pow(currentLevel - baseLevel, 2) * 0.0075 * baseValue);
            CurrentAmount = TotalAmount;
        }
    }

    public class Armor
    {
        public double TotalAmount;
        public string Type;
        public double CurrentAmount;
        public Armor(string type, int baseLevel, int currentLevel, int baseValue)
        {
            Type = type;
            TotalAmount = baseValue * (1 + Math.Pow(currentLevel - baseLevel, 1.75) / 200);
            CurrentAmount = TotalAmount;
        }
    }

    public class Health
    {
        public double TotalAmount;
        public string Type;
        public double CurrentAmount;
        public Health(string type, int baseLevel, int currentLevel, int baseValue)
        {
            Type = type;
            TotalAmount = baseValue * (1 + Math.Pow(currentLevel - baseLevel, 2) * 0.015);
            CurrentAmount = TotalAmount;
        }

    }
}
