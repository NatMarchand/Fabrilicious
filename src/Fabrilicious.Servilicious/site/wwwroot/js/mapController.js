(function () {
    "use strict";

    angular.module("app")
        .controller("MapController", MapController);

    function MapController($http, $log, $interval, $document) {
        var ctrl = this;

        ctrl.nodes = [];
        ctrl.highlightPartition = highlightPartition;
        ctrl.lastUpdate = null;
        loadData();

        ctrl.timer = $interval(loadData, 1000);

        function loadData() {
            $http.get('api/fabric/details')
                .success(function (data) {
                    ctrl.nodes = data;
                    ctrl.lastUpdate = new Date();
                });
        }

        function highlightPartition(partition, isOn) {
            var all = $document[0].querySelectorAll('.partition-' + partition);
            [].forEach.call(all, function (element) {
                if(isOn)
                    angular.element(element).addClass('bg-primary');
                else
                    angular.element(element).removeClass('bg-primary');
            });
        }
    }
})();