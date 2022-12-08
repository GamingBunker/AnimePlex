<template>
  <v-card
    width="18rem"
    class="mr-2 ml-1 my-2"
  >
    <v-img
        :src="!item.urlPageDownload? 'data:image/jpg;base64,'+ConvertBase64(item.image) : item.image"
        class="card-img-top rounded-top"
        height="400"
        cover
    />
    <v-card-title>
      {{ item.name }}
    </v-card-title>
    <v-card-actions>
      <v-btn
        :class="getStatusDetails"
        block
        @click="details()"
      >
        Details
      </v-btn>
    </v-card-actions>
  </v-card>
  <component
    :is="activeModal"
    :item="checkNull(data)? item : data"
    @closeDialog="closeDialog"
    @updateData="updateData"
    @closeDialogAndUpdate="$emit('closeDialogAndUpdate')"
  />
</template>

<script>
import { Buffer } from 'buffer'

import detailsCard from "./detailsCard";

import _ from 'lodash'

import api from '/mixins/api'
import lodash from '/mixins/lodash'
import axios from "axios";
export default {
  name:"previewCard",
  components:{
    detailsCard
  },
  props: [
      'item'
  ],
  mixins:[
    api,
      lodash
  ],
  data(){
    return{
      activeModal:null,
      data:null
    }
  },
  computed:{
    getStatusDetails(){
      if(!_.isNil(this.item.exists) && this.item.exists === true){
        return 'success'
      }
      return 'primary'
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
      if(!_.isNil(this.item.exists) && this.item.exists === true){
        if(this.item.typeView === 'anime')
        {
          axios(`/api/anime/get?search=${this.item.name}`)
              .then(res => {
                this.data = res.data[0];
              })
              .finally(() => {
                this.activeModal = 'detailsCard'
              })
        }
      }else
        this.activeModal = 'detailsCard'
    },
    closeDialog(){
      this.activeModal = null
    },
    updateData(data){
      this.data = data;
    }
  }
}
</script>