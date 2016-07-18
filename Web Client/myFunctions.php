<?php
function myConnect(){
	$conn = @mysqli_connect('localhost', 'root','', 'polistudio');
	if(!$conn)
		throw new Exception();
	return $conn;
}

function getTablesFromDB(){
	echo("<script>
			tables = new Array();\n");
	$conn = myConnect();
		if(!$conn)
			return -2;
		
	$query = "SELECT id, x_TopL, y_TopL, height, width FROM room_table WHERE room_id = 1;";	
		if(! ($risposta = mysqli_query($conn, $query)) ){
			mysqli_close($conn);
			return -2;
		}					
		mysqli_close($conn);
		$json = array();
		while( $riga = mysqli_fetch_array($risposta) ){
			echo("tables.push({'id': ".$riga[0].", 'x_TopL': ".$riga[1].", 'y_TopL':".$riga[2].
												", 'height': ".$riga[3].", 'width':".$riga[4]."});\n");			
		}
		echo("	tables = _.sortBy(tables, function(o){return o.x_TopL;});
				var i = 1;
				for(var key in tables)
					tables[key].pos=i++;
				</script>");
}

function getSpotsFromDB(){
	echo("<script>
			spots = new Array();\n");
	$conn = myConnect();
		if(!$conn)
			return -2;
		
	$query = "SELECT id, x, y, is_free FROM spot WHERE room_id = 1;";	
		if(! ($risposta = mysqli_query($conn, $query)) ){
			mysqli_close($conn);
			return -2;
		}					
		mysqli_close($conn);
		$json = array();
		while( $riga = mysqli_fetch_array($risposta) ){
			echo("spots.push({'id': ".$riga[0].", 'x': ".$riga[1].", 'y':".$riga[2].", 'isFree':".$riga[3]."});\n");			
		}
		echo("</script>");
}

function getStateFromDB(){
	$conn = myConnect();
		if(!$conn)
			return null;
		
	$query = "SELECT state FROM studyroom WHERE id = 1;";	
		if(! ($risposta = mysqli_query($conn, $query)) ){
			mysqli_close($conn);
			return null;
		}					
		mysqli_close($conn);
		if( $riga = mysqli_fetch_array($risposta) ){
			return $riga[0];	
		}
		else
			return null;
}

function setStateIntoDB($new_state){
	$conn = myConnect();
		if(!$conn)
			return null;
	$query = "UPDATE studyroom SET state = '".$new_state."' WHERE id = 1";
	
	if(! ($risposta = mysqli_query($conn, $query)) ){
		mysqli_close($conn);
		return 0;
	}
	mysqli_close($conn);
	return 1;	
}
?>