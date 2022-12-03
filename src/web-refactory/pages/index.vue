<template>
  <div
      id="main"
      class="fill-height"
  >
    <searchComponent />
  </div>
</template>

<script>
import searchComponent from "../component/searchComponent";
export default {
  name: "index",
  components:{
    searchComponent
  },
}
</script>

<style scoped>
#main{
  background-image: url("/images/background.jpg");
  background-repeat: repeat-y;
  background-size: contain;
}
</style>