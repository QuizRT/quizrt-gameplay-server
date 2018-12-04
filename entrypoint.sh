#!/bin/bash

set -e
run_cmd="dotnet run --server.urls http://*:82"

# until dotnet ef database update; do
# >&2 echo "SQL Server is starting up"
# sleep 1
# done

>&2 echo "Executing command"
exec $run_cmd