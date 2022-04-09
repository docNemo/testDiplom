using System;
using System.Collections.Generic;

namespace DataContext
{
    public partial class NameOntologyPredicate
    {
        public long IdConnection { get; set; }
        public long IdNameRepr { get; set; }
        public string NamePredicate { get; set; } = null!;

        public virtual NameInRepresentation IdNameReprNavigation { get; set; } = null!;
    }
}
