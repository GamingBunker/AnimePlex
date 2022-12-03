<template>
  <v-dialog
      v-model="activator"
      width="500"
      persistent
  >
    <v-card>
      <v-card-item>
        <v-btn
            color="primary"
            @click="$emit('closeDialog')"
            class="my-1"
        >
          <v-icon
            color="white"
          >
            $arrowLeft
          </v-icon>
        </v-btn>
      </v-card-item>
      <v-card-title class="px-0">
        <v-img
            :src="getImage"
            class="card-img-top rounded-top hide-img"
            height="100"
            cover
        >
        </v-img>
        <div class="card-title">
          {{item.name}}
        </div>
      </v-card-title>
      <v-card-text>
        <descriptionAnime
            v-if="type === 'anime'"
            :item="item"
        />
        <statusDownloadAnime
          :item="item"
        />
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<script>
import axios from "axios";
import {Buffer} from "buffer";
import _ from 'lodash'

import descriptionAnime from "./descriptionAnime";
import statusDownloadAnime from "./statusDownloadAnime";

export default {
  name: "detailsCard",
  components: {
    descriptionAnime,
    statusDownloadAnime
  },
  props: [
    'item'
  ],
  data() {
    return {
      activator: true,
      type:'',
    }
  },
  mounted() {
    if(!_.isNil(this.item.episodeTotal))
      this.type='anime';
    else
      this.type='manga';
  },
  computed: {
    getImage() {
      console.log(this.item)
      if (!_.isNil(this.item.image) && this.item.image.indexOf('/9j/') !== -1)
        return 'data:image/jpg;base64,' + this.ConvertBase64(this.item.image);

      return this.item.image;
    }
  },
  methods: {
    ConvertBase64(imgBase64) {
      if (imgBase64 == null)
        return null

      let buff = Buffer.from(imgBase64);
      return buff.toString()
    },
  }
}
</script>

<style lang="scss" scoped>
.hide-img{
  -webkit-filter: blur(5px);
  -moz-filter: blur(5px);
  -o-filter: blur(5px);
  -ms-filter: blur(5px);
}
.card-title{
  position: absolute;
  top: 100px;
  left: 15px;
  color: white;
  font-weight: bold;
  font-size: 30px;
  background-color: rgba(0, 0, 0, 0.3);
}
</style>