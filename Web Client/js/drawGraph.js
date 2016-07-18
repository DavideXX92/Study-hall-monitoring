function drawBarChart(file){
	var margin = {top: 20, right: 20, bottom: 30, left: 40},
		width = $("#graphs").width() - margin.left - margin.right;
		height = $("#graphs").height() * 90 / 100 - margin.top - margin.bottom;
		
	var x = d3.scale.ordinal()
		.rangeRoundBands([0, width], .1);

	var y = d3.scale.linear()
		.range([height, 0]);

	var xAxis = d3.svg.axis()
		.scale(x)
		.orient("bottom");

	var yAxis = d3.svg.axis()
		.scale(y)
		.orient("left");
		//.ticks(10, "%");

	var svg = d3.select('#barChart').select('svg').select('g');
	var isupdate = true;
	if(svg.empty()){
		var svg = d3.select("#barChart").append("svg")
			.attr("width", width + margin.left + margin.right)
			.attr("height", height + margin.top + margin.bottom)
		  .append("g")
			.attr("transform", "translate(" + margin.left + "," + margin.top + ")");
		isupdate = false;
	}

	d3.tsv("doc/" + file, type, function(error, data) {
		if (error) throw error;

		x.domain(data.map(function(d) { return d.day; }));
		y.domain([0, d3.max(data, function(d) { return d.seatsOccupied; })]);

		if(!isupdate){
			svg.append("g")
				.attr("class", "x axis")
				.attr("transform", "translate(0," + height + ")")
				.call(xAxis)
				.append("text")
			    .attr("transform", "rotate(0)")
			    .attr("x", width)
				.attr("y", 25)
			    .attr("dx", "0")
			    .style("text-anchor", "end")
			    .text("Day of month");	;

			svg.append("g")
				.attr("class", "y axis")
				.call(yAxis)
				.append("text")
				.attr("transform", "rotate(-90)")
				.attr("y", 6)
				.attr("dy", ".71em")
				.style("text-anchor", "end")
				.text("Occupied seats");
				
			svg.selectAll(".bar")
			  .data(data)
			.enter().append("rect")
			  .attr("class", "bar")
			  .attr("x", function(d) { return x(d.day); })
			  .attr("width", x.rangeBand())
			  .attr("y", function(d) { return y(d.seatsOccupied); })
			  .attr("height", function(d) { return height - y(d.seatsOccupied); });
		}
		else{
			/*svg.append("g")
				.attr("class", "x axis")
				.attr("transform", "translate(0," + height + ")")
				.call(xAxis)
				.transition()
				.delay(0)
				.duration(2000);*/

			svg.append("g")
				.attr("class", "y axis")
				.call(yAxis)
				.transition()
				.delay(0)
				.duration(2000)
				.attr("transform", "rotate(-90)")
				.attr("y", 6)
				.attr("dy", ".71em")
				.style("text-anchor", "end")
				.text("Occupied seats");
				
			svg.selectAll(".bar")
			  .data(data)
			  .attr("class", "bar")
			  .transition()
			  .delay(0)
			  .duration(2000)
			  .attr("x", function(d) { return x(d.day); })
			  .attr("width", x.rangeBand())
			  .attr("y", function(d) { return y(d.seatsOccupied); })
			  .attr("height", function(d) { return height - y(d.seatsOccupied); });
		}
	});

	function type(d) {
	  d.seatsOccupied = +d.seatsOccupied;
	  return d;
	}
}

var yAxisGroup = null,
	xAxisGroup = null,
	dataCirclesGroup = null,
	dataLinesGroup = null;
	
