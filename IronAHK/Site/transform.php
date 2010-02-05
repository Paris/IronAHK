<?php

define('THIS_DIR', dirname(__FILE__));
define('MAKEFILE', THIS_DIR . '/../../Makefile');

if (!file_exists(MAKEFILE))
	exit('Makefile does not exist');
	
$makefile = file_get_contents(MAKEFILE);
preg_match('/^name=(.+)/m', $makefile, $out);
define('NAME', trim($out[1]));
preg_match('/^libname=(.+)/m', $makefile, $out);
define('RUSTY', NAME . '.' . trim($out[1]));
preg_match('/^config=(.+)/m', $makefile, $out);
define('CONFIG', trim($out[1]));

define('BUILD_DIR', realpath(THIS_DIR . '/../bin/' . CONFIG));

define('SOURCE_XML_1', realpath(BUILD_DIR . '/' . RUSTY . '.xml'));
define('SOURCE_XML_2', realpath(BUILD_DIR . '/../' . RUSTY . '.xml'));
define('SOURCE_XML', file_exists(SOURCE_XML_1) ? SOURCE_XML_1 : SOURCE_XML_2);

define('OUTPUT_DIR', realpath(THIS_DIR . '/docs/commands'));
define('SOURCE_XSL', realpath(OUTPUT_DIR . '/view.xsl'));
define('RUSTY_PREFIX', RUSTY . '.Core');
define('TAG_HEAD', 'doc');
define('TAG_PARENT', 'members');

header('Content-Type: text/plain');

if (!class_exists('XMLReader'))
	exit('PHP not configured with XMLReader.');
if (!class_exists('DOMDocument'))
	exit('PHP not configured with DOMDocument.');
if (!class_exists('XSLTProcessor'))
	exit('PHP not configured with XSLTProcessor, enable extension php_xsl.');

if (!file_exists(SOURCE_XML))
	exit('Source XML does not exist.');
if (!file_exists(SOURCE_XSL))
	exit('Source XSL does not exist.');

$xml = new XMLReader;
if (!$xml->open(SOURCE_XML))
	exit('Could not load source XML.');

$i = 0;
while ($i < 2)
{
	if (!$xml->next())
		exit('Error reading source XML.');
	
	if ($xml->nodeType == XMLReader::SIGNIFICANT_WHITESPACE)
		continue;
	
	$name = $xml->name;
	
	if ($i == 0 and $name == TAG_HEAD)
	{
		$xml->read();
		$i++;
	}
	else if ($i == 1 and $name == TAG_PARENT)
		$i++;
}
unset($i);

$xsl = new DOMDocument;
if (!$xsl->load(SOURCE_XSL))
	exit('Could not load source XSL.');
$xslt = new XSLTProcessor;
$xslt->importStyleSheet($xsl); 

$tidy = function_exists('tidy_parse_string');
if ($tidy)
	$config = array('output-xhtml' => true, 'indent' => 'auto', 'wrap' => 0, 'output-encoding' => 'utf8', 'tidy-mark' => false);

while ($xml->read())
{
	if ($xml->nodeType == XMLReader::SIGNIFICANT_WHITESPACE)
		continue;
	else if ($xml->name == TAG_PARENT)
		break;
	
	$src = $xml->readOuterXML();
	preg_match('/\Q<member name="M:' . RUSTY_PREFIX . '.\E([\w_]+?)\(/', $src, $name);
	if (count($name) != 2 or empty($name[0]))
		continue;
	$name = $name[1];
	
	$src = "<?xml version=\"1.0\"?>\n<?xml-stylesheet type=\"text/xsl\" href=\"../" . basename(SOURCE_XSL) . "\"?>\n<" .
		TAG_HEAD . ">\n    <assembly>\n        <name>" . RUSTY . "</name>\n    </assembly>\n    <" .
		TAG_PARENT . ">\n        " . $src . "\n    </" . TAG_PARENT . ">\n</" . TAG_HEAD . ">\n";
	
	$node = new DOMDocument;
	$node->loadXML($src);
	$html = $xslt->transformToXML($node);
	
	if ($tidy)
	{
		$tidy = tidy_parse_string($html, $config);
		tidy_clean_repair($tidy);
		$html = $tidy;
		$tidy = true;
	}
	
	$dir = OUTPUT_DIR . '/' . strtolower($name);
	$output = array('html' => $html, 'xml' => $src);
	if (!is_dir($dir))
		if (!@mkdir($dir))
			exit("Could not create directory \"$dir\".");
	
	foreach ($output as $ext => $content)
	{
		$path = $dir . '/index.' . $ext;
		$file = fopen($path, 'w');
		if ($file === false)
			exit('Could not export to "' . $path . '".');
		fwrite($file, $content);
		fclose($file);
	}
	
	$xml->next();
	echo("Exported $name.\n");
}

$xml->close();

echo($tidy ? "Tidied HTML.\n" : "Could not tidy HTML.\n");
echo("Done.\n");

?>
