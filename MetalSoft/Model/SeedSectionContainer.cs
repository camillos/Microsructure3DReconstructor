using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MetalSoft.Model
{
    public  class SeedSectionContainer
    {
        public int ID { get; set; }

        private List<SeedSection> seeds;

        public int SeedsSectionCount { get { return seeds.Count; } }
        public SeedSection GetSeedSection(int i)
        {
            return seeds[i];
        }

        public SeedSectionContainer()
        {
            seeds = new List<SeedSection>();
            ID = Helper.SeedSectionContainerID++;
        }


        public void Add(SeedSection seed)
        {
            seed.Container = this;
            seeds.Add(seed);
        }

        public PointF a, b;
        public double discreteLenght;
        public int maxTop, maxBottom;

        private SeedSection pattern;
        private SeedSection matched;

        public SeedSection Pattern { get { return pattern; } set { pattern = value; } }
        public SeedSection Matched { get { return matched; } set { matched = value; } }

    }




}
