using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Computation
{
    class ActionContext
    {
        private readonly Dictionary<NodeReference, double> _informedNodes = new Dictionary<NodeReference, double>();

        private readonly Dictionary<NodeReference, int> _votes = new Dictionary<NodeReference, int>();

        internal NodeReference GetTopNode() {
            if (_informedNodes.Count == 0)
                return null;

            return getTop().Key;
        }

        internal double GetTopConfidence()
        {
            if (_informedNodes.Count == 0)
                return double.NegativeInfinity;

            return getTop().Value;
        }

        internal KeyValuePair<NodeReference, double>[] hypotheses()
        {
            var result = new List<KeyValuePair<NodeReference, double>>();

            foreach (var pair in _informedNodes)
            {
                var votes = _votes[pair.Key];
                var score = _informedNodes[pair.Key];

                result.Add(new KeyValuePair<NodeReference, double>(pair.Key, score / votes));
            }

            return result.ToArray();
        }
        
        internal void Hypothesis(NodeReference node, double confidence)
        {
            if (_informedNodes.ContainsKey(node))
            {
                _votes[node] += 1;
                _informedNodes[node] += confidence;
            }
            else
            {
                _votes[node] = 1;
                _informedNodes[node] = confidence;
            }
        }

        private KeyValuePair<NodeReference, double> getTop()
        {
            return _informedNodes.Aggregate((n1, n2) => n1.Value / _votes[n1.Key] > n2.Value / _votes[n2.Key] ? n1 : n2);
        }
    }
}
