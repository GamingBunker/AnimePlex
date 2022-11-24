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
body{
  background-image: url("assets/images/background.jpg") !important;
  background-size: contain;
  background-attachment: fixed;
}
#main{
  background-image: url("assets/images/background.jpg");
  background-repeat: repeat-y;
  background-size: contain;
}
</style>