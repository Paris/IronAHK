CC=mdtool
MONO=mono
CSC=gmcs

name=IronAHK
config=Release

outdir=bin
deploy=Deploy/$(outdir)/$(config)/Setup.exe
setup=setup.sh
working=$(name)/$(outdir)/$(config)

.PHONY=all docs dist install uninstall clean

all: clean
	$(CC) build "--configuration:$(config)" "$(name).sln"

docs: all
	$(MONO) $(deploy) docs

dist: all
	$(MONO) $(deploy)

install: all
	(cd "$(working)"; "./$(setup)" install)

uninstall:
	(cd "$(working)"; "./$(setup)" uninstall)

clean:
	for dir in $(shell ls -d */ | xargs -l basename); do \
		for sub in "$(outdir)" obj; do \
			if [ -d "$${dir}/$${sub}" ]; then rm -R "$${dir}/$${sub}"; fi \
		done; \
	done;
	if [ -d "$(outdir)" ]; then rm -R "$(outdir)"; fi
