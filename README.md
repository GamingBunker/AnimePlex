[![Docker Image CI Main](https://github.com/GamingBunker/AnimePlex/actions/workflows/docker-image.yml/badge.svg?branch=main)](https://github.com/GamingBunker/AnimePlex/actions/workflows/docker-image.yml) 

# Anime Plex 🎬
Scarica le serie tv/film dal sito [AnimeSaturn](https://www.animesaturn.it/) e mette nella cartella Plex.

## La struttura del progetto
Il progetto si suddivide in 4 progetti:
- 🧮Api Server (C#)
- 💾Update Service (C#)
- 💽Upgrade Service (C#)
- 📩Download Service (C#)
- 📨Notify Service (C#)
- 🌐Web Server([Nuxtjs](https://nuxtjs.org/))

Servizi utilizzati:
- 🐰[RabbitMQ](https://www.rabbitmq.com/)
- 🗄[Postgresql](https://www.postgresql.org/)

### Lista di immagini disponibili su DockerHub
| Nome Immagine | Link |
| ------ | ------ |
| 🌐Web Client | [Link](https://hub.docker.com/layers/199777729/kju7pwd2/animeplex-web/main/images/sha256-99790d8b0949afe78eb300799090b4dcb868580f5d018981176a7ba6e4318090?context=repo) |
| 🧮Api Server | [Link](https://hub.docker.com/layers/199777863/kju7pwd2/animeplex-api/main/images/sha256-086b60d487e7b31d0e2a95430496ebf744789d71dc4c7a297300ce3b39b36977?context=repo) |
| 💾Update Service | [Link](https://hub.docker.com/layers/199778049/kju7pwd2/animeplex-updateservice/main/images/sha256-79547385aedc3497e830d4955face899f6b761b0f770f4d77cfca0aa341803a2?context=repo) |
| 💽Upgrade Service | [Link](https://hub.docker.com/layers/200921820/kju7pwd2/animeplex-upgradeservice/main/images/sha256-3189cfabe612ec0d442963d0081ff41f12ee506f0bc4c44bce7aa4a106ed7e77?context=repo) |
| 📩Download Service | [Link](https://hub.docker.com/layers/199777944/kju7pwd2/animeplex-downloadservice/main/images/sha256-c712e8701865bda929ae2cc374a7e3abd788741fc0430053df1fcef9daa83c83?context=repo) |
| 📨Notify Service | [Link]() |

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