
namespace Atomia.Store.Core
{
    /// <summary>
    /// Each <see cref="Product"/> can have multiple pricing variants
    /// </summary>
    public sealed class PricingVariant
    {
        /// <summary>
        /// Is price fixed price
        /// </summary>
        public bool FixedPrice { get; set; }

        /// <summary>
        /// The <see cref="RenewalPeriod"/>, if any for the product.
        /// </summary>
        /// <remarks>Set to null means no renewal period.</remarks>
        public RenewalPeriod RenewalPeriod { get; set; }

        /// <summary>
        /// The price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The counter type
        /// </summary>
        public CounterType CounterType { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public PricingVariant()
        {
            FixedPrice = true;
        }
    }
}
