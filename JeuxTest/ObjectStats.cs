using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiEngine
{
    public class ObjectStats : INodes
    {
        public double currentHealth;
        public double currentEnergy;

        public uint Health { set; get; }
        public double CurrentHealth
        {
            set
            {
                if(value <= 0)
                {
                    currentHealth = value;
                    (Parent as GameObject).OnDeathEvent();
                }
                if (value > Health)
                    currentHealth = Health;
                currentHealth = value;
            }
            get => (uint)currentHealth;
        }

        public uint Energy { set; get; }
        public double CurrentEnergy { set; get; }
        public double HealthRegenerationRate { set; get; }
        public double EnergyRegenerationRate { set; get; }
        public uint Mass { set; get; }
        public uint ThrustPower { set; get; }
        public uint Damage { set; get; }
        public List<ObjectStatsSignal> DamageQueries { set; get; }
        public List<INodes> Children { set; get; }
        public INodes Parent { set; get; }
     

        public ObjectStats(GameObject gameObject )
        {
            Health = 100;
            CurrentHealth = Health;
            Energy = 100;
            CurrentEnergy = Energy;
            HealthRegenerationRate = 0;
            EnergyRegenerationRate = 0;
            Mass = 10;
            ThrustPower = 0;
            Damage = 0;
            DamageQueries = new List<ObjectStatsSignal>();
            Children = new List<INodes>();
            Parent = gameObject as INodes;
        }

        public void Update()
        {
            double delta = Engine.Instance.CoreUpdate.DeltaTime / 1000.0;//Engine.Instance.CoreUpdate.DeltaTime / 1000.0;
            CurrentHealth += HealthRegenerationRate != 0 ? Health / HealthRegenerationRate * delta : 0;
            ProcessDamageQueries();
        }

        public void OnDamageQuery(ObjectStatsSignal query)
        {

        }

        public void ProcessDamageQueries()
        {
            foreach(ObjectStatsSignal query in DamageQueries)
            { 
                CurrentHealth -= query.Damage;
            }
            DamageQueries = new List<ObjectStatsSignal>();
        }


        public void AddChild(INodes child)
        {
            Children.Add(child);
        }
        public void RemoveChild(INodes child)
        {
            Children.Remove(child);
        }

    }
}
