#!/bin/bash
trap 'kill $(jobs -p)' EXIT

make db &
make redis &
dotnet run --project ./real-time-online-chats.Api/src/WebAPI/WebAPI.csproj &

wait
