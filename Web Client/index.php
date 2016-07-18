<?php
	include("myFunctions.php");
	
	if(isset($_POST['newState'])){		
		setStateIntoDB($_POST['newState']);
		$state = $_POST['newState'];
		if($_POST['newState'] == "Learning")
			$newState = "Monitoring";
		else
			$newState = "Learning";
	}
	else{
		$state = getStateFromDB();					
		if($state == null){
			$state = "Undefined";
		}
		if($state == "Learning")
			$newState = "Monitoring";
		else
			$newState = "Learning";
	}
?>
<html>
<head>
	<title>Polistudio</title>
	<link rel="stylesheet" href="css/style.css">
	<link rel="stylesheet" href="css/bootstrap.css">
	<script src="js/ajax.js" type="text/javascript"></script>
	<script src="js/lodash.js" type="text/javascript"></script>
	<script src="js/websocket.js" type="text/javascript"></script>
	<script src="js/d3.min.js" type="text/javascript"></script>
	<script src="js/drawRoom.js" type="text/javascript"></script>
	<script src="js/drawGraph.js" type="text/javascript"></script>
	<script src="d3LineChart.js" type="text/javascript"></script>
</head>
<body class='bg'>
	<?php
		getTablesFromDB();
		getSpotsFromDB();
	?>
	<div id="header">
	</div>
	<div id="content">
		<div id='roomname'>
			<form method="POST">
				<input type="hidden" name="newState" value="<?php echo($newState)?>">
				<input class="mybtn" type="submit" value="State: <?php echo($state)?>">
			</form>
			<p class='title'>Castelfidardo study room</p>
		</div>
		<div id='main'>
			<div id="stats" class='bg'>
				<div id="info">
					<?php
						if($state == "Monitoring")
							echo "<p class='stat'>Occupied seats: <span id='occupiedSeats'></span>/<span id='totalSeats'></span></p>
								  <script>printOccupiedSeats()</script>";
					?>
				</div>
				<div id="graphs">
					<div id="switchDiv">
						<button type="button" class="mybtn" onClick="switchGraph();"><</Button>
						<span id="graph_name"></span>						
						<button type="button" class="mybtn" onClick="switchGraph();">></Button>
						<script>
							graph_name = "Daily occupation";
							printGraphName(graph_name);
						</script>
					</div>
					<div id="barChart">
						<script>drawBarChart('08-07-2016.tsv');</script>
						<button class="mybtn graphButton" type="button" onCLick="drawBarChart('04-07-2016.tsv')">04 Jul</button>
						<button class="mybtn graphButton" type="button" onCLick="drawBarChart('05-07-2016.tsv')">05 Jul</button>
						<button class="mybtn graphButton" type="button" onCLick="drawBarChart('06-07-2016.tsv')">06 Jul</button>
						<button class="mybtn graphButton" type="button" onCLick="drawBarChart('07-07-2016.tsv')">07 Jul</button>
						<button class="mybtn graphButton" type="button" onCLick="drawBarChart('08-07-2016.tsv')">08 Jul</button>
					</div>
					<div id="lineChart">						
						<script>
							month = "giugno-2016.tsv";
							drawLineChart(month);
						</script>
						<button id="button" class="mybtn graphButton" type="button" onCLick="drawLineChart(switchMonth());writeMonth();"></button>
						<script>writeMonth()</script>
					</div>
				</div>
			</div>
			<div id="room">
				<script>
					tableAndSpotJoin(spots);
					drawTables();					
					drawSpots(spots);
				</script>
			</div>
		</div>	
		</div>
	</div>
	<!--script>
		for (var key in spots) {
		  if (spots.hasOwnProperty(key)) {
			  $('#room').append("<div><span>");
			  for (var internKey in spots[key])
				$('#room').append(" " + internKey+": "+spots[key][internKey]+"</span>");
			$('#room').append("</div>");
		  }
		}
	</script-->
</body>
</html>