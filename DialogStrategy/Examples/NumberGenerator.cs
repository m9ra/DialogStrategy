using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Examples
{
    class NumberGenerator : NodeGenerator
    {
        public readonly int MaxNumber;
        public NumberGenerator(int maxNumber)
        {
            MaxNumber = maxNumber;
        }

        internal override bool HasGenerated(object data)
        {
            return data is int;
        }

        internal override IEnumerable<object> Generate()
        {
            for (var i = 0; i < MaxNumber; ++i)
                yield return i;
        }

        internal override IEnumerable<string> Edges(object fromNode, object toNode)
        {
            if (toNode as string == AlgebraLayer.NumberParent)
                yield return Graph.IsRelation;
        }

        internal override bool HasEdges(object toNode)
        {
            return toNode as string == AlgebraLayer.NumberParent;
        }

        internal override IEnumerable<object> Targets(object fromNode, string edge)
        {
            if (edge == Graph.IsRelation)
                yield return AlgebraLayer.NumberParent;
        }

        internal override IEnumerable<object> Sources(object toNode, string edge)
        {
            if (edge == Graph.IsRelation && toNode as string == AlgebraLayer.NumberParent)
                //every number has is relation to number parent
                return Generate();

            return Enumerable.Empty<object>();
        }
    }
}
