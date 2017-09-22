using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Forms;

namespace SimFrame
{
    class Simulation
    {
        public Enemy Enemy;
        public WeaponStats _weaponStats;
        public List<SimEvent> _eventList;
        public List<SimEvent> _eventHistory;
        public List<SimHistoryEvent> _damageHistory;

        public Simulation(Enemy enemy, WeaponStats weaponStats)
        {
            Enemy = enemy;
            _weaponStats = weaponStats;
            _eventList = new List<SimEvent>();
            _eventHistory = new List<SimEvent>();
            _damageHistory = new List<SimHistoryEvent>();
            _eventList.Add(new SimShot(0, this));
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
            }
        }
    }

    class SimulationSeries : ISeries
    {
        private List<Simulation> list;
        public SimulationSeries(List<Simulation> list)
        {

        }

        public ObservableCollection<object> LegendItems => throw new NotImplementedException();

        public ISeriesHost SeriesHost { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    internal class SimHistoryEvent
    {
        public double activationTime;
        public string damageType;
        public string damageTo;
        public double damageDone;
        public double damageReduction;

        public SimHistoryEvent(double activationTime, string damageType, string damageTo, double damageDone, double damageReduction)
        {
            this.activationTime = activationTime;
            this.damageType = damageType;
            this.damageTo = damageTo;
            this.damageDone = damageDone;
            this.damageReduction = damageReduction;
        }
    }

    interface ISimEventInterface
    {
        List<SimEvent> Process();
    }
    abstract class SimEvent : IComparer<SimEvent> , ISimEventInterface
    {
        public double _activationTime;
        public Simulation _simulation;

        public SimEvent(double activationTime, Simulation simulation)
        {
            _activationTime = activationTime;
            _simulation = simulation;
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

        public SimProc(double activationTime, Simulation simulation, string statusType)
            : base(activationTime, simulation)
        {
            _statusType = statusType;
        }

        override public List<SimEvent> Process()
        {
            switch (_statusType)
            {
                case "Corrosive":
                    _simulation.Enemy.Armor.CurrentAmount *= (1 / 0.75);
                    break;
                case "Viral":
                    _simulation.Enemy.Health.CurrentAmount *= (1 / 0.5);
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

        public DamageProc(double activationTime, Simulation simulation, string statusType, double tickDamage)
            : base(activationTime, simulation, statusType)
        {
            _tickDamage = tickDamage;
        }

        public override List<SimEvent> Process()
        {
            List<SimHistoryEvent> list = _simulation.Enemy.simProc(_tickDamage, _statusType, _activationTime);
            _simulation._damageHistory.AddRange(list);
            return new List<SimEvent>();
        }
        
    }
    class SimShot : SimEvent
    {
        public bool _isMultishot;
        public SimShot(double activationTime, Simulation simulation, bool isMultishot = false)
            : base(activationTime, simulation)
        {
            _isMultishot = isMultishot;
        }

        override public List<SimEvent> Process()
        {
            var simList = new List<SimEvent>();
            simList.AddRange(_simulation.Enemy.simShot(_activationTime, _simulation, _isMultishot));
            _simulation._weaponStats.CurrentMagazine--;
            if (_simulation._weaponStats.CurrentMagazine > 0)
            {
                simList.Add(new SimShot(_activationTime + _simulation._weaponStats.getTimeBetweenShots(), _simulation));
            }
            else
            {
                simList.Add(new SimShot(_activationTime + _simulation._weaponStats.ReloadTime, _simulation));
            }
            return simList;
        }
    }
    class ReportForm : Form
    {
        public ReportForm(List<Simulation> simList)
        {
            Chart chart = new Chart();

            chart.Series.Add();
            var grid = new FlowLayoutPanel();
            grid.Controls.Add()
            this.Controls.Add()
        }
    }
}
