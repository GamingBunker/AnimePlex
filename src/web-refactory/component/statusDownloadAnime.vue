<template>
  <template v-if="!checkNull(item.urlPage)">
    <v-btn
        block
      class="primary"
      @click="showStatus = !showStatus"
    >
      {{showStatus? 'hide status download' : 'show status download'}}
    </v-btn>
  </template>
  <template v-if="showStatus">
    <div v-for="episode in episodes" :key="episode.id" style="margin: 10px 0px;">
      <div>
        <span>Id: <b>{{episode.id}}</b></span>
      </div>
      <div>
        <span>{{episode.animeId}} episode: {{episode.numberEpisodeCurrent}}</span>
      </div>
      <progressBar
        :item="episode"
        type="anime"
      />
    </div>
  </template>
</template>

<script>
import lodash from "../mixins/lodash";

import progressBar from "./progressBar";
import axios from "axios";
import {useFetch} from "nuxt/app";
export default {
  name: "statusDownloadAnime",
  components:{
    progressBar
  },
  props:[
      'item'
  ],
  mixins:[
    lodash
  ],
  data(){
    return{
      showStatus:false,
      episodes:null,
      date:null
    }
  },
  watch:{
    date:{
      handler(){
        setTimeout(async () => {
          const {data} = await useFetch(`/api/anime-episode?name=${this.item.name}`)
          this.episodes = data;
          this.date = new Date();
        }, 2000);
      },
      immediate: true
    }
  }
}
</script>

<style scoped>

</style>