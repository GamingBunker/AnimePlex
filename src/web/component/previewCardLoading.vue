<template>
  <v-card
      width="18rem"
      class="mr-2 ml-1 my-2"
  >
    <v-sheet
        class="animated-background"
        height="400"
    />
    <v-card-title>
      <v-sheet
          class="animated-background"
          width="70%"
          height="30"
      />
    </v-card-title>
    <v-card-actions>

      <v-sheet
          class="animated-background mx-1"
          width="100%"
          height="35"
      />
    </v-card-actions>
  </v-card>
</template>
<script>
export default {
  name: 'previewCardLoading'
}
</script>
<style lang="scss" scoped>
@keyframes placeHolderShimmer {
  0% {
    background-position: -468px 0
  }
  100% {
    background-position: 468px 0
  }
}

.animated-background {
  animation-duration: 1.25s;
  animation-fill-mode: forwards;
  animation-iteration-count: infinite;
  animation-name: placeHolderShimmer;
  animation-timing-function: ease-in;
  background: darkgray;
  background: linear-gradient(to right, #9e9e9e 10%, #b6b6b6 30%, #9e9e9e 33%);
  background-size: 800px 104px;
  height: 100px;
  position: relative;
}
</style>