[![Docker Image CI Main](https://github.com/GamingBunker/AnimePlex/actions/workflows/docker-image.yml/badge.svg?branch=main)](https://github.com/GamingBunker/AnimePlex/actions/workflows/docker-image.yml) 

# Anime Plex 🎬
Scarica le serie tv/film dal sito [AnimeSaturn](https://www.animesaturn.it/) e mette nella cartella Plex.

## La struttura del progetto
Il progetto si suddivide in 9 progetti:
- 🧮Api Service (C#)
- 📩Download Service (C#)
- 📨Notify Service (C#)
- 💾Update Service (C#)
- 💽Upgrade Service (C#)
- 💱Conversion Service (C#)
- 🏠Room server (Hapi)
- 📁Path server (Nodejs)
- 🌐Web Server([Nuxtjs](https://nuxtjs.org/))

Servizi utilizzati:
- 🐰[RabbitMQ](https://www.rabbitmq.com/)
- 🗄[Postgresql](https://www.postgresql.org/)

### Lista di immagini disponibili su DockerHub
| Nome Immagine | Link |
| ------ | ------ |
| 🧮Api Service | [Link](https://hub.docker.com/r/kju7pwd2/animeplex-api) |
| 📩Download Service | [Link](https://hub.docker.com/r/kju7pwd2/animeplex-downloadservice) |
| 📨Notify Service | [Link](https://hub.docker.com/r/kju7pwd2/animeplex-notifyservice) |
| 💾Update Service | [Link](https://hub.docker.com/r/kju7pwd2/animeplex-updateservice) |
| 💽Upgrade Service | [Link](https://hub.docker.com/r/kju7pwd2/animeplex-upgradeservice) |
| 💱Conversion Service | [Link](https://hub.docker.com/r/kju7pwd2/animeplex-conversionservice) |
| 🏠Room server (Hapi) | [Link](https://hub.docker.com/r/kju7pwd2/animeplex-roomserver) |
| 🌐Web Client | [Link](https://hub.docker.com/r/kju7pwd2/animeplex-web) |

## 🌐Web Server
Questo progetto verrà utilizzato per gli utenti che vorranno visualizzare e scaricare gli episodi.

### Expose Ports:
- 3000 tcp

### Variabili globali richiesti:
```sh
example:
    #--- API ---
    HOST_API: "localhost" #localhost [default]
    PORT_API : "33333" #5000 [default]
    PROTOCOL_API: "https" or "http" #http [default]
    
    #--- WebSocket ---
    HOST_WS: "ws://localhost:1111/path" #ws://localhost:1234/room [default]
    
    #--- Path ---
    BASE_PATH: "/path" #"/" [default]
    HOST_HTTP_SERVER: "localhost" #localhost [default]
    PORT_HTTP_SERVER: "33333" #8080 [default]
    
    #--- Share link ---
    SHARE_ROOM: "localhost:33333" #localhost:3000 [default]
```

## 🧮Api Service
Questo progetto verrà utilizzato per esporre i dati in maniera facile e veloce con il database postgresql.

### Expose Ports:
- 80 tcp

### Information general:
- `not` require volume mounted on Docker
### Variabili globali richiesti:
```sh
example:
    #--- DB ---
    DATABASE_CONNECTION: User ID=guest;Password=guest;Host=localhost;Port=33333;Database=db; [require]
    
    #--- Rabbit ---
    USERNAME_RABBIT: "guest" #guest [default]
    PASSWORD_RABBIT: "guest" #guest [default]
    ADDRESS_RABBIT: "localhost" #localhost [default]

    #--- API ---
    PORT_API: "33333" #5000 [default]
    
    #--- Logger ---
    LOG_LEVEL: "Debug|Info|Error" #Info [default]
    WEBHOOK_DISCORD_DEBUG: "url" [not require]
    
    #--- General ---
    BASE_PATH: "/folder/anime" or "D:\\\\Directory\Anime" #/ [default]
    LIMIT_THREAD_PARALLEL: "8" #5 [default]
```

## 💾Update Service
Questo progetto verrà utilizzato per controllare se sono presenti nel file locale se non ci sono, invia un messaggio a DownloadService che scarica l'episodio mancante.
### Information general:
- `require` volume mounted on Docker
### Variabili globali richiesti:
```sh
example:
    #--- Rabbit ---
    USERNAME_RABBIT: "guest" #guest [default]
    PASSWORD_RABBIT: "guest" #guest [default]
    ADDRESS_RABBIT: "localhost" #localhost [default]
    
    #--- API ---
    ADDRESS_API: "localhost" #localhost [default]
    PORT_API: "33333" #5000 [default]
    PROTOCOL_API: "http" or "https" #http [default]
    
    #--- Logger ---
    LOG_LEVEL: "Debug|Info|Error" #Info [default]
    WEBHOOK_DISCORD_DEBUG: "url" [not require]
    
    #--- General ---
    BASE_PATH: "/folder/anime" or "D:\\\\Directory\Anime" #/ [default]
    TIME_REFRESH: "60000" <-- milliseconds #120000 [default] 2 minutes
    LIMIT_THREAD_PARALLEL: "8" #5 [default]
    SELECT_SERVICE: "manga or anime" #anime
```

## 💽Upgrade Service
Questo progetto verrà utilizzato per scaricare i nuovi episodi
### Information general:
- `not` require volume mounted on Docker
### Variabili globali richiesti:
```sh
example:
    #--- rabbit ---
    USERNAME_RABBIT: "guest" #guest [default]
    PASSWORD_RABBIT: "guest" #guest [default]
    ADDRESS_RABBIT: "localhost" #localhost [default]
    
    #--- API ---
    ADDRESS_API: "localhost" #localhost [default]
    PORT_API: "33333" #5000 [default]
    PROTOCOL_API: "http" or "https" #http [default]
    
    #--- Logger ---
    LOG_LEVEL: "Debug|Info|Error" #Info [default]
    WEBHOOK_DISCORD_DEBUG: "url" [not require]
    
    #--- General ---
    BASE_PATH: "/folder/anime" or "D:\\\\Directory\Anime" #http [default]
    TIME_REFRESH: "60000" <-- milliseconds #1200000 [default] 20 minutes
    LIMIT_THREAD_PARALLEL: "8" #5 [default]
    SELECT_SERVICE: "manga or anime" #anime
```

## 📩Download Service
Questo progetto verrà utilizzato per scaricare i video e mettere nella cartella.
### Information general:
- `require` volume mounted on Docker
### Variabili globali richiesti:
```sh
example:
    #--- rabbit ---
    USERNAME_RABBIT: "guest" #guest [default]
    PASSWORD_RABBIT: "guest" #guest [default]
    ADDRESS_RABBIT: "localhost" #localhost [default]
    LIMIT_CONSUMER_RABBIT: "5" #3 [default]
    
    #--- API ---
    ADDRESS_API: "localhost" #localhost [default]
    PORT_API: "33333" #3000 [default]
    PROTOCOL_API: "http" or "https" #http [default]
    
    #--- Logger ---
    LOG_LEVEL: "Debug|Info|Error" #Info [default]
    WEBHOOK_DISCORD_DEBUG: "url" [not require]
    
    #--- General ---
    LIMIT_THREAD_PARALLEL: "500" #5 [default]
    PATH_TEMP: "/tmp/folder" #D:\\TestAnime\\temp [default]
    BASE_PATH: "/folder/anime" or "D:\\\\Directory\Anime" #/ [default]
```

## 📨Notify Service
### Information general:
- `not` require volume mounted on Docker
```sh
example:
    #--- rabbit ---
    USERNAME_RABBIT: "guest" #guest [default]
    PASSWORD_RABBIT: "guest" #guest [default]
    ADDRESS_RABBIT: "localhost" #localhost [default]

    #--- API ---
    ADDRESS_API: "localhost" #localhost [default]
    PORT_API: "33333" #5000 [default]
    PROTOCOL_API: "http" or "https" #http [default]
    
    #---Webhook---
    WEBHOOK_DISCORD: "url" [require]
    
    #---logger---
    LOG_LEVEL: "Debug|Info|Error" #Info [default]
    WEBHOOK_DISCORD_DEBUG: "url" [not require]
```

## 💱Conversion Service
Questo progetto verrà utilizzato per convertire file ts in mp4 da poter riprodurre in streaming
### Information general:
- `require` volume mounted on Docker
### Variabili globali richiesti:
```sh
example:
    #--- rabbit ---
    USERNAME_RABBIT: "guest" #guest [default]
    PASSWORD_RABBIT: "guest" #guest [default]
    ADDRESS_RABBIT: "localhost" #localhost [default]
    LIMIT_CONSUMER_RABBIT: "5" #3 [default]
    
    #--- API ---
    ADDRESS_API: "localhost" #localhost [default]
    PORT_API: "33333" #3000 [default]
    PROTOCOL_API: "http" or "https" #http [default]
    
    #--- Logger ---
    LOG_LEVEL: "Debug|Info|Error" #Info [default]
    WEBHOOK_DISCORD_DEBUG: "url" [not require]
    
    #--- General ---
    PATH_TEMP: "/folder/temp" [require]
    PATH_FFMPEG: "/folder/bin" #/usr/local/bin/ffmpeg [default]
    BASE_PATH: "/folder/anime" or "D:\\\\Directory\Anime" #/ [default]
```

## 🏠Room server (Hapi)
questo progetto viene gestito le sessioni di streaming e le interazioni dei video degli altri, per esempio se viene messo in pausa tutte le persone che sono presenti in quella stanza viene messo in pausa il video.
### Information general:
- `not` require volume mounted on Docker
### Variabili globali richiesti:
```sh
example:
    #--- General ---
    HOST: "localhost" #0.0.0.0 [default]
    PORT: "33333" #1234 [default]
    PATH_URL: "/path" #/room [default]
```
## 📁Path server (Nodejs)
Questo servizio serve ad esporre i file video per web.
### Information general:
Creare un container che contiene una lts di linux.
Installare `nodejs` e `npm`, infine installare il pacchetto ftp: `npm install --global http-server`
Avviare con `http-server '/root/anime'` come avvio della macchina

Oppure si può usare questa immagine: `danjellz/http-server`, la cartella che viene esposta è la seguente: `/public`