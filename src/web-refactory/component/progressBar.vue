<template>
  <v-progress-linear
      v-model="item.percentualDownload"
      :color="getStatus"
      height="25"
  >
    <template v-slot:default="{ value }">
      <strong>{{ this.item.stateDownload.toUpperCase() }}</strong>
    </template>
  </v-progress-linear>
</template>

<script>
export default {
  name: "progressBar",
  props:[
      'item',
      'type'
  ],
  computed:{
    getStatus(){
      if(this.type === 'anime'){
        switch(this.item.stateDownload){
          case 'downloading':
            return 'primary';
          case 'conversioning':
            return 'info';
          case 'completed':
            return 'success';
          case 'failed':
            return 'error';
          case 'wait conversion':
            return 'gray';
          case 'pending':
            return 'warning';
          default:
            return 'gray';
        }
      }
    }
  }
}
</script>