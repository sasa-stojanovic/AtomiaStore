using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomia.Store.Core
{
    /// <summary>
    /// A counter range in the store.
    /// </summary>
    public sealed class CounterRange
    {
        /// <summary>
        /// Human readable name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Lower margin.
        /// </summary>
        public decimal LowerMargin { get; set; }

        /// <summary>
        /// Upper margin.
        /// </summary>
        public decimal UpperMargin { get; set; }

        /// <summary>
        /// The price
        /// </summary>
        public decimal Price { get; set; }
    }
}
