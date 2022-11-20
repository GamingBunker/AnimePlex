<template>
  <div
      id="main"
      class="fill-height"
  >
      <div style="padding-bottom: 10px;">
        <img src="~/assets/images/bar-anime.jpg" class="logo" />
      </div>
  </div>
</template>

<script>
import searchComponent from "../components/searchComponent";
export default {
  name: "index",
  components:{
    searchComponent
  }
}
</script>

<style scoped>
.logo{
  height: 220px;
  width: 100%;
  object-fit: contain;
}
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