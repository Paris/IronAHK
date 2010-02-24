window.onload = function ()
{
	if (window.location.protocol === 'file:')
	{
		var a = document.getElementsByTagName('a');
		for (var i = 0; i < a.length; i++)
		{
			if (a[i].href.substr(a[i].href.length - 1, 1) === '/')
				a[i].href += 'index.html';
		}
	}
};
