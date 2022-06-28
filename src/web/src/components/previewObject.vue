<template>
    <div class="card" style="width: 18rem; padding: 0px; margin-right: 7px; margin-left: 3px; margin-bottom:10px;">
        <div class="row">
            <div class="col">
                <template v-if="!(object.urlExternal) || object.urlExternal == false">
                    <img :src="'data:image/jpg;base64,'+ConvertBase64(object.image)" class="card-img-top rounded-top" width="400" height="450" style="object-fit: cover;">
                </template>
                <template v-else>
                    <img :src="object.image" class="card-img-top rounded-top" width="400" height="450" style="object-fit: cover;">
                </template>
            </div>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col">
                    <h5 class="card-title">{{object.name}}</h5>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <a @click="details()" class="btn btn-primary">Details</a>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import router from "../router";
import {Buffer } from "buffer"
import {AnimeStore, MangaStore} from "../stores/store"
    export default {
        data(){
            return{
                animeStore:AnimeStore(),
                mangaStore:MangaStore(),
            }
        },
        props:{
            object:Object
        },
        methods: {
            ConvertBase64(imgBase64){
                var buff = Buffer.from(imgBase64);
                return  buff.toString()
            },
            details(){
                
                if(this.object.typeView === 'anime')
                    this.animeStore.select(this.object)
                else
                    this.mangaStore.select(this.object)

                router.push("/details")
            }
        }
    }
</script>