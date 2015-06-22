(function () {
    "use strict";

    angular.module("app")
        .controller("DetailsController", DetailsController);

    function DetailsController($http, $log, $interval) {
        var ctrl = this;
        ctrl.nodes = [];
        ctrl.applicationTypes = [];
        ctrl.applications = [];

        ctrl.healthStateToLabelClass = healthStateToLabelClass;
        ctrl.healthStateToRowClass = healthStateToRowClass;

        loadData();

        //ctrl.timer = $interval(loadData, 500);

        function loadData() {
            $http.get('api/fabric/nodes')
                .success(function (data) {
                    ctrl.nodes = data;
                });
            $http.get('api/fabric/applicationTypes')
                .success(function (data) {
                    ctrl.applicationTypes = data;
                    ctrl.applicationTypes.forEach(function (applicationType) {
                        $http.get('api/fabric/serviceTypes/' + applicationType.applicationTypeName + '/' + applicationType.applicationTypeVersion)
                            .success(function (data2) {
                                applicationType.serviceTypes = data2;
                            });
                    });
                });
            $http.get('api/fabric/applications')
                .success(function (data) {
                    ctrl.applications = data;
                    ctrl.applications.forEach(function (application) {
                        $http.get('api/fabric/services/' + application.applicationName)
                            .success(function (data2) {
                                application.services = data2;
                                application.services.forEach(function (service) {
                                    $http.get('api/fabric/partitions/' + service.serviceName)
                                        .success(function (data3) {
                                            service.partitions = data3;
                                            service.partitions.forEach(function (partition) {
                                                $http.get('api/fabric/replicas/' + partition.partitionInformation.id)
                                                    .success(function (data3) {
                                                        partition.replicas = data3;
                                                    });
                                            });
                                        });
                                });
                            });
                    });
                });
        };

        function healthStateToRowClass(healthState) {
            switch (healthState) {
                case 'Ok':
                    return '';
                case 'Warning':
                    return 'warning';
                case 'Error':
                    return 'danger';
                default :
                    return 'default';
            }
        }

        function healthStateToLabelClass(healthState) {
            switch (healthState) {
                case 'Ok':
                    return 'label-success';
                case 'Warning':
                    return 'label-warning';
                case 'Error':
                    return 'label-danger';
                default :
                    return 'label-default';
            }
        }
    }
})();