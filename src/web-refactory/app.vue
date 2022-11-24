<template>
  <div>
    <NuxtLayout />
  </div>
</template>
<script>
import {useAsyncData, useFetch} from "nuxt/app";
export default {
  data(){
    return{
      rs:null,
    }
  },
  mounted() {
    useAsyncData('hello', async () => {
      const {data} = await useFetch('/api/hello');
      this.rs = data;
    })
  }
}
</script>
