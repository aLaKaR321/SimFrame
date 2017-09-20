using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimFrame
{
    class Simulation
    {
        private Enemy Enemy;
        private List<SimEvent> _eventList;
        private List<SimEvent> _eventHistory;
        public Simulation(Enemy enemy, WeaponStats weaponStats)
        {
            Enemy = enemy;
            _eventList = new List<SimEvent>();
            _eventHistory = new List<SimEvent>();
            _eventList.Add(new SimShot(0, enemy, _eventList, weaponStats));
        }
        
        public void Execute()
        {
            while(Enemy.Health.CurrentAmount > 0)
            {
                SimEvent currentEvent = _eventList.First();
                _eventList.AddRange(currentEvent.Process());
                _eventHistory.Add(currentEvent);
                _eventList.Remove(currentEvent);
                _eventList = _eventList.OrderBy(x=>x._activationTime).ToList();
                Console.WriteLine();
            }
        }
    }
    interface ISimEventInterface
    {
        List<SimEvent> Process();
    }
    abstract class SimEvent : IComparer<SimEvent> , ISimEventInterface
    {
        public double _activationTime;
        public Enemy _enemy;
        public List<SimEvent> _currentEvents;

        public SimEvent(double activationTime, Enemy enemy, List<SimEvent> currentEvents)
        {
            _activationTime = activationTime;
            _enemy = enemy;
            _currentEvents = currentEvents;
        }

        public int Compare(SimEvent x, SimEvent y)
        {
            return x._activationTime.CompareTo(y._activationTime);
        }
        public abstract List<SimEvent> Process();
    }
    class SimProc : SimEvent
    {
        public string _statusType;

        public SimProc(double activationTime, Enemy enemy, List<SimEvent> currentEvents, string statusType)
            : base(activationTime, enemy, currentEvents)
        {
            _statusType = statusType;
        }

        override public List<SimEvent> Process()
        {
            switch (_statusType)
            {
                case "Corrosive":
                    _enemy.Armor.CurrentAmount *= (1 / 0.75);
                    break;
                case "Viral":
                    _enemy.Health.CurrentAmount *= (1 / 0.5);
                    break;
                //case "Magnetic":
                //    _enemy.Armor.CurrentAmount *= (1 / 0.75);
                //    break;
                default:
                    break;
            }
            return new List<SimEvent>();
        }
    }
    class DamageProc : SimProc
    {
        public double _tickDamage;

        public DamageProc(double activationTime, Enemy enemy, List<SimEvent> currentEvents, string statusType, double tickDamage)
            : base(activationTime, enemy, currentEvents, statusType)
        {
            _tickDamage = tickDamage;
        }
    }
    class SimShot : SimEvent
    {
        private WeaponStats _weaponStats;

        public SimShot(double activationTime, Enemy enemy, List<SimEvent> currentEvents, WeaponStats weaponStats)
            : base(activationTime, enemy, currentEvents)
        {
            _weaponStats = weaponStats;
        }

        override public List<SimEvent> Process()
        {
            var simList = new List<SimEvent>();
            simList.AddRange(_enemy.simShot(_weaponStats, _activationTime, _currentEvents));
            _weaponStats.CurrentMagazine--;
            if (_weaponStats.CurrentMagazine > 0)
            {
                simList.Add(new SimShot(_activationTime + _weaponStats.getTimeBetweenShots(), _enemy, _currentEvents, _weaponStats));
            }
            else
            {
                simList.Add(new SimShot(_activationTime + _weaponStats.ReloadTime, _enemy, _currentEvents, _weaponStats));

                //simList.Add(new SimReload(_activationTime, _enemy, _currentEvents, _weaponStats));
            }
            return simList;
        }
    }
    //class SimReload : SimEvent
    //{
    //    private WeaponStats _weaponStats;

    //    public SimReload(double activationTime, Enemy enemy, List<SimEvent> currentEvents, WeaponStats weaponStats)
    //        : base(activationTime, enemy, currentEvents)
    //    {
    //        _enemy = enemy;
    //        _weaponStats = weaponStats;
    //    }

    //    override public List<SimEvent> Process()
    //    {
    //        var simList = new List<SimEvent>();
    //        _weaponStats.CurrentMagazine = _weaponStats.MagazineSize;
    //        simList.Add(new SimShot(_activationTime, _enemy, _currentEvents, _weaponStats));
    //        return simList;
    //    }
    //}
}
