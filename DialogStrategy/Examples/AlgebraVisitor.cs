using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DialogStrategy.Knowledge;

namespace DialogStrategy.Examples
{
    interface IAlgebraVisitable
    {
        void Accept(AlgebraVisitor visitor);
    }

    class AlgebraVisitor
    {
        public void VisitNode(NodeReference node)
        {
            var o = node.Data;

            //visit string nodes
            if (o is string)
            {
                switch (o as string)
                {
                    case AlgebraLayer.NumberParent:
                        VisitNumberParent();
                        break;
                    case AlgebraLayer.OperatorParent:
                        VisitOperatorParent();
                        break;
                }
                return;
            }

            //visit number nodes
            if (o is int)
            {
                Visit((int)o);
                return;
            }

            //visit algebra visitable nodes
            var visitable = node as IAlgebraVisitable;
            if (visitable == null)
                //node cannot be visited
                return;

            visitable.Accept(this);
        }

        public virtual void Visit(int x) { }

        public virtual void Visit(OperatorInstance x) { }

        public virtual void Visit(Operator x) { }

        public virtual void VisitNumberParent() { }

        public virtual void VisitOperatorParent() { }
    }
}
