using System;
using System.Collections.Generic;

namespace DataContext
{
    public partial class NameInRepresentation
    {
        public NameInRepresentation()
        {
            NameOntologyPredicates = new HashSet<NameOntologyPredicate>();
        }

        public long IdNameRepr { get; set; }
        public string NameRepr { get; set; } = null!;

        public virtual ICollection<NameOntologyPredicate> NameOntologyPredicates { get; set; }
    }
}
