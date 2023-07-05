<?php
	$action = isset($_REQUEST['action']) ? $_REQUEST['action'] : '';
	$return_var = "0";
	$output = array();
	$error = array();

			
	switch ($action) {
		case 'stop_spooler':
			// Stop the spooler service
			$stopSpoolerCommand = 'net stop spooler';
			$return_var = "0";
			$output = array();
			$error = array();
			exec($stopSpoolerCommand . ' 2>&1', $output, $error);
			print_r($output);
			break;
		case 'delete_queue':
			// Delete the print queue
			$deleteQueueCommand = 'del /Q /F %SystemRoot%\System32\spool\PRINTERS\*';
			exec($deleteQueueCommand . ' 2>&1', $output, $error);
			print_r($output);
			break;
		case 'start_spooler':
			// Start the spooler service
			$startSpoolerCommand = 'net start spooler';
			exec($startSpoolerCommand . ' 2>&1', $output, $error);
			print_r($output);
			break;
		default:
			echo 'Invalid action.';
			exit();
	}

	echo 'Success';
?>