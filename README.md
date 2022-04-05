[![Docker Image CI Main](https://github.com/GamingBunker/AnimePlex/actions/workflows/docker-image.yml/badge.svg?branch=main)](https://github.com/GamingBunker/AnimePlex/actions/workflows/docker-image.yml) 

# Anime Plex 🎬
Scarica le serie tv/film dal sito [AnimeSaturn](https://www.animesaturn.it/) e mette nella cartella Plex.

## La struttura del progetto
Il progetto si suddivide in 4 progetti:
- 🧮Api Server (C#)
- 📩Download Service (C#)
- 📨Notify Service (C#)
- 💾Update Service (C#)
- 💽Upgrade Service (C#)
- 🌐Web Server([Nuxtjs](https://nuxtjs.org/))

Servizi utilizzati:
- 🐰[RabbitMQ](https://www.rabbitmq.com/)
- 🗄[Postgresql](https://www.postgresql.org/)

### Lista di immagini disponibili su DockerHub
| Nome Immagine | Link |
| ------ | ------ |
| 🧮Api Server | [Link](https://hub.docker.com/r/kju7pwd2/animeplex-api) |
| 📩Download Service | [Link](https://hub.docker.com/r/kju7pwd2/animeplex-downloadservice) |
| 📨Notify Service | [Link](https://hub.docker.com/r/kju7pwd2/animeplex-notifyservice) |
| 💾Update Service | [Link](https://hub.docker.com/r/kju7pwd2/animeplex-updateservice) |
| 💽Upgrade Service | [Link](https://hub.docker.com/r/kju7pwd2/animeplex-upgradeservice) |
| 🌐Web Client | [Link](https://hub.docker.com/r/kju7pwd2/animeplex-web) |

## 🌐Web Server
Questo progetto verrà utilizzato per gli utenti che vorranno visualizzare e scaricare gli episodi.
### Variabili globali richiesti:
```sh
example:
    #--- API ---
    HOST_API: "localhost"
    PORT_API : "33333"
    PROTOCOL_API: "https" or "http"
```

## 🧮Api Server
Questo progetto verrà utilizzato per esporre i dati in maniera facile e veloce con il database postgresql.
### Information general:
- `not` require volume mounted on Docker
### Variabili globali richiesti:
```sh
example:
    #--- DB ---
    DATABASE_CONNECTION: User ID=guest;Password=guest;Host=localhost;Port=33333;Database=db;
    
    #--- Logger ---
    LOG_LEVEL: "Debug|Info|Error"
    
    #--- General ---
    ASPNETCORE_ENVIRONMENT: Development
    BASE_PATH: "/folder/anime" or "D:\\\\Directory\Anime" not require volume mounted
```

## 💾Update Service
Questo progetto verrà utilizzato per controllare se sono presenti nel file locale se non ci sono, invia un messaggio a DownloadService che scarica l'episodio mancante.
### Information general:
- `require` volume mounted on Docker
### Variabili globali richiesti:
```sh
example:
    #--- Rabbit ---
    USERNAME_RABBIT: "guest"
    PASSWORD_RABBIT: "guest"
    ADDRESS_RABBIT: "localhost"
    
    #--- API ---
    ADDRESS_API: "localhost"
    PORT_API: "33333"
    PROTOCOL_API: "http" or "https"
    
    #--- Logger ---
    LOG_LEVEL: "Debug|Info|Error"
    
    #--- General ---
    DOTNET_ENVIRONMENT: Development
    BASE_PATH: "/folder/anime" or "D:\\\\Directory\Anime"
    TIME_REFRESH: "60000" <-- milliseconds
```

## 💽Upgrade Service
Questo progetto verrà utilizzato per scaricare i nuovi episodi
### Information general:
- `not` require volume mounted on Docker
### Variabili globali richiesti:
```sh
example:
    #--- rabbit ---
    USERNAME_RABBIT: "guest"
    PASSWORD_RABBIT: "guest"
    ADDRESS_RABBIT: "localhost"
    
    #--- API ---
    ADDRESS_API: "localhost"
    PORT_API: "33333"
    PROTOCOL_API: "http" or "https"
    
    #--- Logger ---
    LOG_LEVEL: "Debug|Info|Error"
    
    #--- General ---
    DOTNET_ENVIRONMENT: Development
    BASE_PATH: "/folder/anime" or "D:\\\\Directory\Anime" not require volume mounted
    TIME_REFRESH: "60000" <-- milliseconds
```

## 📩Download Service
Questo progetto verrà utilizzato per scaricare i video e mettere nella cartella.
### Information general:
- `not` require volume mounted on Docker
### Variabili globali richiesti:
```sh
example:
    #--- rabbit ---
    USERNAME_RABBIT: "guest"
    PASSWORD_RABBIT: "guest"
    ADDRESS_RABBIT: "localhost"
    LIMIT_CONSUMER_RABBIT: "5"
    
    #--- API ---
    ADDRESS_API: "localhost"
    PORT_API: "33333"
    PROTOCOL_API: "http" or "https"
    
    #--- Logger ---
    LOG_LEVEL: "Debug|Info|Error"
    
    #--- General ---
    DOTNET_ENVIRONMENT: Development
```

## 📨Notify Service
### Information general:
- `not` require volume mounted on Docker
```sh
example:
    #--- rabbit ---
    USERNAME_RABBIT: "guest"
    PASSWORD_RABBIT: "guest"
    ADDRESS_RABBIT: "localhost"
    
    #---Webhook---
    WEBHOOK_DISCORD: "url"
    
    #---logger---
    LOG_LEVEL: "Debug|Info|Error"
    
    #---general---
    DOTNET_ENVIRONMENT: Development
```