using Atomia.Store.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Atomia.Store.AspNetMvc.Models
{
    public class CounterTypeModel
    {
        private readonly ICurrencyFormatter currencyFormatter = DependencyResolver.Current.GetService<ICurrencyFormatter>();
        private readonly IResourceProvider resourceProvider = DependencyResolver.Current.GetService<IResourceProvider>();
        private readonly CounterType counterType;

        /// <summary>
        /// Construct an instance from a <see cref="Atomia.Store.Core.CounterType"/>
        /// </summary>
        public CounterTypeModel(CounterType counterType)
        {
            this.counterType = counterType;
        }

        /// <summary>
        /// Counter id
        /// </summary>
        public string CounterId
        {
            get
            {
                return counterType.CounterId;
            }
        }

        /// <summary>
        /// Counter name
        /// </summary>
        public string Name
        {
            get
            {
                return counterType.Name;
            }
        }

        /// <summary>
        /// Counter description
        /// </summary>
        public string Description
        {
            get
            {
                return counterType.Description;
            }
        }

        /// <summary>
        /// Display name
        /// </summary>
        public string Display
        {
            get
            {
                return counterType.Name;
            }
        }

        /// <summary>
        /// Counter unit name
        /// </summary>
        public string UnitName
        {
            get
            {
                return counterType.UnitName;
            }
        }

        /// <summary>
        /// Range unit value
        /// </Counter>
        public decimal UnitValue
        {
            get
            {
                return counterType.UnitValue;
            }
        }

        /// <summary>
        /// Require subscription
        /// </Counter>
        public bool RequireSubscription
        {
            get
            {
                return counterType.RequireSubscription;
            }
        }

        /// <summary>
        /// Counter ranges
        /// </summary>
        public IReadOnlyCollection<CounterRangeModel> Ranges
        {
            get
            {
                return counterType.Ranges.OrderBy(r => r.LowerMargin).Select(r => new CounterRangeModel(r, counterType.UnitName)).ToList();
            }
        }
    }
}
