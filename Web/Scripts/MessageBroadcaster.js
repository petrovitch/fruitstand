/*
MessageBroadcaster
	Manages connections and events to our real-time messaging interface (initially SignalR)
*/
var MessageBroadcaster = function () {
	var messageHub = null,
		groups = [],
		hasStarted = false,
		subscriptions = [],
		Init = function () {
			messageHub = $.connection.developerMessageHub;

			$.connection.hub.error(function(msg) {
				console.error("SignalR error: " + msg);
			});

			// The SignalR JS API only allows one listener to each event, so here we simply register ourselves as the single listener, and other subscribers
			// simply get relayed the info via our subscriptions[] array
			RegisterEvent('APIRequestSent');
			RegisterEvent('APIResponseReceived');
			RegisterEvent('APIError');
				
			// Start hub only after all listeners are configured
			$.connection.hub.start().done(function () {
				hasStarted = true;
				AddGroups();
				console.log('SignalR using ' + $.connection.hub.transport.name + ' transport');
					
				// Close when user leaves page
				$(window).bind('beforeunload', function () {
					CloseConnection();
				});
			});
		},
		CloseConnection = function () {
			console.log("Closing SignalR connection");
			$.connection.hub.stop();
		},
		RegisterEvent = function (eventName) {
			console.log("Registering to event", eventName);
			messageHub.client[eventName] = function (x, y, z) { RaiseEvent(eventName, x, y, z); };
		},
		AddGroups = function () {
			// Connect to new groups?
			while (groups.length) {
				var grp = groups[0];
				messageHub.server.joinGroup(grp.groupType, grp.groupName);
				groups.splice(0, 1);
			}
		},
			
		// The raise event command takes (and proxies) up to three parameters (x,y,z).  Javascript allows us to just append them to our callback handler without throwing any errors if the
		// actual handler does not have this many arguments
		RaiseEvent = function (eventName, x, y, z) {
			// We may have multiple handlers with this event name, so we iterate
			$.each(subscriptions, function (ind, subscription) {
				if (subscription[0] == eventName) subscription[1](x, y, z);
			});
		},

		JoinGroup = function (type, groupID) {
			groups.push({ groupType: type, groupName: groupID });
			if (hasStarted) AddGroups();
		},
		Subscribe = function (eventName, callback) {
			// Push this handler so we can call it back later
			subscriptions.push([eventName, callback]);
		};
	Init();
	return {
		Subscribe: Subscribe,
		JoinGroup: JoinGroup
	};

}();