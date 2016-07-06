using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Learning
{
    class PatternGraph
    {
        private DataPattern targetPattern;

        public IEnumerable<NodeReference> Nodes { get; set; }

        public PatternGraph(DataPattern targetPattern)
        {
            // TODO: Complete member initialization
            this.targetPattern = targetPattern;
        }

        internal PatternGraph Except(PatternGraph inputGraph)
        {
            throw new NotImplementedException();
        }
    }
}
