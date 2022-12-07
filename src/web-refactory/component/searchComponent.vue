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
    <div>
      <div
          v-if="isLoading"
          class="d-flex flex-wrap justify-center"
      >
        <previewCardLoading
            v-for="index in 9"
            :key="index"
        />
      </div>
      <template
          v-else-if="!checkNull(data)"
      >
        <div class="d-flex flex-column">
          <div>
            <v-pagination
                v-model="page"
                class="my-4"
                color="white"
                :length="pages.length + 1"
                prev-icon="arrow-left"
                next-icon="arrow-right"
            />
          </div>
          <div class="d-flex flex-row flex-wrap justify-center">
            <previewCard
                v-for="item in pages[page - 1]"
                :key="item.name"
                :item="item"
            />
          </div>
        </div>
      </template>
    </div>
  </div>
</template>
<script>
import lodash from '/mixins/lodash'
import {useStore} from "../store";

import previewCard from "./previewCard";
import previewCardLoading from "./previewCardLoading"
import axios from "axios";
import _ from 'lodash'

export default {
  name: "searchComponent",
  components: {
    previewCard,
    previewCardLoading
  },
  setup() {
    const store = useStore();
    return {store}
  },
  props: [
    'typeSearch'
  ],
  mixins: [
    lodash
  ],
  data() {
    return {
      data: [],
      pages: [],
      isLoading: false,
      search: '',
      page: null
    }
  },
  watch: {
    data() {
      this.setPages();
    }
  },
  computed: {
    getCurrentSelectSearch() {
      const type = this.store.getCurrentSelectSearch;

      this.data = null;
      this.pages = null;
      this.page = null;

      switch (type) {
        case 'all':
          this.getAll();
      }

      return type;
    },
    getPathImages() {
      switch (this.getCurrentSelectSearch) {
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
    getStyleBanner() {
      switch (this.getCurrentSelectSearch) {
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
  methods: {
    setPages() {
      let data = [];
      let pages = [];
      if (!_.isNil(this.data)) {
        for (const item of this.data) {
          if (pages.length > 11) {
            data.push(pages)
            pages = [];
          }
          pages.push(item);
        }

        if (pages.length > 0)
          data.push(pages)

        this.pages = data;
        this.page = this.pages.length > 0 ? 1 : 0;
        this.isLoading = false;
      }
    },
    clickSearch() {
      this.isLoading = true;
      switch (this.getCurrentSelectSearch) {
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
    getAll() {
      this.isLoading = true;
      axios('/api/all')
          .then(res => {
            const {data} = res;
            this.data = data;
          })
          .catch(err => {
            console.log(err)
          })
          .finally(() => {
            this.isLoading = false;
          })
    },
    searchLocal() {
      axios(`/api/search-local?search=${this.search}`)
          .then(res => {
            const {data} = res;
            this.data = data;
          })
          .catch(err => {
            console.log(err)
          })
          .finally(() => {
            this.isLoading = false;
          })
    },
    searchAnime() {
      axios(`/api/search-animesaturn?search=${this.search}`)
          .then(res => {
            const {data} = res;
            this.data = data;
          })
          .catch(err => {
            console.log(err)
          })
          .finally(() => {
            this.isLoading = false;
          })
    },
    searchManga() {
      axios(`/api/search-mangaworld?search=${this.search}`)
          .then(res => {
            const {data} = res;
            this.data = data;
          })
          .catch(err => {
            console.log(err)
          })
          .finally(() => {
            this.isLoading = false;
          })
    }
  }
}
</script>

<style scoped>
.logo {
  height: 220px;
  width: 100%;
  object-fit: contain;
}
</style>