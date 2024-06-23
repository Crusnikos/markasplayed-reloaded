## **Mark As Played... Reloaded is coming**

After a long break, I'm back with a new, old one **Mark As Played**, signed with a subtitle **Reloaded**. What can you expect:

- better backend code
- new UI
- more activity

Stay tuned and observe https://www.markasplayed.com/

Old repository is here https://github.com/Crusnikos/markasplayed

## Table of contents

- [General info](#general-info)
- [Technologies](#technologies)
- [Setup](#setup)
- [API Tests Setup](#api-tests-setup)

## General info

- Web application - Blog - Portfolio - A small and friendly place created to share the passion with geeks about games, music, movies, comics, programming and much more. Currently created and edited by one person, however, cooperation with other enthusiasts is not excluded.

## Technologies

- .NET 8.0
- PostgreSQL 16.3-alpine
- React 18.3.1 / TypeScript 4.9.5
- Docker ^4.31.1
- Authentication supported by Firebase Authentication backend.

## Setup

Prerequisites:

```
1.Create Firebase Authentication. More You can read at official docs: https://firebase.google.com/docs/auth?hl=en
```

Before containers start:

```
1.Download and install Node.js (if you dont have)
2.In folder markasplayed/ui/ execute command npm ci
3.Download, install and configure docker (if you dont have)
4.I advice also use WSL2 and Ubuntu for containers launching
5.Create appsettings.json and appsettings.Test.json file in MarkAsPlayed.Api folder and configure as shown in templates
6.Create .env file in MarkAsPlayed folder and configure as shown in template
```

Process launching containers in terminal:

```
1.docker compose build
2.docker compose up -d
3.docker compose exec api bash
4.dotnet run --project MarkAsPlayed.Api/MarkAsPlayed.Api.csproj
5.docker compose exec ui bash
6.npm start
```

Application should be available under adress: http://Localhost:3000

## API Tests Setup

Process launching containers in terminal:

```
1.docker compose build
2.docker compose up -d
3.docker compose exec api bash
4.dotnet test MarkAsPlayed.sln
```
