using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellParser
{
    public class Spell
    {
        public string[] Domains { get; set; }
        public string SpellName { get; set; }
        public int Power { get; set; }
        public int Range { get; set; }
        public int Duration { get; set; }
        public string Type { get; set; }
        public int Complexity { get; set; }
        public string SpellMechanics { get; set; }
        public string[] Techniques { get; set; }
        public string[] Adjuncts { get; set; }
        public string[] Fallacies { get; set; }
        public string Element { get; set; }
        public string Key { get; set; }
    }
}
