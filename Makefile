api:
	./sh-scripts/api.sh

client:
	cd ./real-time-online-chats.Client/ && npm run dev && cd

db:
	docker-compose -f ./docker-files/psql-compose.yaml up --build

db-down:
	docker-compose -f ./docker-files/psql-compose.yaml down

start:
	./sh-scripts/start.sh

redis:
	docker-compose -f ./docker-files/redis-compose.yaml up --build

redis-down:
	docker-compose -f ./docker-files/redis-compose.yaml down
