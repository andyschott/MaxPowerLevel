#!/bin/sh

docker run -p 5001:443 --env-file environment.txt --rm -v ~/.aspnet/https:/https/ maxpowerlevel