<template>
    <div>
        <template v-if="object.typeView === 'manga'">
            <detailsObject
              :artist="object.artist"
              :author="object.author"
              :dateRelease="new Date(Date.parse(object.dateRelease)).getDay()+'-'+new Date(Date.parse(object.dateRelease)).getMonth()+'-'+new Date(Date.parse(object.dateRelease)).getFullYear()"
              :description="object.description"
              :image="object.image"
              :name="object.name"
              :status="object.status"
              :totalChapters="object.totalChapters"
              :totalVolumes="object.totalVolumes"
              :type="object.type"
              :urlPage="object.urlPage"
              :urlPageDownload="object.urlPageDownload"
              :urlExternal="object.urlExternal"
              :typeView ="object.typeView"
              :exists="object.exists"
            />
        </template>
        <template v-else>
            <detailsObject
              :name="object.name" 
              :dateRelease="new Date(Date.parse(object.dateRelease)).getDay()+'-'+new Date(Date.parse(object.dateRelease)).getMonth()+'-'+new Date(Date.parse(object.dateRelease)).getFullYear()" 
              :description="object.description" 
              :image="object.image" 
              :status="object.finish" 
              :studio="object.studio"
              :duration="object.durationEpisode"
              :totalEpisode="object.episodeTotal"
              :vote="object.vote"
              :urlPage="object.urlPage"
              :urlPageDownload="object.urlPageDownload"
              :urlExternal="object.urlExternal"
              :exists="object.exists"
              :typeView ="object.typeView"
            />
        </template>
    </div>
</template>

<script>
import detailsVue from '../components/detailsObject.vue'
import {AnimeStore, MangaStore} from "../stores/store"
    export default {
        components:{
            detailsObject:detailsVue
        },
        data(){
            return{
                animeStore:AnimeStore(),
                mangaStore:MangaStore(),

                object:null
            }
        },
        created(){
            if(this.animeStore.selectAnime != null)
                this.object = this.animeStore.selectAnime
            else
                this.object = this.mangaStore.selectManga
        },
        computed:{
            object(){
                if(this.animeStore.selectAnime != null)
                    return this.animeStore.selectAnime
                else
                    return this.mangaStore.selectManga
            }
        }
    }
</script>