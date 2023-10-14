using System;
using System.Collections.Generic;
using System.Linq;

namespace STamMultiplayerTestTak.Package.UI
{
    public class TableFilter<T>
    {
        private readonly Func<T, bool> _predicate;

        public TableFilter(Func<T,bool> predicate)
        {
            _predicate = predicate;
        }
        public IEnumerable<T> Apply(IEnumerable<T> source)
        {
            return source.Where(_predicate);
        }
    }
}