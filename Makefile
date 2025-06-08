api:
	./sh-scripts/api.sh
	#dotnet run --project ./real-time-online-chats.Api/src/WebAPI/WebAPI.csproj

client:
	cd ./real-time-online-chats.Client/ && npm run dev && cd

db:
	docker-compose -f real-time-online-chats.Api/src/WebAPI/psql-compose.yaml up --build

start:
	./sh-scripts/start.sh
