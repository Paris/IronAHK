<?php

define('OUTPUT_DIR', realpath(dirname(__FILE__) . '/docs/commands'));

header('Content-Type: text/plain');

if (!is_dir(OUTPUT_DIR))
	exit('Output directory does not exist.');

$dir = @opendir(OUTPUT_DIR);
if ($dir === false)
	exit('Could not open output directory.');

chdir(OUTPUT_DIR);

while (($subdir = readdir($dir)) !== false)
{
	if ($subdir == '.' or $subdir == '..' or !is_dir($subdir))
		continue;
	@unlink("$subdir/index.html");
	@unlink("$subdir/index.xml");
	@rmdir($subdir);
	echo(is_dir($subdir) ? 'Could not remove ' : 'Removed ');
	echo("$subdir.\n");
}

closedir($dir);

echo("Done.\n");

?>
