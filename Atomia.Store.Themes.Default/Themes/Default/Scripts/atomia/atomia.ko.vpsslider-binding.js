
(function ($, ko) {
    'use strict';
    var skipUpdate = false;
    /** Show or hide element with a jQuery slide animation. Binding option 'slideDuration' sets animation time in ms. */
    ko.bindingHandlers.vpsSlider = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var sliderConf;
            var currentValueIndex;
            if (bindingContext.$data.sliderConfig != null) {
                if ($(element).hasClass('cpu')) {
                    sliderConf = bindingContext.$data.sliderConfig.cpu;
                    currentValueIndex = bindingContext.$data.selectedCpuQuantityIndex();
                }
                if ($(element).hasClass('ram')) {
                    sliderConf = bindingContext.$data.sliderConfig.ram;
                    currentValueIndex = bindingContext.$data.selectedRamQuantityIndex();
                }
                if ($(element).hasClass('disk')) {
                    sliderConf = bindingContext.$data.sliderConfig.disk;
                    currentValueIndex = bindingContext.$data.selectedDiskQuantityIndex();
                }

                $(element).slider({
                    value: currentValueIndex + 1,
                    min: 1,
                    max: sliderConf.values.length,
                    range: "min",
                    step: 1,
                    animate: true,
                    slide: function (event, ui) {
                        skipUpdate = true;
                        var observable = valueAccessor();
                        observable(ui.value - 1);
                    }
                });
            }
        },
        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            ko.unwrap(valueAccessor());
            var observable = valueAccessor();
            if (!skipUpdate) {
                $(element).slider('value', observable() + 1);
            }
            else {
                skipUpdate = false;
            }
        }
    };

}(jQuery, ko));
