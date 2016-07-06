using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogStrategy.Computation.Condition
{
    class PatternNode
    {
        internal readonly List<string> EdgePatterns = new List<string>();

        internal readonly List<PatternNode> TargetNodes = new List<PatternNode>();

        internal readonly string Name;

        internal PatternNode(string name)
        {
            this.Name = name;
        }

        internal void AddEdge(string edge, PatternNode targetNode)
        {
            if (EdgePatterns.Contains(edge) && TargetNodes.Contains(targetNode))
                //improve
                return;

            EdgePatterns.Add(edge);
            TargetNodes.Add(targetNode);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(Name);
            builder.Append("[");

            for (int i = 0; i < EdgePatterns.Count; ++i)
            {
                if (i > 0)
                    builder.Append(' ');

                var edge = EdgePatterns[i];
                var target = TargetNodes[i];

                builder.Append(edge);
                builder.Append(":");
                builder.Append(target.Name);
            }

            builder.Append("]");

            return builder.ToString();
        }
    }
}
