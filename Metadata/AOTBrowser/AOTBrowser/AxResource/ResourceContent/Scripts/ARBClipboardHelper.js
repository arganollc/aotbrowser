(function () {
    "use strict";

    $dyn.ui.defaults.ARBClipboardHelper = {};

    $dyn.controls.ARBClipboardHelper = function (data, element) {
        var self = this;

        $dyn.ui.Control.apply(self, arguments);
        $dyn.ui.applyDefaults(self, data, $dyn.ui.defaults.ARBClipboardHelper);

        element.focus();

        $dyn.observe(this.SetCopyText, function (_data) {
            if (_data != '') {
                navigator.clipboard.writeText(_data);
            }
        });
    }

    $dyn.controls.ARBClipboardHelper.prototype = $dyn.extendPrototype($dyn.ui.Control.prototype, {});
})();