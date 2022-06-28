<template>
    <div>
        <!-- input -->
        <div class="d-flex align-items-center">
            <div class="input-group" style="margin: 5px 0px;">
                <input v-model="value" type="text" class="form-control" placeholder="name..." style="width: 200px;">
            </div>
            <div class="row center">
                <div class="btn-group" role="group" style="height: 40px; margin-left: 10px;">
                    <button @click="search()" class="btn btn-danger" type="button" style="background-color: blueviolet; border-color: blueviolet; width: 33%;">
                        <template v-if="type === CONST_DEFAULT_ANIME">
                            <img src="../assets/img/logo_animesaturn.png" style="object-fit: cover; margin-left: -6px; margin-top: -2px; width: 50px;">
                        </template>
                        <template v-else>
                            <img src="../assets/img/MangaWorldLogo.svg" style="object-fit: cover; margin-top: -2px; width: 50px;">
                        </template>
                    </button>
                    <button @click="searchDB()" class="btn btn-light" type="button" style="width: 33%;"><i class="bi bi-search"></i></button>
                </div>
            </div>
        </div>
        <!-- pagination -->
        <nav style="margin: 10px 0px;">
            <!-- anime -->
            <template v-if="animeStore.getIndex.length > 0">
                <ul class="pagination nav justify-content-center pagination-sm">
                    <div>
                        <li class="page-item"><a @click="redo()" class="page-link" style="margin: 0px 2px;" href="#"><i class="bi bi-arrow-left"></i></a></li>
                    </div>
                    <div v-for="numberPage in (animeStore.getIndex.length)" :key="numberPage">
                        <template v-if="(numberPage-1) == animeStore.getActualPage">
                            <li class="page-item active" style="margin: 0px 2px;">
                                <a @click="toPage(numberPage)" class="page-link " href="#">{{numberPage}}</a>
                            </li>
                        </template>
                        <template v-else>  
                            <li class="page-item" style="margin: 0px 2px;">
                                <a @click="toPage(numberPage)" class="page-link" href="#">{{numberPage}}</a>
                            </li>
                        </template>
                    </div>
                    <div>
                        <li class="page-item"><a @click="next()" class="page-link" style="margin: 0px 2px;" href="#"><i class="bi bi-arrow-right"></i></a></li>
                    </div>
                </ul>
            </template>
            <template v-if="mangaStore.getIndex.length > 0">
                <ul class="pagination nav justify-content-center pagination-sm">
                    <div>
                        <li class="page-item"><a @click="redo()" class="page-link" style="margin: 0px 2px;" href="#"><i class="bi bi-arrow-left"></i></a></li>
                    </div>
                    <div v-for="numberPage in (mangaStore.getIndex.length)" :key="numberPage">
                        <template v-if="(numberPage-1) == mangaStore.getActualPage">
                            <li class="page-item active" style="margin: 0px 2px;">
                                <a @click="toPage(numberPage)" class="page-link " href="#">{{numberPage}}</a>
                            </li>
                        </template>
                        <template v-else>  
                            <li class="page-item" style="margin: 0px 2px;">
                                <a @click="toPage(numberPage)" class="page-link" href="#">{{numberPage}}</a>
                            </li>
                        </template>
                    </div>
                    <div>
                        <li class="page-item"><a @click="next()" class="page-link" style="margin: 0px 2px;" href="#"><i class="bi bi-arrow-right"></i></a></li>
                    </div>
                </ul>
            </template>
        </nav>
        <!-- preview -->
        <div v-if="animeStore.getAnime != null"  class="d-flex flex-row flex-wrap justify-content-center">
            <div v-for="view in animeStore.getAnime" :key="view.name">
              <preview :object="view"/>
            </div>
        </div>
        <div v-if="mangaStore.getManga != null"  class="d-flex flex-row flex-wrap justify-content-center">
            <div v-for="view in mangaStore.getManga" :key="view.name">
              <preview :object="view"/>
            </div>
        </div>
        <!-- previewLoading -->
        <div class="d-flex flex-row flex-wrap justify-content-center">
            <template v-if="loading == true">
                <div v-for="number in 8" :key="number">
                    <previewLoading />
                </div>
            </template>
        </div>
    </div>
</template>

<script>
import {AnimeStore, MangaStore} from "../stores/store"
import previewVue from "./previewObject.vue";
import previewLoadingVue from "./previewLoading.vue";
    export default {
        props:{
            type:String
        },
        components:{
            preview:previewVue,
            previewLoading:previewLoadingVue
        },
        data(){
            return{
                value:null,
                loading:false,

                
                animeStore:AnimeStore(),
                mangaStore:MangaStore(),

                CONST_DEFAULT_ANIME:'anime',

                protocol:import.meta.env.VITE_PROTOCOL_API,
                host:import.meta.env.VITE_HOST_API,
                port:import.meta.env.VITE_PORT_API
            }
        },
        methods:{
            search(){
                this.loading = true;

                if(this.type === this.CONST_DEFAULT_ANIME)
                {
                    this.$http.get(`${this.protocol}://${this.host}:${this.port}/anime/list/name/${this.value}`)
                        .then(rs => {
                            this.animeStore.insertAnime(rs.data);
                    })
                    .catch(error => {
                        if(error.message.includes('404'))
                        this.pages = [];
                        //else
                        //this.validateCertificate = true
                    })
                    .then(() => {
                        this.loading = false;
                    })
                }else
                {
                    //get api external manga
                    this.$http.get(`${this.protocol}://${this.host}:${this.port}/manga/list/name/${this.value}`)
                        .then(rs => {
                            this.mangaStore.insertManga(rs.data);
                    })
                    .catch(error => {
                        if(error.message.includes('404'))
                        this.pages = [];
                        //else
                        //this.validateCertificate = true
                    })
                    .then(() => {
                        this.loading = false;
                    })
                }
            },
            searchDB(){
                this.loading = true;

                if(this.type === this.CONST_DEFAULT_ANIME)
                {
                    this.$http.get(`${this.protocol}://${this.host}:${this.port}/anime/names/${this.value}`)
                        .then(rs => {
                            this.animeStore.insertAnime(rs.data);
                    })
                    .catch(error => {
                        if(error.message.includes('404'))
                        this.pages = [];
                        //else
                        //this.validateCertificate = true
                    })
                    .then(() => {
                        this.loading = false;
                    })
                }else
                {
                    this.$http.get(`${this.protocol}://${this.host}:${this.port}/manga/names/${this.value}`)
                        .then(rs => {
                            this.mangaStore.insertManga(rs.data);
                    })
                    .catch(error => {
                        if(error.message.includes('404'))
                        this.pages = [];
                        //else
                        //this.validateCertificate = true
                    })
                    .then(() => {
                        this.loading = false;
                    })
                }
            },
            next(){
                if(this.animeStore.getIndex.length > 0)
                    this.animeStore.nextPage()
                else
                    this.mangaStore.nextPage()
            },
            redo(){
                if(this.animeStore.getIndex.length > 0)
                    this.animeStore.redoPage()
                else
                    this.mangaStore.redoPage()
            },
            toPage(numberPositionPage){
                if(this.animeStore.getIndex.length > 0)
                    this.animeStore.toPage(numberPositionPage)
                else
                    this.mangaStore.toPage(numberPositionPage)
            }
        }
    }
</script>