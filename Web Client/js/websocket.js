$(document).ready(function(){
	//create a new WebSocket object.
	var wsUri = "ws://localhost:9000/polistudio/server.php"; 	
	websocket = new WebSocket(wsUri); 
	
	websocket.onopen = function(ev) { // connection is open 
		//$('#room').append("<div class=\"system_msg\">Connected!</div>"); //notify user
	}

	
	//#### Message received from server?
	websocket.onmessage = function(ev) {
		var msg = JSON.parse(ev.data); //PHP sends Json data
		var type = msg.type; //message type
		var umsg = msg.message; //message text
		var uname = msg.name; //user name
		var ucolor = msg.color; //color
		
		var spotSearchGenerator = function(spot){
			return function(o){ return o.id == spot.id;}		
		}
		
		var new_spots = [];
		for (var key in msg) {
			var lambda = spotSearchGenerator(msg[key]);
			var tmp = _.find(spots, lambda);
			if(tmp == undefined){
				new_spots.push({'id': msg[key].id, 'x': msg[key].x, 'y':msg[key].y, 'isFree':msg[key].isFree});				
				tableAndSpotJoin(new_spots);
			}
			else 
				tmp.isFree = msg[key].isFree;			
		}
		if(new_spots.length > 0){
			drawSpots(new_spots);
			spots = spots.concat(new_spots);
		}
		updateSpot();	
		printOccupiedSeats();		
	};
	
	websocket.onerror	= function(ev){$('#message_box').append("<div class=\"system_error\">Error Occurred - "+ev.data+"</div>");}; 
	websocket.onclose 	= function(ev){$('#message_box').append("<div class=\"system_msg\">Connection Closed</div>");}; 
});