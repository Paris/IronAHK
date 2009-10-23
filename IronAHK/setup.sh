#!/bin/sh

if [ -z "$prefix" ]; then prefix=/usr; fi
libdir=lib
bindir=bin

main=IronAHK
outdir=$prefix/$libdir/$main
stub=$prefix/$bindir/$main
clone=$(dirname "$stub")/iak

help() {
	echo "$main setup script"
	echo "Usage: $0 {install|uninstall|help}"
	echo
	echo "Options:"
	echo "	install: saves binaries under \"$prefix\""
	echo "	uninstall: removes files added by installation"
	echo "	help: shows this information"
	echo
	echo "Note: you can change the target directory by exporting the environment variable 'prefix'."
}

case $1 in
	install)
		if [ ! -d "$outdir" ]; then install -d "$outdir"; fi
		install *.dll "$outdir"
		install *.exe "$outdir"
		echo "#!/bin/sh" > "$stub"
		echo "exec mono \"$outdir/$main.exe\" \"$@\"" >> "$stub"
		chmod 755 "$stub"
		ln -s "$stub" "$clone"
		echo "Installation complete"
		;;
	uninstall | remove)
		rm -f "$stub"
		rm -f "$clone"
		rm -fr "$outdir"
		echo "Remove complete"
		;;
	help | --help)
		help
		;;
	*)
		if [ -n "$1" ]; then
			echo "Unrecognised option \"$1\""
			echo
		fi
		help
		;;
esac
