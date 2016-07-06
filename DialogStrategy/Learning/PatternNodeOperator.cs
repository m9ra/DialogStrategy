using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Learning
{
    abstract class PatternNodeOperator
    {
        abstract internal int NodeDistance(DataPattern _targetPattern, PatternGraph inputGraph, NodeReference node);
    }
}
