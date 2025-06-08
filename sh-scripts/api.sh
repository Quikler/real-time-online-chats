#!/bin/bash
trap 'kill $(jobs -p)' EXIT

make db &
#docker-compose -f real-time-online-chats.Api/src/WebAPI/psql-compose.yaml up --build &
dotnet run --project ./real-time-online-chats.Api/src/WebAPI/WebAPI.csproj &

wait
