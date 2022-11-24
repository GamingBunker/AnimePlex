<template>
  <template
    v-if="checkNull(getCurrentSelectSearch)"
  >
    <div style="padding-bottom: 10px;">
      <img src="~/assets/images/bar-anime.jpg" class="logo" />
    </div>
  </template>
  <template
    v-else
  >
    <div class="d-flex flex-row align-center justify-center pt-16" style="margin: 0 500px">
      <v-text-field
        variant="solo"
        label="Search"
        hide-details
        single-line
        append-inner-icon="$search"
        @click:append-inner=""
      />
    </div>
  </template>
</template>

<script>
import lodash from '/mixins/lodash'
import {useStore} from "../store";
export default {
  name: "searchComponent",
  setup(){
    const store = useStore();

    return {store}
  },
  computed:{
    getCurrentSelectSearch(){
      return this.store.getCurrentSelectSearch;
    }
  },
  props:[
    'typeSearch'
  ],
  mixins:[
    lodash
  ]
}
</script>

<style scoped>
.logo{
  height: 220px;
  width: 100%;
  object-fit: contain;
}
</style>