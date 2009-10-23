CC=mdtool
name=IronAHK
config=Release

outdir=bin
setup=setup.sh
version=$(cat version.txt)

.PHONY=all install uninstall dist clean

all: clean
	$(CC) build "--configuration:$(config)" "$(name).sln"

install: all
	(cd "$(outdir)/$(config)"; "./$(setup)" install)

uninstall:
	(cd "$(outdir)/$(config)"; "./$(setup)" uninstall)

dist:

clean:
	for dir in $(shell ls -d */ | xargs -l basename); do \
		for sub in "$(outdir)" obj; do \
			if [ -d "$${dir}/$${sub}" ]; then rm -R "$${dir}/$${sub}"; fi \
		done; \
	done;
