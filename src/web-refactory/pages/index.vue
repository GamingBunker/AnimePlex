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