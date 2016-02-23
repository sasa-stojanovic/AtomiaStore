
namespace Atomia.Store.AspNetMvc.Models
{
    /// <summary>
    /// View model for the /ProductListing/Index page.
    /// </summary>
    public class ProductListingViewModel
    {
        /// <summary>
        /// Query to use for listing products
        /// </summary>
        public virtual string Query { get; set; }

        /// <summary>
        /// Name of <see cref="Atomia.Store.Core.IProductListProvider" to use. />
        /// </summary>
        public virtual string ListingType { get; set; }

        /// <summary>
        /// Use sliders
        /// </summary>
        public virtual bool UseSliders { get; set; }

        /// <summary>
        /// Slider configuration
        /// </summary>
        public virtual string SliderConfig { get; set; }

        /// <summary>
        /// Decimal places for VPS prices
        /// </summary>
        public virtual int VpsPriceDecimalPlaces { get; set; }
    }
}
