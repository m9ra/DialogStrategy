using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Learning
{
    class ClassifierBuilder
    {
        private readonly DataPattern _targetPattern;

        private readonly PatternNodeOperator[] _nodeOperators;

        private PatternGraph _currentGraph;

        public ClassifierBuilder(DataPattern targetPattern)
        {
            _targetPattern = targetPattern;
            _currentGraph = new PatternGraph(targetPattern);
        }

        public int PatternDistance(DataPattern pattern)
        {
            var inputGraph = new PatternGraph(pattern);
            var difference = _currentGraph.Except(inputGraph);

            var distance = 0;

            foreach (var node in difference.Nodes)
            {
                var minDistance = int.MaxValue;
                PatternNodeOperator minOp = null;
                foreach (var op in _nodeOperators)
                {
                    var currentDistance = op.NodeDistance(_targetPattern, inputGraph, node);
                    if (currentDistance < minDistance)
                    {
                        minDistance = currentDistance;
                        minOp = op;
                    }
                }

                if (minOp == null)
                    return int.MaxValue;

                distance += minDistance;    
            }
            //TODO edge operators

            return distance;
        }

        public void AcceptPattern(DataPattern path)
        {
            throw new NotImplementedException();
        }
    }
}
