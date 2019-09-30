(function() {
    'use strict';

    angular
        .module('simoonaApp.Events')
        .factory('eventRepository', eventRepository);

    eventRepository.$inject = [
        '$resource',
        '$http',
        'endPoint'
    ];

    function eventRepository($resource, $http, endPoint) {
        var eventUrl = endPoint + '/Event/';
        var applicationUrl = endPoint + '/ApplicationUser/';

        var service = {
            getAllEvents: getAllEvents,
            getEventOffices: getEventOffices,
            getEventTypes: getEventTypes,
            getEventRecurringTypes: getEventRecurringTypes,
            getEventsByTypeAndOffice: getEventsByTypeAndOffice,
            getMyEvents: getMyEvents,
            getEventDetails: getEventDetails,
            getEventUpdate: getEventUpdate,
            getEventOptions: getEventOptions,
            deleteEvent: deleteEvent,
            createEvent: createEvent,
            updateEvent: updateEvent,
            addColleagues: addColleagues,
            joinEvent: joinEvent,
            leaveEvent: leaveEvent,
            expelUserFromEvent: expelUserFromEvent,
            resetParticipantList: resetParticipantList,
            getUserForAutoComplete: getUserForAutoComplete,
            getUserResponsiblePersonById: getUserResponsiblePersonById,
            getUserForAutoCompleteResponsiblePerson: getUserForAutoCompleteResponsiblePerson,
            exportParticipants: exportParticipants,
            getMaxEventParticipants: getMaxEventParticipants
        };
        return service;

        /////////

        function getAllEvents() {
            return $resource(eventUrl + 'All').query().$promise;
        }

        function getEventOffices() {
            return $resource(eventUrl + 'Offices').query().$promise;
        }

        function getEventTypes() {
            return $resource(eventUrl + 'Types').query().$promise;
        }

        function getEventRecurringTypes() {
            return $resource(eventUrl + 'Recurrences').query().$promise;
        }

        function getEventsByTypeAndOffice(typeId, officeId) {
            return $resource(eventUrl + 'ByTypeAndOffice').query({ typeId: typeId, officeId: officeId }).$promise;
        }

        function getMyEvents(filter, officeId) {
            return $resource(eventUrl + 'MyEvents').query({ filter: filter, officeId: officeId }).$promise;
        }

        function getEventUpdate(id) {
            return $resource(eventUrl + 'Update').get({ eventId: id }).$promise;
        }

        function getEventDetails(id) {
            return $resource(eventUrl + 'Details').get({ eventId: id }).$promise;
        }

        function createEvent(event) {
            return $resource(eventUrl + 'Create').save(event).$promise;
        }

        function updateEvent(event) {
            return $resource(eventUrl + 'Update', '', {
                put: {
                    method: 'PUT'
                }
            }).put(event).$promise;
        }

        function deleteEvent(id) {
            return $resource(eventUrl + 'Delete').delete({ eventId: id }).$promise;
        }

        function getEventOptions(eventId) {
            return $resource(eventUrl + 'Options?eventId=:eventId', {
                eventId: eventId
            }).get().$promise;
        }

        function getUserForAutoCompleteResponsiblePerson(params) {
            return $resource(applicationUrl + 'GetForAutoComplete').query({
                s: params
            }).$promise;
        }

        function getUserForAutoComplete(params, eventId) {
            return $resource(eventUrl + 'GetUsersForAutoComplete').query({
                s: params,
                eventId: eventId
            }).$promise;
        }

        function getUserResponsiblePersonById(id) {
            return $resource(applicationUrl + 'Get').get({
                id: id
            }).$promise;
        }

        function addColleagues(eventId, chosenOptions, participantIds) {
            return $resource(eventUrl + 'AddColleague').save({
                eventId: eventId,
                chosenOptions: chosenOptions,
                participantIds: participantIds
            }).$promise;
        }

        function joinEvent(eventId, chosenOptions) {
            return $resource(eventUrl + 'Join').save({
                eventId: eventId,
                chosenOptions: chosenOptions
            }).$promise;
        }

        function leaveEvent(eventId, userId) {
            return $resource(eventUrl + 'Leave').delete({
                eventId: eventId,
                userId: userId
            }).$promise;
        }

        function expelUserFromEvent(eventId, userId) {
            return $resource(eventUrl + 'Expel').delete({
                eventId: eventId,
                userId: userId
            }).$promise;
        }

        function resetParticipantList(eventId) {
            return $resource(eventUrl + 'ResetAttendees', '', {
                put: {
                    method: 'PUT',
                    params: {
                        eventId: eventId
                    }
                }
            }).put().$promise;
        }

        function exportParticipants(eventId) {
            return $http.get(eventUrl + 'Export?eventId=' + eventId, {
                responseType: 'arraybuffer'
            });
        }

        function getMaxEventParticipants() {
            return $resource(eventUrl + 'MaxParticipants', '', {
                query: {
                    isArray: false,
                    method: 'GET'
                }
            });
        }
    }
})();
