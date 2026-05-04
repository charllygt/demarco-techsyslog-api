.PHONY: dev up tools full down logs test build restore

restore:
	dotnet restore

dev:
	docker compose up -d mongo
	dotnet run --project src/TechsysLog.Api

up:
	docker compose up -d mongo

tools:
	docker compose --profile tools up -d

full:
	docker compose --profile tools --profile full up -d --build

down:
	docker compose --profile tools --profile full down

logs:
	docker compose logs -f api

test:
	dotnet test

build:
	dotnet build --configuration Release
