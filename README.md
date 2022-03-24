# Anime Plex 🎬
Scarica le serie tv/film dal sito [AnimeSaturn](https://www.animesaturn.it/) e mette nella cartella Plex.

## La struttura del progetto
Il progetto si suddivide in 4 progetti:
- 🧮Api Server (C#)
- 💾Update Service (C#)
- 📩Download Server (C#)
- 🌐Web ([Nuxtjs](https://nuxtjs.org/))

Servizi utilizzati:
- 🐰[RabbitMQ](https://www.rabbitmq.com/)
- 🗄[Postgresql](https://www.postgresql.org/)

## 🌐Web Client
Questo progetto verrà utilizzato per gli utenti che vorranno visualizzare e scaricare gli episodi.

## 🧮Api Server
Questo progetto verrà utilizzato per esporre i dati in maniera facile e veloce con il database postgresql.

## 💾Update Service
Questo progetto verrà utilizzato per dare il via a scaricare gli episodi desiderati dagli utenti, nel frattempo può anche scaricare nuovi episodi appena usciranno.

## 📩Download Server
Questo progetto verrà utilizzato per scaricare i video e mettere nella cartella.