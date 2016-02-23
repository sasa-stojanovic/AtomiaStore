using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomia.Store.Core
{
    /// <summary>
    /// A counter type in the store.
    /// </summary>
    public sealed class CounterType
    {
        /// <summary>
        /// Counter id.
        /// </summary>
        public string CounterId { get; set; }

        /// <summary>
        /// Human readable name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Human readable description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Unit value.
        /// </summary>
        public decimal UnitValue { get; set; }

        /// <summary>
        /// Name of the unit.
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or sets the require subscription property
        /// used to define if this counter should be processed
        /// without a subscription or not.
        /// </summary>
        public bool RequireSubscription { get; set; }

        /// <summary>
        /// Gets or sets the ranges.
        /// </summary>
        /// <value>The ranges.</value>
        public IList<CounterRange> Ranges { get; set; }
    }
}
