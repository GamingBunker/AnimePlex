<template>
  <div>
    <div
        class="ma-auto"
        id="banner"
        :style="getStyleBanner"
    >
      <v-img
          :src="getPathImages"
          class="logo"
          height="200"
      />
    </div>
    <div
        v-if="!(checkNull(getCurrentSelectSearch) || getCurrentSelectSearch === 'all')"
        class="ma-auto mt-5"
        style="width: 50%; min-width: 200px"
    >
      <v-text-field
          v-model="search"
          variant="solo"
          label="Search"
          hide-details
          single-line
          color="primary"
          append-inner-icon="$search"
          :loading="isLoading"
          @click:append-inner="clickSearch()"
      />
    </div>
    <div
      class="d-flex flex-wrap justify-center"
    >
      <template
        v-if="isLoading"
      >
        <previewCardLoading
          v-for="index in 9"
          :key="index"
        />
      </template>
      <template
          v-else-if="!checkNull(data)"
      >
        <previewCard
            v-for="item in data"
            :key="item.name"
            :item="item"
        />
      </template>
    </div>
  </div>
</template>
<script>
import lodash from '/mixins/lodash'
import {useStore} from "../store";
import {useAsyncData, useFetch} from "nuxt/app";

import previewCard from "./previewCard";
import previewCardLoading from "./previewCardLoading"
import axios from "axios";

export default {
  name: "searchComponent",
  components:{
    previewCard,
    previewCardLoading
  },
  setup(){
    const store = useStore();
    return {store}
  },
  props:[
    'typeSearch'
  ],
  mixins:[
    lodash
  ],
  data(){
    return{
      data:null,
      isLoading:false,
      search:''
    }
  },
  computed:{
    getCurrentSelectSearch(){
      const type = this.store.getCurrentSelectSearch;
      this.data = null;
      switch (type){
        case 'all':
          this.getAll();
      }

      return type;
    },
    getPathImages(){
      switch (this.getCurrentSelectSearch){
        case null:
        case 'all':
        case "search-local":
          return "/images/bar-anime.jpg";
        case "search-animesaturn":
          return "/images/logo_animesaturn.png";
        case "search-mangaworld":
          return "/images/MangaWorldLogo.svg";
      }
    },
    getStyleBanner(){
      switch (this.getCurrentSelectSearch){
        case null:
        case 'all':
        case "search-local":
          return "width: 100%;";
        case "search-animesaturn":
          return "width: 25%;";
        case "search-mangaworld":
          return "width: 50%;";
      }
    }
  },
  methods:{
    getAll(){
      this.isLoading = true;
      useAsyncData('getAll', async () => {
        const {data} = await useFetch('/api/all');
        this.data = data;
        this.isLoading = false;
      })
    },
    clickSearch(){
      this.isLoading = true;
      switch (this.getCurrentSelectSearch){
        case "search-local":
          this.searchLocal();
          break;
        case "search-animesaturn":
          this.searchAnime();
          break;
        case "search-mangaworld":
          this.searchManga();
          break;
      }
    },
    async searchLocal(){
      const {data} = await axios('/api/search-local', { search: this.search })
      this.data = data;
      this.isLoading = false;
    },
    async searchAnime(){
      const {data} = axios('/api/search-animesaturn', { search: this.search })
      this.data = data;
      this.isLoading = false;
    },
    async searchManga(){
      const {data} = axios('/api/search-mangaworld', { search: this.search })
      this.data = data;
      this.isLoading = false;
    },
  }
}
</script>

<style scoped>
.logo{
  height: 220px;
  width: 100%;
  object-fit: contain;
}
</style>