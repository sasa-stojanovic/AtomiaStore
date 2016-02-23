using Atomia.Store.AspNetMvc.Infrastructure;
using Atomia.Store.AspNetMvc.Models;
using Atomia.Store.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Atomia.Store.AspNetMvc.Filters;

namespace Atomia.Store.AspNetMvc.Controllers
{
    /// <summary>
    /// Product listing page and product JSON API.
    /// </summary>
    public sealed class ProductListingController : Controller
    {
        private readonly IEnumerable<IProductListProvider> productListProviders = DependencyResolver.Current.GetServices<IProductListProvider>();
        private readonly IProductProvider productProvider = DependencyResolver.Current.GetService<IProductProvider>();

        /// <summary>
        /// Product listing page.
        /// </summary>
        /// <param name="query">Query to list products from.</param>
        /// <param name="listingType">Name of the <see cref="Atomia.Store.Core.IProductListingProvider"/> to use.</param>
        /// <param name="viewName">Name of the view to use.</param>
        /// <remarks>The view is expected to call <see cref="GetData"/> to populate the product list with help of Query and/or ListingType properties.</remarks>
        [OrderFlowFilter]
        [HttpGet]
        public ActionResult Index(string query, string listingType = "Category", string viewName = "Index")
        {
            var model = DependencyResolver.Current.GetService<ProductListingViewModel>();
            model.Query = query;
            model.ListingType = listingType;

            model.UseSliders = false;
            if (System.Web.HttpContext.Current.Application["UseSliders"] != null)
            {
                bool useSliders;
                if (Boolean.TryParse(System.Web.HttpContext.Current.Application["UseSliders"].ToString().ToLower(), out useSliders))
                {
                    model.UseSliders = useSliders;
                }
            }
            if (System.Web.HttpContext.Current.Application["SliderConfig"] != null)
            {
                model.SliderConfig = System.Web.HttpContext.Current.Application["SliderConfig"].ToString();
            }
            model.VpsPriceDecimalPlaces = 4;
            if (System.Web.HttpContext.Current.Application["VpsPriceDecimalPlaces"] != null)
            {
                int vpsPriceDecimalPrices = 4;
                if (Int32.TryParse(System.Web.HttpContext.Current.Application["VpsPriceDecimalPlaces"].ToString(), out vpsPriceDecimalPrices))
                {
                    model.VpsPriceDecimalPlaces = vpsPriceDecimalPrices;
                }
            }

            return View(viewName, model);
        }

        /// <summary>
        /// Get product listing data as JSON.
        /// </summary>
        /// <param name="query">Query to list products from.</param>
        /// <param name="listingType">Name of the <see cref="Atomia.Store.Core.IProductListingProvider"/> to use.</param>
        [HttpGet]
        public JsonResult GetData(string query, string listingType = "Category")
        {
            var model = DependencyResolver.Current.GetService<ProductListingModel>();
            var provider = productListProviders.FirstOrDefault(x => x.Name == listingType);

            if (provider == null)
            {
                throw new InvalidOperationException(String.Format("No IProductsProvider with Name \"{0}\" registered.", listingType));
            }

            var searchTerms = new List<SearchTerm> { new SearchTerm(listingType, query) };

            model.Query = query;
            model.Products = provider.GetProducts(searchTerms).Select(r => new ProductModel(r)).ToList();

            return JsonEnvelope.Success(new
            {
                CategoryData = model
            });
        }

        /// <summary>
        /// Get single product JSON data from article number.
        /// </summary>
        public JsonResult GetItem(string articleNumber)
        {
            if (ModelState.IsValid)
            {
                Product product = null;

                try
                {
                    product = productProvider.GetProduct(articleNumber);
                }
                catch(ArgumentException)
                { 
                }

                if (product != null)
                {
                    return JsonEnvelope.Success(new
                    {
                        Item = new ProductModel(product)
                    });
                }
                else
                {
                    return JsonEnvelope.Fail(new
                    {
                        Message = String.Format("No product with article number {0}", articleNumber)
                    });
                }
            }

            return JsonEnvelope.Fail(ModelState);
        }
    }
}
