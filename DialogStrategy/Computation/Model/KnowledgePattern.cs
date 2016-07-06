using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Computation.Model
{
    class KnowledgePattern
    {
        private readonly KnowledgePath[] _paths;

        public IEnumerable<KnowledgePath> Paths { get { return _paths; } }

        public KnowledgePattern(IEnumerable<KnowledgePath> paths)
        {
            _paths = paths.ToArray();
        }
    }
}
