using Atomia.Store.Core;
using Atomia.Web.Plugin.ProductsProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using ApiProduct = Atomia.Web.Plugin.ProductsProvider.Product;
using CoreProduct = Atomia.Store.Core.Product;

namespace Atomia.Store.PublicBillingApi
{
    /// <summary>
    /// Helpers for mapping between products from Atomia Billing Product Service and AtomiaStore products
    /// </summary>
    public sealed class ProductMapper
    {
        private readonly bool pricesIncludeVat;
        private readonly bool inclusiveTaxCalculationType;
        private readonly Language language;
        private readonly string currencyCode;

        public ProductMapper(ILanguagePreferenceProvider languagePreferenceProvider, ICurrencyPreferenceProvider currencyPreferenceProvider, IVatDisplayPreferenceProvider vatDisplayPreferenceProvider, IResellerProvider resellerProvider)
        {
            if (languagePreferenceProvider == null)
            {
                throw new ArgumentNullException("languagePreferenceProvider");
            }

            if (currencyPreferenceProvider == null)
            {
                throw new ArgumentNullException("currencyPreferenceProvider");
            }

            if (vatDisplayPreferenceProvider == null)
            {
                throw new ArgumentNullException("vatDisplayPreferenceProvider");
            }

            this.language = languagePreferenceProvider.GetCurrentLanguage();
            this.currencyCode = currencyPreferenceProvider.GetCurrentCurrency().Code;
            this.pricesIncludeVat = vatDisplayPreferenceProvider.ShowPricesIncludingVat();
            this.inclusiveTaxCalculationType = resellerProvider.GetReseller().InclusiveTaxCalculationType;
        }

        /// <summary>
        /// Map product from Atomia Billing Product Service to AtomiaStore product
        /// </summary>
        public CoreProduct Map(ApiProduct apiProduct)
        {
            var product = new CoreProduct()
            {
                ArticleNumber = apiProduct.ArticleNumber
            };

            SetNameAndDescription(product, apiProduct);

            SetPricingVariants(product, apiProduct);

            SetCustomAttributes(product, apiProduct);

            SetCategories(product, apiProduct);

            return product;
        }

        /// <summary>
        /// Set localized name or description on product
        /// </summary>
        /// <param name="product">The product to set name and description on</param>
        /// <param name="apiProduct">The Atomia Billing product to get localized values from</param>
        private void SetNameAndDescription(CoreProduct product, ApiProduct apiProduct)
        {
            // Set defaults before checking if translations are available.
            product.Name = apiProduct.Name;
            product.Description = apiProduct.Description;

            if (apiProduct.MultilanguageNames != null)
            {
                var names = apiProduct.MultilanguageNames.Where(l => l.LanguageIso639Name.ToUpper() == this.language.PrimaryTag);
                var regionalName = names.FirstOrDefault(l => l.LanguageCulture.ToUpper() == this.language.RegionTag);
                var standardName = names.FirstOrDefault();
                
                if (regionalName != null)
                {
                    product.Name = regionalName.Value;
                }
                else if (standardName != null)
                {
                    product.Name = standardName.Value;
                }
            }

            if (apiProduct.MultilanguageDescriptions != null)
            {
                var descriptions = apiProduct.MultilanguageDescriptions.Where(l => l.LanguageIso639Name.ToUpper() == this.language.PrimaryTag);
                var regionalDescription = descriptions.FirstOrDefault(l => l.LanguageCulture.ToUpper() == this.language.RegionTag);
                var standardDescription = descriptions.FirstOrDefault();
                
                if (regionalDescription != null)
                {
                    product.Description = regionalDescription.Value;
                }
                else if (standardDescription != null)
                {
                    product.Description = standardDescription.Value;
                }
            }
        }

