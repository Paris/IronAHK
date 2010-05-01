CC=xbuild
CLI=mono

name=IronAHK
config=Release

outdir=bin
deploy=Deploy/$(outdir)/$(config)
setup=$(deploy)/Setup.exe
installer=$(deploy)/setup.sh

.PHONY=all docs dist install uninstall clean

all: clean
	$(CC) "/property:Configuration=$(config)"

docs: all
	$(CLI) $(setup) docs

dist: all
	$(CLI) $(setup)

install: all
	(cd "$(deploy)"; "./$(installer)" install)

uninstall: all
	(cd "$(deploy)"; "./$(installer)" install)

clean:
	for dir in $(shell ls -d */ | xargs -l basename); do \
		for sub in "$(outdir)" obj; do \
			if [ -d "$${dir}/$${sub}" ]; then rm -R "$${dir}/$${sub}"; fi \
		done; \
	done;
	if [ -d "$(outdir)" ]; then rm -R "$(outdir)"; fi
