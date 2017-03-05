using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreCatalogue
{
    public class StoreCatalogueException : Exception
    {
        public StoreCatalogueException() : base()
        { }

        public StoreCatalogueException(string message) : base(message)
        { }
    }
}