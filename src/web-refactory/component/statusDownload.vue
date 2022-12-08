<template>
  <v-expansion-panels
      v-if="checkNull(this.item.urlPageDownload)"
      v-model="showStatus"
      class="mt-3"
  >
    <v-expansion-panel>
      <v-expansion-panel-title
        color="primary"
        style="color: white !important;"
        collapse-icon="$sortUp"
        expand-icon="$sortDown"
      >
        {{showStatus === 0? 'Hide status download' : 'Show status download'}}
      </v-expansion-panel-title>
      <v-expansion-panel-text>
        <template v-if="error && checkNull(episodes)">
          <alert
              type="error"
              text="Impossible download information for episodes"
          />
        </template>
        <template v-else-if="checkNull(episodes)">
          <div class="d-flex justify-center">
            <v-progress-circular
              indeterminate
              size="60"
              width="10"
              color="primary"
            />
          </div>
        </template>
        <template v-else>
          <v-sheet
              v-for="media in episodes"
              :key="media.id"
              class="mt-3 pa-2 rounded card-download"
              color="secondary"
              elevation="10"
          >
            <div class="pa-1 d-flex flex-column">
              <template v-if="type === 'anime'">
                <span>Episode: {{media.numberEpisodeCurrent}}</span>
              </template>
              <template v-else>
                <span>Volume: {{media.currentVolume}}</span>
                <span>Chapter: {{media.currentChapter}}</span>
              </template>
            </div>
            <progressBar
                :item="media"
                :type="type"
            />
          </v-sheet>
        </template>
      </v-expansion-panel-text>
    </v-expansion-panel>
  </v-expansion-panels>
</template>

<script>
import progressBar from "./progressBarAnime";
import alert from "./alert";

import lodash from "../mixins/lodash";

import _ from 'lodash'
import axios from "axios";

export default {
  name: "statusDownload",
  components:{
    progressBar,
    alert
  },
  props:[
      'item',
      'type'
  ],
  mixins:[
    lodash
  ],
  data(){
    return{
      showStatus:null,
      episodes:null,
      date:null,
      error:null,
      TIMEOUT: 2000
    }
  },
  updated() {
    this.date = new Date();
  },
  watch:{
    date:{
      handler(){
        setTimeout(() => {
          if(_.isNil(this.item.urlPageDownload))
          {
            axios(`/api/${this.type}/${this.type === 'anime'? 'episode' : 'chapter'}?name=${this.item.name}`, {timeout: this.TIMEOUT})
                .then(res => {
                  const {data} = res;
                  this.episodes = data;
                  this.error = null;
                })
                .catch(err => {
                  console.log(err)
                  this.error = true;
                })
            this.date = new Date();
          }
        }, this.TIMEOUT);
      },
      immediate: true
    }
  }
}
</script>

<style scoped>
.card-download{
  color: white !important;
}
</style>