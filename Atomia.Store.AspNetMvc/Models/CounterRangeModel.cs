using Atomia.Store.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Atomia.Store.AspNetMvc.Models
{
    public class CounterRangeModel
    {
        private readonly ICurrencyFormatter currencyFormatter = DependencyResolver.Current.GetService<ICurrencyFormatter>();
        private readonly IResourceProvider resourceProvider = DependencyResolver.Current.GetService<IResourceProvider>();
        private readonly CounterRange counterRange;
        private readonly string unitName;

        /// <summary>
        /// Construct an instance from a <see cref="Atomia.Store.Core.CounterRange"/>
        /// </summary>
        public CounterRangeModel(CounterRange counterRange, string unitName)
        {
            this.counterRange = counterRange;
            this.unitName = unitName;
        }

        /// <summary>
        /// Currency formatted price.
        /// </summary>
        public string FormatedPrice
        {
            get
            {
                return currencyFormatter.FormatAmount(counterRange.Price);
            }
        }

        /// <summary>
        /// Range lower maring
        /// </summary>
        public decimal LowerMargin
        {
            get
            {
                return counterRange.LowerMargin;
            }
        }

        /// <summary>
        /// Range upper maring
        /// </summary>
        public decimal UpperMargin
        {
            get
            {
                return counterRange.UpperMargin;
            }
        }

        /// <summary>
        /// Range price
        /// </summary>
        public decimal Price
        {
            get
            {
                return counterRange.Price;
            }
        }

        /// <summary>
        /// Property wrapper of ToString representation that will be automatically serialized by default JSON serializer.
        /// </summary>
        public string Display
        {
            get
            {
                return string.Format(resourceProvider.GetResource("PricingVariantUBPDisplay"), counterRange.Name, counterRange.LowerMargin, counterRange.UpperMargin, unitName, FormatedPrice);
            }
        }
    }
}
