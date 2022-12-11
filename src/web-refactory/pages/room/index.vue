<template>
  <div
      id="main"
      class="fill-height"
      style="width: 100%"
  >
    <div
        class="d-flex flex-column align-center"
    >
      <v-btn
          style="color: white !important;"
          color="info"
          @click="close()"
      >
        Go to Home
      </v-btn>
      <div
          v-if="type === 'anime'"
          class="ma-2"
      >
        <video id="my-video" controls>
          <template v-if="episode != null && data != null">
            <source :src="getUrl(data.episodePath)" type="video/mp4">
          </template>
        </video>
      </div>
      <div
        v-if="type === 'manga' && !checkNull(data)"
        class="text-center d-flex flex-column"
      >
        <template v-for="(chapter, index) in data.chapterPath" :key="chapter">
          <img
              :src="getUrl(chapter)"
              class="img-page"
          >
        </template>
        <v-btn
            style="color: white !important;"
            color="info"
            @click="close()"
        >
          Go to Home
        </v-btn>
      </div>
      <div
          class="ma-2"
      >
        <span v-if="!checkNull(idRoom)" class="link-share">
          <v-icon
              class="mr-3"
          >
            $share
          </v-icon>
          {{ `${hostWeb}/room?idroom=${idRoom}` }}
        </span>
      </div>

      <div class="d-flex justify-center" style="width: 100%">
        <div v-for="(user, index) in users" class="ma-3">
          <userSession :nickname="user.nickname" :root="index === 0" :currentUser="currentUser"/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import pkg from 'websocket';

const {w3cwebsocket} = pkg;
import axios from "axios";

import {useRuntimeConfig} from "nuxt/app";

import lodash from "../../mixins/lodash";

import userSession from "../../component/userSession";
import mitt from 'mitt';
export default {
  name: 'room',
  components: {
    userSession
  },
  mixins: [
    lodash
  ],
  data() {
    const runtimeConfig = useRuntimeConfig();

    return {
      data: null,
      hide: "",

      hostSocket: runtimeConfig.public.socketBase,
      hostHTTP: runtimeConfig.public.httpBase,
      hostWeb: runtimeConfig.public.webBase,
      basePath: runtimeConfig.public.basePath,

      ws: null,

      idRoom: null,
      root: null,
      currentUser: null,
      users: null,
      episode: null,
      pause: null,
      time: 0,
      room:null,
      type:null,
    }
  },
  created() {
    this.type = this.$route.query.type;
  },
  mounted() {
    if(this.type === 'manga')
    {
      this.load();
    }else if(this.type === 'anime')
    {
      this.room = mitt();

      console.log("Starting connection to WebSocket Server");
      this.ws = new w3cwebsocket(this.hostSocket);

      const _self = this;
      //sockets
      this.ws.onopen = function () {
        console.log("Successfully connected to the echo WebSocket Server");

        console.log(_self.ws)

        _self.room.emit('checkRoom');

        _self.load();
      };

      this.ws.onmessage = function (event) {
        console.log(event);
        _self.room.emit('socket-message', event);
      }

      this.room.on('socket-message', (event) => {
        console.log("Un messaggio arrivato: ");
        console.log(event.data)

        var data = JSON.parse(event.data)
        if (data.action === 'registration') {
          this.root = data.nickname
          this.getVideoEpisode()

        } else if (data.action === 'UpdateRoom') {
          this.idRoom = data.room.id_room
          this.users = data.room.clients
          this.time = data.room.t
          this.pause = data.room.pause

        } else if (data.action === 'loadVideo') {
          this.idRoom = data.id
          this.root = data.nickname
          this.episode = data.episode

        } else if (data.action === 'currentTime') {
          this.room.emit('currentTime')
        }
      });

      this.room.on('checkRoom', () => {
        console.log('check')
        console.log(_self.ws)
        if (this.$route.query.idroom == null)
          this.room.emit('createRoom');
        else
          this.room.emit('joinRoom');
      });

      this.room.on('createRoom', () => {
        this.currentUser = new Date().getUTCMilliseconds().toString();

        const data = {nickname: this.currentUser, episode: this.$route.query.episode};

        this.ws.send(JSON.stringify({action: 'create', data}))
        console.log('request registration');
      });

      this.room.on('joinRoom', () => {
        this.currentUser = new Date().getUTCMilliseconds().toString();
        this.sendMessage('join', {nickname: this.currentUser, idRoom: this.$route.query.idroom})
        console.log('request join');
      });

      this.room.on('pause', (statePause) => {
        this.sendMessage('updatePause', {pause: statePause, idRoom: this.idRoom})
        console.log('request updatePause');
      });

      this.room.on('time', (stateTime) => {
        this.sendMessage('updateTime', {time: stateTime, idRoom: this.idRoom})
        console.log('request updatePause');
      });

      this.room.on('currentTime', () => {
        var vid = document.getElementById("my-video");

        this.sendMessage('currentTime', {time: vid.currentTime, idRoom: this.idRoom})
        console.log('request currentTime');
      });

      //event
      var vid = document.getElementById("my-video");

      vid.addEventListener("canplaythrough", () => {

        console.log('canplaythrough');
        var vid = document.getElementById("my-video");

        if (vid.currentTime > 0)
          _self.room.emit('time', vid.currentTime);
      });
      vid.addEventListener("pause", () => {
        _self.room.emit('pause', true);
      });
      vid.addEventListener("playing", () => {
        _self.room.emit('pause', false);
      });
    }
  },
  methods: {
    sendMessage(setAction, data) {
      console.log(this.ws);
      this.ws.send(JSON.stringify({action: setAction, data}))
    },
    load() {
      if (this.type === "anime" && this.$route.query.episode != null && this.data == null) {
        this.episode = this.$route.query.episode;
      } else if (this.type === "manga" && this.data == null) {
        axios.get(`/api/manga/register?id=${this.$route.query.chapter}`)
            .then(rs => {
              this.data = rs.data
            });
      }
    },
    getVideoEpisode() {
      //get api internal
      axios.get(`/api/anime/register?id=${this.episode}`)
          .then(rs => {
            this.data = rs.data
          });
    },
    getUrl(url) {

      if (url.includes(':')) {
        url = url.replace(/\\/g, '\\\\');
      }

      console.log(url);
      console.log(this.basePath);

      url = url.replace(this.basePath, '')

      console.log(url);
      console.log(`${this.hostHTTP}/${url}`);

      return `${this.hostHTTP}/${url}`;
    },
    close(){
      if(this.type === 'anime')
      {
        this.ws.close();
        console.log('Closed WebSocket')

        this.ws = null;
      }
      this.$router.push('/')
    }
  },
  watch: {
    episode() {
      this.getVideoEpisode(this.episode);
    },
    time() {
      var vid = document.getElementById("my-video");
      vid.currentTime = this.time
    },
    pause() {
      var vid = document.getElementById("my-video");
      console.log(vid)
      console.log('CAMBIO STATO');
      if (this.pause === true) {
        vid.pause();
      } else if (this.pause === false) {
        try {
          vid.play();
        } catch {
          vid.muted = true;
          vid.play();
        }
      }
    }
  }
}
</script>

<style scoped>
.link-share {
  background-color: white;
  padding: 10px;
  border-radius: 5px;
}

#my-video {
  width: 100%;
  height: auto;
}
.img-page{
  width: 100%;
  max-width: 800px;
  height: auto;
}
</style>