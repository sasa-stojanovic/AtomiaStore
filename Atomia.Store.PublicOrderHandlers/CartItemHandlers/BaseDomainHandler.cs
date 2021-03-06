﻿using Atomia.Store.PublicBillingApi.Handlers;
using Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atomia.Store.PublicOrderHandlers.CartItemHandlers
{
    /// <summary>
    /// Base for domain related <see cref="Atomia.Store.PublicBillingApi.Handlers.OrderDataHandler">OrderDataHandlers</see>.
    /// Provides virtual and abstract members that can be overridden as needed by subclasses.
    /// </summary>
    public abstract class BaseDomainHandler : OrderDataHandler
    {
        /// <summary>
        /// The service type to use for provisioning packages connected to the domain item handled by the subclassing handler
        /// </summary>
        public abstract string DefaultAtomiaService { get; }

        /// <summary>
        /// The Atomia Billing product categories handled by the subclassing handler.
        /// </summary>
        public abstract IEnumerable<string> HandledCategories { get; }

        /// <summary>
        /// Amends order based on how subclass overrides virtual methods
        /// </summary>
        public sealed override PublicOrder AmendOrder(PublicOrder order, PublicOrderContext orderContext)
        {
            var domainItems = orderContext.ItemData.Where(i => this.HandledCategories.Intersect(i.Categories.Select(c => c.Name)).Count() > 0);

            foreach (var domainItem in domainItems)
            {
                var customData = new List<PublicOrderItemProperty>();

                var domainName = GetDomainName(domainItem);
                var connectedItem = GetConnectedItem(orderContext.ItemData, domainItem, domainName);
                
                customData.Add(new PublicOrderItemProperty { Name = "DomainName", Value = domainName });

                if (connectedItem != null)
                {
                    var atomiaService = GetAtomiaService(connectedItem);
                    customData.Add(new PublicOrderItemProperty { Name = "AtomiaService", Value = atomiaService });

                    var extraProperties = GetAtomiaServiceExtraProperties(domainItem);
                    if (!String.IsNullOrEmpty(extraProperties))
                    {
                        customData.Add(new PublicOrderItemProperty
                        {
                            Name = "AtomiaServiceExtraProperties",
                            Value = extraProperties
                        });
                    }
                }
                else
                {
                    customData.Add(new PublicOrderItemProperty { Name = "AtomiaService", Value = DefaultAtomiaService });
                }

                var domainRegContactProvider = new DomainRegContactProvider(orderContext);
                if (!string.IsNullOrEmpty(domainRegContactProvider.DomainRegContactData))
                {
                    customData.Add(new PublicOrderItemProperty { Name = "DomainRegContact", Value = domainRegContactProvider.DomainRegContactData });
                }

                foreach(var extraData in GetExtraCustomData(domainItem))
                {
                    customData.Add(extraData);
                }

                var orderItem = new PublicOrderItem
                {
                    ItemId = Guid.Empty,
                    ItemNumber = domainItem.ArticleNumber,
                    Quantity = 1,
                    RenewalPeriodId = domainItem.RenewalPeriodId,
                    CustomData = customData.ToArray()
                };

                Add(order, orderItem);
            }

            return order;
        }

        /// <summary>
        /// Get domain name to add as custom attribute on the order item.
        /// </summary>
        protected virtual string GetDomainName(ItemData domainItem)
        {
            var domainNameAttr = domainItem.CartItem.CustomAttributes.FirstOrDefault(ca => ca.Name == "DomainName");

            if (domainNameAttr == null)
            {
                throw new InvalidOperationException("Domain registration cart item must have CustomAttribute \"DomainName\".");
            }

            var domainName = Normalize(domainNameAttr.Value);

            return domainName;
        }

        /// <summary>
        /// Get any other items, like packages, that are connected to the domain item's domain name.
        /// </summary>
        protected virtual ItemData GetConnectedItem(IEnumerable<ItemData> allItems, ItemData domainItem, string domainName)
        {
            var connectedItem = allItems.FirstOrDefault(other =>
                    other.ArticleNumber != domainItem.ArticleNumber &&
                    other.CartItem.CustomAttributes.Any(ca => ca.Name == "DomainName" && ca.Value == domainName));

            return connectedItem;
        }

        /// <summary>
        /// Get the service that should be used to provision any items connected to the domain item's domain name.
        /// </summary>
        /// <remarks>Added as custom attribute on domain order item</remarks>
        protected virtual string GetAtomiaService(ItemData connectedItem)
        {
            return DefaultAtomiaService;
        }

        /// <summary>
        /// Get any AtomiaServiceExtraProperties needed to provision connected item.
        /// </summary>
        /// <remarks>Added as custom attribute on domain order item</remarks>
        protected virtual string GetAtomiaServiceExtraProperties(ItemData connectedItem)
        {
            var extraProperties = String.Empty;

            if (connectedItem != null)
            {
                var extraPropertiesAttr = connectedItem.Product.CustomAttributes.FirstOrDefault(ca => ca.Name == "atomiaserviceextraproperties");
                if (extraPropertiesAttr != null)
                {
                    extraProperties = extraPropertiesAttr.Value;
                }
            }

            return extraProperties;
        }

        /// <summary>
        /// Get any custom attributes needed on the domain order item apart from AtomiaService and AtomiaServiceExtraProperties
        /// </summary>
        /// <param name="domainItem"></param>
        /// <returns></returns>
        protected virtual IEnumerable<PublicOrderItemProperty> GetExtraCustomData(ItemData domainItem)
        {
            return new List<PublicOrderItemProperty>();
        }

        /// <summary>
        /// Helper method to check if HostingPackage product is allowed to have website provisioned by default.
        /// </summary>
        protected bool IsHostingPackageWithWebsitesAllowed(ItemData connectedItem)
        {
            return (connectedItem.Categories.Select(c => c.Name).Contains("HostingPackage") &&
                    !connectedItem.Product.CustomAttributes.Any(ca => ca.Name == "nowebsites" && ca.Value.ToLowerInvariant() == "true"));
        }
    }
}
