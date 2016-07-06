using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Learning
{
    class DataPattern
    {
        public readonly AnotatedData2 Data;

        public readonly IEnumerable<KnowledgePath> Paths;
    }
}
