<template>
<v-alert
  :type="type"
  :icon="getIcon"
>
  {{text}}
</v-alert>
</template>

<script>
export default {
  name: "alert",
  props:[
      'type',
      'text'
  ],
  computed:{
    getIcon(){
      switch (this.type){
        case 'error':
          return '$error';
      }
    }
  }
}
</script>

<style scoped>

</style>