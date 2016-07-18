<?php
/*
 *
 *	GESTIONE DATABASE
 *
 *
*/
function myConnect(){
		$conn = @mysqli_connect('localhost', 'root','', 'polistudio');
		if(!$conn)
			throw new Exception();
		return $conn;
	}

function getSpotFromDB(){
	$conn = myConnect();
		if(!$conn)
			return -2;
		
	$query = "SELECT id, is_free FROM spot;";	
		if(! ($risposta = mysqli_query($conn, $query)) ){
			mysqli_close($conn);
			return -2;
		}					
		mysqli_close($conn);
		//array('message'=>$user_message)
		$json = array();
		while( $riga = mysqli_fetch_array($risposta) ){
			echo("id: " + $riga['id'] + " is_free: " + $riga['is_free']);
			array_push($json, "{'id':"+ $riga['id'] + ", 'isFree:'"+ $riga['is_free']+"'}");
			//$json.push("{'id':"+ $riga['id'] + ", 'isFree:'+ $riga['is_free']+'}");
		}
		return $json;
}
?>