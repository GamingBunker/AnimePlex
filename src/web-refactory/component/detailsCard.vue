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
            class="my-1 mr-1"
        >
          <v-icon
              color="white"
          >
            $arrowLeft
          </v-icon>
        </v-btn>
        <template
            v-if="!checkNull(this.item.urlPageDownload)"
        >
          <v-btn
              color="warning"
              @click="download()"
          >
            <template
              v-if="isLoadingDownload"
            >
              <v-progress-circular
                  indeterminate
                  size="25"
              />
            </template>
            <template
              v-else
            >
              <v-icon>
                $download
              </v-icon>
            </template>
          </v-btn>
        </template>
        <template v-else>
          <v-btn
              color="warning"
              class="mr-1"
          >
            <v-icon>
              $redownload
            </v-icon>
          </v-btn>
          <v-btn
              color="error"
          >
            <v-icon>
              $trash
            </v-icon>
          </v-btn>
        </template>
      </v-card-item>
      <template v-if="error">
        <alert
          type="error"
          :text="error"
          class="ma-2"
        />
      </template>
      <v-card-title class="px-0">
        <v-img
            :src="getImage"
            class="card-img-top rounded-top hide-img"
            height="100"
            cover
        >
        </v-img>
        <div class="card-title">
          {{ item.name }}
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
import alert from "./alert";

import lodash from '/mixins/lodash'

export default {
  name: "detailsCard",
  components: {
    descriptionAnime,
    statusDownloadAnime,
    alert
  },
  props: [
    'item'
  ],
  mixins: [
    lodash
  ],
  data() {
    return {
      activator: true,
      type: '',
      isLoadingDownload:false,
      error:null
    }
  },
  mounted() {
    if (!_.isNil(this.item.episodeTotal) || (!_.isNil(this.item.typeView) && this.item.typeView === 'anime'))
      this.type = 'anime';
    else
      this.type = 'manga';
  },
  computed: {
    getImage() {
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
    download(){
      this.isLoadingDownload = true;
      this.error = null;

      axios.post(`/api/anime/download?url=${this.item.urlPageDownload}`)
          .then((res) => {
            const {data} = res;
            this.$emit('updateData', data);
          })
          .catch(() => {
            this.isLoadingDownload = false;
            this.error = 'Impossible send request for download this anime'
          })
    }
  }
}
</script>

<style lang="scss" scoped>
.hide-img {
  -webkit-filter: blur(5px);
  -moz-filter: blur(5px);
  -o-filter: blur(5px);
  -ms-filter: blur(5px);
}

.card-title {
  overflow-wrap: break-word !important;
  position: relative;
  top: -65px;
  left: 15px;
  color: white;
  font-weight: bold;
  font-size: 30px;
  background-color: rgba(0, 0, 0, 0.3);
}
</style>