function drawLineChart(file) {	
	var w = $("#graphs").width();
	var h = $("#graphs").height() * 90 / 100;
	var monthNames = [ "January", "February", "March", "April", "May", "June",
	"July", "August", "September", "October", "November", "December" ];
	var maxDataPointsForDots = 50;
	var transitionDuration = 1000;
				
	//var data = generateData();
	d3.tsv("doc/" + file, type, function(error, data) {
		if (error) throw error;
		console.log(data);
		
		var margin = 40;
		var max = d3.max(data, function(d) { return d.value });
		var max_y = d3.max(data, function(d) { return d.date });
		var min = 0;
		var pointRadius = 4;
		//var x = d3.time.scale().range([0, w - margin * 2]).domain([data[0].date, data[data.length - 1].date]);
		var x = d3.scale.linear().range([0, w - margin * 2]).domain([min, max_y]);
		var y = d3.scale.linear().range([h - margin * 2, 0]).domain([min, max]);
		var xAxis = d3.svg.axis().scale(x).tickSize(h - margin * 2).tickPadding(10).ticks(7);
		var yAxis = d3.svg.axis().scale(y).orient('left').tickSize(-w + margin * 2).tickPadding(10);

		var svg = d3.select('#lineChart').select('svg').select('g');
		if (svg.empty()) {
			svg = d3.select('#lineChart')
				.append('svg:svg')
					.attr('width', w)
					.attr('height', h)
					.attr('class', 'viz')
				.append('svg:g')
					.attr('transform', 'translate(' + margin + ',' + margin + ')');
		}

		t = svg.transition().duration(transitionDuration);



		// y ticks and labels
		if (!yAxisGroup) {
			yAxisGroup = svg.append('svg:g')
				.attr('class', 'yTick')
				.call(yAxis)
				.append("text")
			    .attr("transform", "rotate(-90)")
			    .attr("y", 6)
			    .attr("dy", ".71em")
			    .style("text-anchor", "end")
			    .text("Occupied seats");		
		}
		else {
			t.select('.yTick').call(yAxis);
		}

		// x ticks and labels
		if (!xAxisGroup) {
			xAxisGroup = svg.append('svg:g')
				.attr('class', 'xTick')
				.call(xAxis)
				.append("text")
			    .attr("transform", "rotate(0)")
			    .attr("x", w - 75)
				.attr("y", 293)
			    .attr("dx", "0")
			    .style("text-anchor", "end")
			    .text("Day of month");					
		}
		else {
			t.select('.xTick').call(xAxis);
		}

		// Draw the lines
		if (!dataLinesGroup) {
			dataLinesGroup = svg.append('svg:g');
		}

		var dataLines = dataLinesGroup.selectAll('.data-line')
				.data([data]);

		var line = d3.svg.line()
			// assign the X function to plot our line as we wish
			.x(function(d,i) { 
				// verbose logging to show what's actually being done
				//console.log('Plotting X value for date: ' + d.date + ' using index: ' + i + ' to be at: ' + x(d.date) + ' using our xScale.');
				// return the X coordinate where we want to plot this datapoint
				//return x(i); 
				return x(d.date); 
			})
			.y(function(d) { 
				// verbose logging to show what's actually being done
				//console.log('Plotting Y value for data value: ' + d.value + ' to be at: ' + y(d.value) + " using our yScale.");
				// return the Y coordinate where we want to plot this datapoint
				//return y(d); 
				return y(d.value); 
			})
			.interpolate("linear");

			 /*
			 .attr("d", d3.svg.line()
			 .x(function(d) { return x(d.date); })
			 .y(function(d) { return y(0); }))
			 .transition()
			 .delay(transitionDuration / 2)
			 .duration(transitionDuration)
				.style('opacity', 1)
							.attr("transform", function(d) { return "translate(" + x(d.date) + "," + y(d.value) + ")"; });
			  */

		var garea = d3.svg.area()
			.interpolate("linear")
			.x(function(d) { 
				// verbose logging to show what's actually being done
				return x(d.date); 
			})
					.y0(h - margin * 2)
			.y1(function(d) { 
				// verbose logging to show what's actually being done
				return y(d.value); 
			});

		dataLines
			.enter()
			.append('svg:path')
					//.attr("class", "area")
					//.attr("d", garea(data));

		dataLines.enter().append('path')
			 .attr('class', 'data-line')
			 .style('opacity', 0.3)
			 .attr("d", line(data));
			/*
			.transition()
			.delay(transitionDuration / 2)
			.duration(transitionDuration)
				.style('opacity', 1)
				.attr('x1', function(d, i) { return (i > 0) ? xScale(data[i - 1].date) : xScale(d.date); })
				.attr('y1', function(d, i) { return (i > 0) ? yScale(data[i - 1].value) : yScale(d.value); })
				.attr('x2', function(d) { return xScale(d.date); })
				.attr('y2', function(d) { return yScale(d.value); });
			*/

		dataLines.transition()
			.attr("d", line)
			.duration(transitionDuration)
				.style('opacity', 1)
							.attr("transform", function(d) { return "translate(" + x(d.date) + "," + y(d.value) + ")"; });

		dataLines.exit()
			.transition()
			.attr("d", line)
			.duration(transitionDuration)
							.attr("transform", function(d) { return "translate(" + x(d.date) + "," + y(0) + ")"; })
				.style('opacity', 1e-6)
				.remove();

		d3.selectAll(".area").transition()
			.duration(transitionDuration)
			.attr("d", garea(data));

		// Draw the points
		if (!dataCirclesGroup) {
			dataCirclesGroup = svg.append('svg:g');
		}

		var circles = dataCirclesGroup.selectAll('.data-point')
			.data(data);

		circles
			.enter()
				.append('svg:circle')
					.attr('class', 'data-point')
					.style('opacity', 1e-6)
					.attr('cx', function(d) { return x(d.date) })
					.attr('cy', function() { return y(0) })
					.attr('r', function() { return (data.length <= maxDataPointsForDots) ? pointRadius : 0 })
				.transition()
				.duration(transitionDuration)
					.style('opacity', 1)
					.attr('cx', function(d) { return x(d.date) })
					.attr('cy', function(d) { return y(d.value) });

		circles
			.transition()
			.duration(transitionDuration)
				.attr('cx', function(d) { return x(d.date) })
				.attr('cy', function(d) { return y(d.value) })
				.attr('r', function() { return (data.length <= maxDataPointsForDots) ? pointRadius : 0 })
				.style('opacity', 1);

		circles
			.exit()
				.transition()
				.duration(transitionDuration)
					// Leave the cx transition off. Allowing the points to fall where they lie is best.
					//.attr('cx', function(d, i) { return xScale(i) })
					.attr('cy', function() { return y(0) })
					.style("opacity", 1e-6)
					.remove();
	});
	
	function type(d) {
	  d.date = +d.date;
	  return d;
	}
}

