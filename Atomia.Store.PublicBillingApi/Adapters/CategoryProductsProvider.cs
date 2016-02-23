using Atomia.Store.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using CoreProduct = Atomia.Store.Core.Product;

namespace Atomia.Store.PublicBillingApi.Adapters
{
    /// <summary>
    /// <see cref="Atomia.Store.Core.IProductListProvider"/> to get products ín specific category from current reseller's shop in Atomia Billing.
    /// </summary>
    public sealed class CategoryProductsProvider : IProductListProvider
    {
        private readonly ApiProductsProvider apiProductsProvider;
        private readonly ProductMapper productMapper;

        /// <summary>
        /// Construct a new instance
        /// </summary>
        public CategoryProductsProvider(ApiProductsProvider apiProductsProvider, ProductMapper productMapper)
        {
            if (apiProductsProvider == null)
            {
                throw new ArgumentNullException("apiProductsProvider");
            }

            if (productMapper == null)
            {
                throw new ArgumentNullException("productMapper");
            }

            this.apiProductsProvider = apiProductsProvider;
            this.productMapper = productMapper;
        }

        /// <summary>
        /// The unique name of the <see cref="Atomia.Store.Core.IProductListProvider"/>
        /// </summary>
        public string Name
        {
            get { return "Category"; }
        }

        /// <summary>
        /// Get products by category. The value of the first <see cref="Atomia.Store.Core.SearchTerm"/> will be used as the category name to list products from.
        /// </summary>
        /// <param name="terms">Search terms, the first of which should have the value of category to find products from</param>
        /// <returns>Any found products for the category, ordered by price.</returns>
        public IEnumerable<CoreProduct> GetProducts(ICollection<SearchTerm> terms)
        {
            var category = terms.First().Value;
            var products = new List<CoreProduct>();

            var apiProducts = apiProductsProvider.GetProductsByCategories(new List<string>() { category });

            foreach(var apiProduct in apiProducts)
            {
                var product = this.productMapper.Map(apiProduct);
                
                products.Add(product);
            }

            var sortedProducts = products.Where(p => p.PricingVariants.Any(v => v.FixedPrice)).OrderBy(p => p.PricingVariants.Min(v => v.Price)).ToList();
            sortedProducts.AddRange(products.Where(p => !p.PricingVariants.Any(v => v.FixedPrice)).OrderBy(p => p.PricingVariants.Min(v => v.CounterType.Ranges.Min(r => r.Price))).ToList());
            return sortedProducts;
        }
    }
}
