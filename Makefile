CC=mdtool
PHP=php-cgi
CSC=gmcs
name=IronAHK
libname=Rusty
config=Release

outdir=bin
setup=setup.sh
site=Site

.PHONY=all docs install uninstall clean

all: clean
	$(CC) build "--configuration:$(config)" "$(name).sln"

docs:
	if [ ! -d "$(name)/$(outdir)/$(config)" ]; then mkdir -p "$(name)/$(outdir)/$(config)"; fi
	$(CSC) "-doc:$(name)/$(outdir)/$(config)/$(name).$(libname).xml" "-out:$(name)/$(outdir)/$(config)/$(name).$(libname).dll" -reference:System.Windows.Forms,System.Drawing "-recurse:$(libname)/*.cs" -target:library -unsafe
	$(PHP) -f "$(name)/$(site)/transform.php"

install: all
	(cd "$(outdir)/$(config)"; "./$(setup)" install)

uninstall:
	(cd "$(outdir)/$(config)"; "./$(setup)" uninstall)

clean:
	for dir in $(shell ls -d */ | xargs -l basename); do \
		for sub in "$(outdir)" obj; do \
			if [ -d "$${dir}/$${sub}" ]; then rm -R "$${dir}/$${sub}"; fi \
		done; \
	done;