function generateData() {
	var data = [];
	var i = Math.max(Math.round(Math.random()*100), 3);

	while (i--) {
		var date = new Date();
		date.setDate(date.getDate() - i);
		date.setHours(0, 0, 0, 0);
		data.push({'value' : Math.round(Math.random()*1200), 'date' : date});
	}
	return data;
}

function printGraphName(name){
	$('#graph_name').text(name);
}

function switchGraph(){
	if(graph_name == "Daily occupation"){
		graph_name = "Avarage occupation";
		$('#barChart').css('height', 0);
		$('#barChart').css('visibility', 'hidden');
		$('#lineChart').css('height', 90);
		$('#lineChart').css('visibility', 'visible');
	}
	else{
		graph_name = "Daily occupation";	
		$('#barChart').css('height', 90);
		$('#barChart').css('visibility', 'visible');	
		$('#lineChart').css('height', 0);
		$('#lineChart').css('visibility', 'hidden');		
	}
	$('#graph_name').text(graph_name);
}

function switchMonth(){
	if(month == 'luglio-2016.tsv'){
		month = 'giugno-2016.tsv';
		return month;
	}
	else{
		month = 'luglio-2016.tsv';
		return month;
	}
}

function writeMonth(){
	if(month == 'giugno-2016.tsv')
		$('#button').text("June");
	else
		$('#button').text("July");
}