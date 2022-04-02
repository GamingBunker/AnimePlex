#### MAIN: [![Docker Image CI Main](https://github.com/GamingBunker/AnimePlex/actions/workflows/docker-image.yml/badge.svg?branch=main)](https://github.com/GamingBunker/AnimePlex/actions/workflows/docker-image.yml) 

#### DEV: [![Docker Image CI Dev](https://github.com/GamingBunker/AnimePlex/actions/workflows/docker-image.yml/badge.svg?branch=dev)](https://github.com/GamingBunker/AnimePlex/actions/workflows/docker-image.yml)

# Anime Plex 🎬
Scarica le serie tv/film dal sito [AnimeSaturn](https://www.animesaturn.it/) e mette nella cartella Plex.

## La struttura del progetto
Il progetto si suddivide in 4 progetti:
- 🧮Api Server (C#)
- 💾Update Service (C#)
- 💽Upgrade Service (C#)
- 📩Download Server (C#)
- 🌐Web Server([Nuxtjs](https://nuxtjs.org/))

Servizi utilizzati:
- 🐰[RabbitMQ](https://www.rabbitmq.com/)
- 🗄[Postgresql](https://www.postgresql.org/)

## 🌐Web Client
Questo progetto verrà utilizzato per gli utenti che vorranno visualizzare e scaricare gli episodi.
### Variabili globali richiesti:
```sh
example:
    HOST_API: "localhost"
    PORT_API : "33333"
    PROTOCOL_API: "https" or "http"
```

## 🧮Api Server
Questo progetto verrà utilizzato per esporre i dati in maniera facile e veloce con il database postgresql.
### Variabili globali richiesti:
```sh
example:
    ASPNETCORE_ENVIRONMENT: Development
    DATABASE_CONNECTION: User ID=guest;Password=guest;Host=localhost;Port=33333;Database=db;
    BASE_PATH: "/folder/anime" or "D:\\\\Directory\Anime"
```
## 💾Update Service
Questo progetto verrà utilizzato per controllare se sono presenti nel file locale se non ci sono, invia un messaggio a DownloadService che scarica l'episodio mancante.
### Variabili globali richiesti:
```sh
example:
    DOTNET_ENVIRONMENT: Development
    USERNAME_RABBIT: "guest"
    PASSWORD_RABBIT: "guest"
    ADDRESS_RABBIT: "localhost"
    BASE_PATH: "/folder/anime" or "D:\\\\Directory\Anime"
    ADDRESS_API: "localhost"
    PORT_API: "33333"
    PROTOCOL_API: "http" or "https"
    TIME_REFRESH: "60000" <-- milliseconds
```
## 💽Upgrade Service (C#)
Questo progetto verrà utilizzato per scaricare i nuovi episodi
### Variabili globali richiesti:
```sh
example:
    DOTNET_ENVIRONMENT: Development
    BASE_PATH: "/folder/anime" or "D:\\\\Directory\Anime"
    ADDRESS_API: "localhost"
    PORT_API: "33333"
    PROTOCOL_API: "http" or "https"
    TIME_REFRESH: "60000" <-- milliseconds
```
## 📩Download Server
Questo progetto verrà utilizzato per scaricare i video e mettere nella cartella.
### Variabili globali richiesti:
```sh
example:
    DOTNET_ENVIRONMENT: Development
    LIMIT_CONSUMER_RABBIT: "5"
    USERNAME_RABBIT: "guest"
    PASSWORD_RABBIT: "guest"
    ADDRESS_RABBIT: "localhost"
    ADDRESS_API: "localhost"
    PORT_API: "33333"
    PROTOCOL_API: "http" or "https"
```