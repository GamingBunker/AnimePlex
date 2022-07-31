<template>
    <div class="container d-flex flex-column justify-content-center" style="margin-top: 100px;">
        {{load()}}
        <video id="my-video" width="100%" controls>
            <template v-if="episode != null && data != null">
                <source :src="getUrl(data.episodePath)" type="video/mp4">
            </template>
        </video>

        <div class="d-flex justify-content-center" style="margin: 10px;">
            <div class="card" style="width: 110px;">
                <div style="margin-top: 25%; margin: auto;">
                    <a :href="'http://192.168.20.10:3000/room?idroom='+idRoom">Share room</a>
                </div>
            </div>
            <div v-for="user in users">
                <template v-if="user.nickname === currentUser">
                    <userSession :nickname="user.nickname" :current="true" />
                </template>
                <template v-else>
                    <userSession :nickname="user.nickname" :current="false" />
                </template>
            </div>
        </div>
    </div>
</template>

<script>
var WebSocket = require('websocket').w3cwebsocket;
var randomName = require('random-name')
import userComponentVue from '../components/userComponent.vue';
    export default {
        data(){
            return{
                data:null,
                hide:"",
                
                host: this.$config.ipAPI,
                port: this.$config.portAPI,
                protocol: this.$config.protocolAPI,

                ws:null,

                idRoom:null,
                currentUser:null,
                users:null,
                episode:null,
                pause:null,
                time:0
            }
        },
        created(){
            console.log("Starting connection to WebSocket Server");
            this.ws = new WebSocket(this.$config.ipWebSocket);
        },
        mounted(){
            //sockets
            this.ws.onopen = function () {
                console.log("Successfully connected to the echo WebSocket Server");
                
                $nuxt.$emit('checkRoom');
            };            

            this.ws.onmessage = function(event){
                console.log(event);
                $nuxt.$emit('socket-message', event);
            }

            this.$nuxt.$on('socket-message', (event)=>{
                console.log("Un messaggio arrivato: ");
                console.log(event.data)

                var data = JSON.parse(event.data)
                if(data.action == 'registration'){
                    this.currentUser = data.nickname
                }else if(data.action == 'UpdateRoom'){
                    this.idRoom = data.room.id_room
                    this.users = data.room.clients
                    this.time = data.room.t
                    this.pause = data.room.pause
                }else if(data.action == 'loadVideo'){
                    this.idRoom = data.id
                    this.currentUser = data.nickname
                    this.episode = data.episode
                }
            });

            this.$nuxt.$on('checkRoom', ()=>{
                if(this.$route.query.idroom == null)
                    this.$nuxt.$emit('createRoom');
                else
                    this.$nuxt.$emit('joinRoom');
            });

            this.$nuxt.$on('createRoom', ()=>{
                this.sendMessage('create', {nickname: randomName(), episode: this.$route.query.episode})
                console.log('request registration');
            });
            
            this.$nuxt.$on('joinRoom', ()=>{
                this.sendMessage('join', {nickname: randomName(), idRoom: this.$route.query.idroom})
                console.log('request join');
            });
            
            this.$nuxt.$on('pause', (statePause)=>{
                this.sendMessage('updatePause', {pause: statePause, idRoom: this.idRoom})
                console.log('request updatePause');
            });
            
            this.$nuxt.$on('time', (stateTime)=>{
                this.sendMessage('updateTime', {time: stateTime, idRoom: this.idRoom})
                console.log('request updatePause');
            });

            //event
            var vid = document.getElementById("my-video");

            vid.addEventListener("canplaythrough", () => {
                
                console.log('canplaythrough');
                var vid = document.getElementById("my-video");

                if(vid.currentTime > 0)
                    $nuxt.$emit('time', vid.currentTime);
            });
            vid.addEventListener("pause", () => {
                $nuxt.$emit('pause', true);
            });
            vid.addEventListener("playing", () => {
                $nuxt.$emit('pause', false);
            });
        },
        methods:{
            sendMessage(setAction, data){
                this.ws.send(JSON.stringify({action:setAction, data}))
            },
            load(){
                if(this.$route.query.type === "anime" && this.$route.query.episode != null && this.data == null)
                {
                    this.episode = this.$route.query.episode;
                    
                }else if(this.$route.query.type === "manga" && this.data == null){
                    //get api internal
                    this.$axios.get(`${this.protocol}://${this.host}:${this.port}/chapter/register/chapterid/${this.$route.query.episode}`)
                        .then(rs => {
                        this.data = rs.data
                    });
                }
            },
            getVideoEpisode(id){
                //get api internal
                this.$axios.get(`${this.protocol}://${this.host}:${this.port}/episode/register/episodeid/${id}`)
                    .then(rs => {
                    this.data = rs.data
                });
            },
            getUrl(url){
                
                url = url.replace(/\\/g, '\/');

                var src = url.split('video');
                
                return require('../video'+src[1])
            }
        },
        watch:{
            episode(){
                this.getVideoEpisode(this.episode);
            },
            time(){
                var vid = document.getElementById("my-video");
                vid.currentTime = this.time
            },
            pause(){
                var vid = document.getElementById("my-video");
                console.log('CAMBIO STATO');
                if(this.pause == true){
                    vid.pause();
                }else if(this.pause == false){
                    try
                    {
                        vid.play();
                    }catch{
                        vid.muted = true;
                        vid.play();
                    }
                }
            }
        },
        components:{
            userSession:userComponentVue
        }
    }
</script>

<style>
body{
  background-image: url("../assets/img/background.jpg");
  background-size: contain;
  background-attachment: fixed;
}

</style>