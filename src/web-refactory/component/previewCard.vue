<template>
  <v-card
    width="18rem"
    class="mr-2 ml-1 my-2"
  >
    <v-img
        :src="item.image.indexOf('/9j/') !== -1? 'data:image/jpg;base64,'+ConvertBase64(item.image) : item.image"
        class="card-img-top rounded-top"
        height="400"
        cover
    />
    <v-card-title>
      {{ item.name }}
    </v-card-title>
    <v-card-actions>
      <v-btn
        class="primary"
        block
        @click="details()"
      >
        Details
      </v-btn>
    </v-card-actions>
  </v-card>
  <component
    :is="activeModal"
    :item="item"
    @closeDialog="closeDialog"
  />
</template>

<script>
import { Buffer } from 'buffer'

import detailsCard from "./detailsCard";
export default {
  name:"previewCard",
  components:{
    detailsCard
  },
  props: [
      'item'
  ],
  data(){
    return{
      activeModal:null
    }
  },
  methods: {
    ConvertBase64(imgBase64) {
      if (imgBase64 == null)
        return null

      let buff = Buffer.from(imgBase64);
      return buff.toString()
    },
    details(){
      this.activeModal = 'detailsCard'
    },
    closeDialog(){
      this.activeModal = null
    }
  }
}
</script>