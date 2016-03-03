/* jshint -W079 */
var Atomia = Atomia || {};
Atomia.ViewModels = Atomia.ViewModels || {};
/* jshint +W079 */

(function (exports, _, ko) {
	'use strict';

	function ProductVPSItem(productData, sliderConfig) {
	    var self = this;

		self.selectedPricingVariant = ko.observable();

		self.articleNumber = productData.ArticleNumber;
		self.name = productData.Name;
		self.description = productData.Description;
		self.sliderConfig = sliderConfig[productData.ArticleNumber];

		self.selectedCpuQuantityIndex = ko.observable();
		self.selectedRamQuantityIndex = ko.observable();
		self.selectedDiskQuantityIndex = ko.observable();

		self.selectedCpuQuantityIndex(self.sliderConfig.cpu.startIndex);
		self.selectedRamQuantityIndex(self.sliderConfig.ram.startIndex);
		self.selectedDiskQuantityIndex(self.sliderConfig.disk.startIndex);

		self.cpuPrices = _.find(productData.PricingVariants, function (variantData) {
		    return variantData.FixedPrice == false && variantData.CounterType.CounterId == "cpu";
		});
		self.ramPrices = _.find(productData.PricingVariants, function (variantData) {
		    return variantData.FixedPrice == false && variantData.CounterType.CounterId == "ram";
		});
		self.diskPrices = _.find(productData.PricingVariants, function (variantData) {
		    return variantData.FixedPrice == false && variantData.CounterType.CounterId == "disk";
		});

	    /** 
         * Checks if item is equal to other item based on article number.
         * @param {Object} other - The item to compare to
         * @returns {boolean} whether the items are equal or not.
         */
		self.equals = function equals(other) {
		    return self.articleNumber === other.articleNumber;
		};

		/** Shortcut to price of pricing variant. */
		self.price = ko.pureComputed(function () {
		    var cpuUnitPrice = _.find(self.cpuPrices.CounterType.Ranges, function (range) {
		        return range.LowerMargin <= self.sliderConfig.cpu.values[self.selectedCpuQuantityIndex()] / self.cpuPrices.CounterType.UnitValue && range.UpperMargin >= self.sliderConfig.cpu.values[self.selectedCpuQuantityIndex()] / self.cpuPrices.CounterType.UnitValue;
		    }).Price;
		    var ramUnitPrice = _.find(self.ramPrices.CounterType.Ranges, function (range) {
		        return range.LowerMargin <= self.sliderConfig.ram.values[self.selectedRamQuantityIndex()] / self.ramPrices.CounterType.UnitValue && range.UpperMargin >= self.sliderConfig.ram.values[self.selectedRamQuantityIndex()] / self.ramPrices.CounterType.UnitValue;
		    }).Price;
		    var diskUnitPrice = _.find(self.diskPrices.CounterType.Ranges, function (range) {
		        return range.LowerMargin <= self.sliderConfig.disk.values[self.selectedDiskQuantityIndex()] / self.diskPrices.CounterType.UnitValue && range.UpperMargin >= self.sliderConfig.disk.values[self.selectedDiskQuantityIndex()] / self.diskPrices.CounterType.UnitValue;
		    }).Price;

		    return cpuUnitPrice * self.sliderConfig.cpu.values[self.selectedCpuQuantityIndex()] + ramUnitPrice * self.sliderConfig.ram.values[self.selectedRamQuantityIndex()] + diskUnitPrice * self.sliderConfig.disk.values[self.selectedDiskQuantityIndex()];
		});
	}

	function VPSCalculator(response, sliderConfigJson) {
	    var self = this;

	    self.products = ko.observableArray();
	    self.selectedProduct = ko.observable();

	    var sliderConfig = JSON.parse(sliderConfigJson);

	    /** Create createProductVPSItem object. */
	    self.createProductVPSItem = function CreateProductVPSItem(productData) {
	        return new ProductVPSItem(productData, sliderConfig);
	    };

	    /** Select product and update calculator. */
	    self.selectProduct = function SelectProduct(item) {
	        self.selectedProduct(item);
	    };

	    var products = response.data.CategoryData.Products;

	    _.each(products, function (product) {
	        var item = self.createProductVPSItem(product);
	        self.products.push(item);
	    });

	    if (self.products().length > 0) {
	        self.selectProduct(self.products()[0]);
	    }
	}

	_.extend(exports, {
	    ProductVPSItem: ProductVPSItem,
	    VPSCalculator: VPSCalculator
	});

})(Atomia.ViewModels, _, ko);
