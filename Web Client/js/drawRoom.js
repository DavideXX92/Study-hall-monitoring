width_tables = 30 / 100;
height_tables = 20 / 100;
padding_left_table_1 = 10 / 100;
padding_left_table_2 = 57 / 100;
padding_top_tables = 40 / 100;
dim_spots = 5/100;

function tableAndSpotJoin(spots){		
	var tableSearchGenerator = function(spot){
			return function(table){					
				return (spot.x <= table.x_TopL + table.width + 10 && spot.x >= table.x_TopL - 10) ? true : false;
			}			
	}
	
	for(var skey in spots){		
		var lambda = tableSearchGenerator(spots[skey]);
		var table = _.find(tables, lambda);
		if(table != undefined){
			spots[skey].id_table = table.id;
			spots[skey].pos = spots[skey].y >= table.height / 2 + table.y_TopL ? "bottom" : "top";
		}
		else{
			spots[skey].id_table = -1;
			console.log("Spot non assegnato: " + spots[skey].id)
		}
	}
}

function drawTables(){
	var svgContainer = d3.select("#room").append("svg")
										 .attr("width", $("#room").width())
										 .attr("height", $("#room").height());
	 
	var rectangles = svgContainer.selectAll("rect")
							   .data(tables, function(data){return data.id})
							   .enter()
							   .append("rect")

	var rectangleAttributes = rectangles
							  .attr("x", function(data){if(data.pos == 1 )
															return $("#room").width() * padding_left_table_1;
														else if(data.pos == 2)
															return $("#room").width() * padding_left_table_2})
						      .attr("y", function(data){return $("#room").height() * padding_top_tables})
						      .attr("width", $("#room").width() * width_tables)
							  .attr("height", $("#room").height() * height_tables)
							  .attr("rx", 10)         // set the x corner curve radius
							  .attr("ry", 10)
							  .style("fill", "#c06d1d");
}

function drawSpots(spots){	 
	var svgContainer = d3.select("#room svg");
	 
	var squares = svgContainer.selectAll("rect")
								.data(spots, function(data){return data.id})
								.enter()
								.append("rect");

	var squareAttributes = squares
							.attr("x", function(data){
							    var table = _.find(tables,{'id': data.id_table});
								var width_table =  width_tables * $("#room").width();
								var pos_table_param = (table.pos == 1 ? padding_left_table_1 : padding_left_table_2) * $("#room").width();			
								var dist_left = data.x - table.x_TopL;
								var dist_right = table.x_TopL + table.width - data.x;
								return width_table * (dist_left < dist_right ? 20 / 100 : 70 / 100) + pos_table_param;
						   })
						   .attr("y", function(data){
							   if(data.pos == "top")
									return $("#room").height() * padding_top_tables - $("#room").height() * 15/100;
							   else if(data.pos == "bottom")
									return $("#room").height() * padding_top_tables + $("#room").height() * height_tables + $("#room").height() * 6/100;
						   })
							.attr("width", $("#room").width() * dim_spots)
							.attr("height", $("#room").width() * dim_spots)
							.attr("rx", 10)         // set the x corner curve radius
							.attr("ry", 10)
							.style("fill", function(data){
								if(data.isFree)
									return "green";
								else
									return "red";
						   });
}

function updateSpot(){
	var svgContainer = d3.select("#room svg");
	var squares = svgContainer.selectAll("rect")
							   .data(spots, function(data){return data.id})
							   .transition()
							   .delay(0)
							   .duration(3000)
							   .style("fill", function(data){
								if(data.isFree == 1)
									return "green";
								else
									return "red";
						   });

	var squareAttributes = squares
							.attr("x", function(data){
							    var table = _.find(tables,{'id': data.id_table});
								var width_table =  width_tables * $("#room").width();
								var pos_table_param = (table.pos == 1 ? padding_left_table_1 : padding_left_table_2) * $("#room").width();			
								var dist_left = data.x - table.x_TopL;
								var dist_right = table.x_TopL + table.width - data.x;
								return width_table * (dist_left < dist_right ? 20 / 100 : 70 / 100) + pos_table_param;
						   })
						   .attr("y", function(data){
							   if(data.pos == "top")
									return $("#room").height() * padding_top_tables - $("#room").height() * 15/100;
							   else if(data.pos == "bottom")
									return $("#room").height() * padding_top_tables + $("#room").height() * height_tables + $("#room").height() * 6/100;
						   })
							.attr("width", $("#room").width() * dim_spots)
							.attr("height", $("#room").width() * dim_spots)
							.attr("rx", 10)         // set the x corner curve radius
							.attr("ry", 10);
}

function printOccupiedSeats(isUpdate){
	var totalSeats = spots.length;
	var occupiedSeats = _.filter(spots, function(o){return o.isFree == 1 ? false : true}).length;

	$("#occupiedSeats").text(occupiedSeats);
	$("#totalSeats").text(totalSeats);
}