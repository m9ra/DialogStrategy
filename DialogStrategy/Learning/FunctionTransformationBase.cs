using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogStrategy.Learning
{
    public abstract class FunctionTransformationBase
    {
        internal abstract IEnumerable<ConditionalFunction> GetTransformations(ConditionalFunction function, AnotatedData failingData);
    }
}
