using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSSWebApp.Models.Heuristics
{
    public class Pair<X,Y>
    {
        X elem1;
        Y elem2;

        public Pair(X elem1, Y elem2)
        {
            this.elem1 = elem1;
            this.elem2 = elem2;
        }

        public X getFirstElem()
        {
            return this.elem1;
        }

        public Y getSecondElem()
        {
            return this.elem2;
        }

        public override bool Equals(object obj)
        {
            Pair<X, Y> otherPair = obj as Pair<X, Y>;
            return this.getFirstElem().Equals(otherPair.elem1) &&
                    this.getSecondElem().Equals(otherPair.elem2);
        }
    }
}