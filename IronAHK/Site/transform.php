<?php

define('NAME', 'IronAHK');
define('RUSTY_NAME', 'Rusty');
define('RUSTY', NAME . '.' . RUSTY_NAME);
define('THIS_DIR', dirname(__FILE__));

define('BUILD_DIR', realpath(THIS_DIR . '/../../' . NAME . '/bin'));
define('SOURCE_XML', realpath(THIS_DIR . '/../../' . RUSTY_NAME . '/bin/' . RUSTY . '.xml'));

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
