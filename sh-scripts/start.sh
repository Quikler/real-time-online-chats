#!/bin/bash
trap 'kill $(jobs -p)' EXIT

make api &
make client &

wait