        /// <summary>
        /// Select and set price in relevant currency
        /// </summary>
        /// <param name="product">The product to set price on</param>
        /// <param name="apiProduct">The Atomia Billing product to select prices from</param>
        private void SetPricingVariants(CoreProduct product, ApiProduct apiProduct)
        {
            product.PricingVariants = new List<PricingVariant>();

            if (apiProduct.RenewalPeriods != null && apiProduct.RenewalPeriods.Count > 0)
            {
                foreach (var renewalPeriod in apiProduct.RenewalPeriods)
                {
                    var price = GetPrice(renewalPeriod.Prices, apiProduct.Taxes);

                    product.PricingVariants.Add(new PricingVariant
                    {
                        Price = price,
                        RenewalPeriod = new RenewalPeriod(renewalPeriod.RenewalPeriodValue, renewalPeriod.RenewalPeriodUnit)
                    });
                }
            }
            else if (apiProduct.Prices != null && apiProduct.Prices.Count > 0)
            {
                var price = GetPrice(apiProduct.Prices, apiProduct.Taxes);

                product.PricingVariants.Add(new PricingVariant
                {
                    Price = price,
                    RenewalPeriod = null
                });
            }

            if (apiProduct.CounterTypes != null && apiProduct.CounterTypes.Count > 0)
            {
                foreach (var apiCounterType in apiProduct.CounterTypes)
                {
                    PricingVariant counterPrice = new PricingVariant { FixedPrice = false };
                    CounterType counterType = new CounterType();
                    counterType.CounterId = apiCounterType.CounterId;
                    counterType.Name = apiCounterType.Name;
                    counterType.Description = apiCounterType.Description;

                    if (apiCounterType.MultilanguageNames != null)
                    {
                        var names = apiCounterType.MultilanguageNames.Where(l => l.LanguageIso639Name.ToUpper() == this.language.PrimaryTag);
                        var regionalName = names.FirstOrDefault(l => l.LanguageCulture.ToUpper() == this.language.RegionTag);
                        var standardName = names.FirstOrDefault();

                        if (regionalName != null)
                        {
                            counterType.Name = regionalName.Value;
                        }
                        else if (standardName != null)
                        {
                            counterType.Name = standardName.Value;
                        }
                    }

                    if (apiCounterType.MultilanguageDescriptions != null)
                    {
                        var descriptions = apiCounterType.MultilanguageDescriptions.Where(l => l.LanguageIso639Name.ToUpper() == this.language.PrimaryTag);
                        var regionalDescription = descriptions.FirstOrDefault(l => l.LanguageCulture.ToUpper() == this.language.RegionTag);
                        var standardDescription = descriptions.FirstOrDefault();

                        if (regionalDescription != null)
                        {
                            counterType.Description = regionalDescription.Value;
                        }
                        else if (standardDescription != null)
                        {
                            counterType.Description = standardDescription.Value;
                        }
                    }

                    counterType.UnitName = apiCounterType.UnitName;
                    counterType.UnitValue = apiCounterType.UnitValue;
                    counterType.RequireSubscription = apiCounterType.RequireSubscription;
                    counterType.Ranges = new List<CounterRange>();

                    foreach (var apiCounterRange in apiCounterType.Ranges)
                    {
                        CounterRange counterRange = new CounterRange();
                        counterRange.Name = apiCounterRange.Name;

                        if (apiCounterRange.MultilanguageNames != null)
                        {
                            var names = apiCounterRange.MultilanguageNames.Where(l => l.LanguageIso639Name.ToUpper() == this.language.PrimaryTag);
                            var regionalName = names.FirstOrDefault(l => l.LanguageCulture.ToUpper() == this.language.RegionTag);
                            var standardName = names.FirstOrDefault();

                            if (regionalName != null)
                            {
                                counterRange.Name = regionalName.Value;
                            }
                            else if (standardName != null)
                            {
                                counterRange.Name = standardName.Value;
                            }
                        }

                        counterRange.LowerMargin = apiCounterRange.LowerMargin;
                        counterRange.UpperMargin = apiCounterRange.UpperMargin;
                        counterRange.Price = GetPrice(apiCounterRange.Prices, apiProduct.Taxes);

                        counterType.Ranges.Add(counterRange);
                    }

                    counterPrice.CounterType = counterType;

                    product.PricingVariants.Add(counterPrice);
                }
            }
        }

        /// <summary>
        /// Copy custom attributes from Atomia Billing product to AtomiaStore product
        /// </summary>
        private void SetCustomAttributes(CoreProduct product, ApiProduct apiProduct)
        {
            product.CustomAttributes = new List<CustomAttribute>();

            foreach (var prop in apiProduct.Properties)
            {
                product.CustomAttributes.Add(new CustomAttribute
                {
                    Name = prop.Key,
                    Value = prop.Value
                });
            }
        }

        /// <summary>
        /// Set categories with name and localized description on product
        /// </summary>
        private void SetCategories(CoreProduct product, ApiProduct apiProduct)
        {
            product.Categories = new List<Category>();

            foreach (var shopCategory in apiProduct.ShopCategories)
            {
                var category = new Category { Name = shopCategory.Name, Description = shopCategory.Name };

                if (shopCategory.MultilanguageDescriptions != null)
                {
                    var descriptions = shopCategory.MultilanguageDescriptions.Where(l => l.LanguageIso639Name.ToUpper() == this.language.PrimaryTag);
                    var regionalDescription = descriptions.FirstOrDefault(l => l.LanguageCulture.ToUpper() == this.language.RegionTag);
                    var standardDescription = descriptions.FirstOrDefault();

                    if (regionalDescription != null)
                    {
                        category.Description = regionalDescription.Value;
                    }
                    else if (standardDescription != null)
                    {
                        category.Description = standardDescription.Value;
                    }
                }

                product.Categories.Add(category);
            }
        }

        /// <summary>
        /// Get correct price for currency and calculate with or without taxes applied.
        /// </summary>
        private decimal GetPrice(IList<ProductPrice> prices, IList<ProductTax> taxes)
        {
            var price = prices.FirstOrDefault(p => p.CurrencyCode == this.currencyCode);
            if (price == null)
            {
                throw new ArgumentException(String.Format("No prices available for currency code {0}", this.currencyCode));
            }

            var priceCalculator = new PriceCalculator(this.pricesIncludeVat, this.inclusiveTaxCalculationType);
            return priceCalculator.CalculatePrice(price.Value, taxes);
        }
    }
}
