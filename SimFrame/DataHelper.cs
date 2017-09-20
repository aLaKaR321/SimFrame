using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimFrame
{
    class DataHelper
    {
        public static Random Random = new Random(); // TODO: implement better (thread safe, non-locking) random
        public static List<string> physicalList = new List<string>() {
                    "Impact",
                    "Puncture",
                    "Slash"
                };
        public static Dictionary<string, Dictionary<string, double>> DamageTypeDictionary = new Dictionary<string, Dictionary<string, double>>
            {
                {
                    "Impact",
                    new Dictionary<string, double>
                    {
                        { "Flesh", -0.25 },
                        { "Cloned Flesh", -0.25 },
                        { "Fossil", 0 },
                        { "Infested", 0 },
                        { "Infested Flesh", 0 },
                        { "Infested Sinew", 0 },
                        { "Machinery", 0.25 },
                        { "Robotic", 0 },
                        { "Object", 0 },
                        { "Shield", 0.5 },
                        { "Proto Shield", 0.25 },
                        { "Ferrite Armor", 0 },
                        { "Alloy Armor", 0 }
                    }
                },
                {
                    "Puncture",
                    new Dictionary<string, double>
                    {
                        { "Flesh", 0 },
                        { "Cloned Flesh", 0 },
                        { "Fossil", 0 },
                        { "Infested", 0 },
                        { "Infested Flesh", 0 },
                        { "Infested Sinew", 0.25 },
                        { "Machinery", 0 },
                        { "Robotic", 0.25 },
                        { "Object", 0 },
                        { "Shield", -0.2 },
                        { "Proto Shield", -0.5 },
                        { "Ferrite Armor", 0.5 },
                        { "Alloy Armor", 0.15 }
                    }
                },
                {
                    "Slash",
                    new Dictionary<string, double>
                    {
                        { "Flesh", 0.25 },
                        { "Cloned Flesh", 0.25 },
                        { "Fossil", 0.15 },
                        { "Infested", 0.25 },
                        { "Infested Flesh", 0.5 },
                        { "Infested Sinew", 0 },
                        { "Machinery", 0 },
                        { "Robotic", -0.25 },
                        { "Object", 0 },
                        { "Shield", 0 },
                        { "Proto Shield", 0 },
                        { "Ferrite Armor", -0.15 },
                        { "Alloy Armor", -0.5 }
                    }
                },
                {
                    "Finisher",
                    new Dictionary<string, double>
                    {
                        { "Flesh", 0 },
                        { "Cloned Flesh", 0 },
                        { "Fossil", 0 },
                        { "Infested", 0 },
                        { "Infested Flesh", 0 },
                        { "Infested Sinew", 0.33 },
                        { "Machinery", 0 },
                        { "Robotic", 0 },
                        { "Object", 0 },
                        { "Shield", 0 },
                        { "Proto Shield", 0 },
                        { "Ferrite Armor", 0 },
                        { "Alloy Armor", 0 }
                    }
                },
                {
                    "Cold",
                    new Dictionary<string, double>
                    {
                        { "Flesh", 0 },
                        { "Cloned Flesh", 0 },
                        { "Fossil", -0.25 },
                        { "Infested", 0 },
                        { "Infested Flesh", -0.5 },
                        { "Infested Sinew", 0.25 },
                        { "Machinery", 0 },
                        { "Robotic", 0 },
                        { "Object", 0 },
                        { "Shield", 0.5 },
                        { "Proto Shield", 0 },
                        { "Ferrite Armor", 0 },
                        { "Alloy Armor", 0.25 }
                    }
                },
                {
                    "Electricity",
                    new Dictionary<string, double>
                    {
                        { "Flesh", 0 },
                        { "Cloned Flesh", 0 },
                        { "Fossil", 0 },
                        { "Infested", 0 },
                        { "Infested Flesh", 0 },
                        { "Infested Sinew", 0 },
                        { "Machinery", 0.5 },
                        { "Robotic", 0.5 },
                        { "Object", 0 },
                        { "Shield", 0 },
                        { "Proto Shield", 0 },
                        { "Ferrite Armor", 0 },
                        { "Alloy Armor", -0.5 }
                    }
                },
                {
                    "Heat",
                    new Dictionary<string, double>
                    {
                        { "Flesh", 0 },
                        { "Cloned Flesh", 0.25 },
                        { "Fossil", 0 },
                        { "Infested", 0.25 },
                        { "Infested Flesh", 0.5 },
                        { "Infested Sinew", 0 },
                        { "Machinery", 0 },
                        { "Robotic", 0 },
                        { "Object", 0 },
                        { "Shield", 0 },
                        { "Proto Shield", -0.5 },
                        { "Ferrite Armor", 0 },
                        { "Alloy Armor", 0 }
                    }
                },
                {
                    "Toxin",
                    new Dictionary<string, double>
                    {
                        { "Flesh", 0.5 },
                        { "Cloned Flesh", 0 },
                        { "Fossil", -0.5 },
                        { "Infested", 0 },
                        { "Infested Flesh", 0 },
                        { "Infested Sinew", -0.25 },
                        { "Machinery", -0.25 },
                        { "Robotic", -0.25 },
                        { "Object", 0 },
                        { "Shield", 0 },
                        { "Proto Shield", 0 },
                        { "Ferrite Armor", 0.25 },
                        { "Alloy Armor", 0 }
                    }
                },
                {
                    "Blast",
                    new Dictionary<string, double>
                    {
                        { "Flesh", 0 },
                        { "Cloned Flesh", 0 },
                        { "Fossil", 0.5 },
                        { "Infested", 0 },
                        { "Infested Flesh", 0 },
                        { "Infested Sinew", -0.5 },
                        { "Machinery", 0.75 },
                        { "Robotic", 0 },
                        { "Object", 0 },
                        { "Shield", 0 },
                        { "Proto Shield", 0 },
                        { "Ferrite Armor", -0.25 },
                        { "Alloy Armor", 0 }
                    }
                },
                {
                    "Corrosive",
                    new Dictionary<string, double>
                    {
                        { "Flesh", 0 },
                        { "Cloned Flesh", 0 },
                        { "Fossil", 0.75 },
                        { "Infested", 0 },
                        { "Infested Flesh", 0 },
                        { "Infested Sinew", 0 },
                        { "Machinery", 0 },
                        { "Robotic", 0 },
                        { "Object", 0 },
                        { "Shield", 0 },
                        { "Proto Shield", -0.5 },
                        { "Ferrite Armor", 0.75 },
                        { "Alloy Armor", 0 }
                    }
                },
                {
                    "Gas",
                    new Dictionary<string, double>
                    {
                        { "Flesh", -0.25 },
                        { "Cloned Flesh", -0.5 },
                        { "Fossil", 0 },
                        { "Infested", 0.75 },
                        { "Infested Flesh", 0.5 },
                        { "Infested Sinew", 0 },
                        { "Machinery", 0 },
                        { "Robotic", 0 },
                        { "Object", 0 },
                        { "Shield", 0 },
                        { "Proto Shield", 0 },
                        { "Ferrite Armor", 0 },
                        { "Alloy Armor", 0 }
                    }
                },
                {
                    "Magnetic",
                    new Dictionary<string, double>
                    {
                        { "Flesh", 0 },
                        { "Cloned Flesh", 0 },
                        { "Fossil", 0 },
                        { "Infested", 0 },
                        { "Infested Flesh", 0 },
                        { "Infested Sinew", 0 },
                        { "Machinery", 0 },
                        { "Robotic", 0 },
                        { "Object", 0 },
                        { "Shield", 0.75 },
                        { "Proto Shield", 0.75 },
                        { "Ferrite Armor", 0 },
                        { "Alloy Armor", -0.5 }
                    }
                },
                {
                    "Radiation",
                    new Dictionary<string, double>
                    {
                        { "Flesh", 0 },
                        { "Cloned Flesh", 0 },
                        { "Fossil", -0.75 },
                        { "Infested", -0.5 },
                        { "Infested Flesh", 0 },
                        { "Infested Sinew", 0.5 },
                        { "Machinery", 0 },
                        { "Robotic", 0.25 },
                        { "Object", 0 },
                        { "Shield", -0.25 },
                        { "Proto Shield", 0 },
                        { "Ferrite Armor", 0 },
                        { "Alloy Armor", 0.75 }
                    }
                },
                {
                    "Viral",
                    new Dictionary<string, double>
                    {
                        { "Flesh", 0.5 },
                        { "Cloned Flesh", 0.75 },
                        { "Fossil", 0 },
                        { "Infested", -0.5 },
                        { "Infested Flesh", 0 },
                        { "Infested Sinew", 0 },
                        { "Machinery", -0.25 },
                        { "Robotic", 0 },
                        { "Object", 0 },
                        { "Shield", 0 },
                        { "Proto Shield", 0 },
                        { "Ferrite Armor", 0 },
                        { "Alloy Armor", 0 }
                    }
                }
            };
    }
}
