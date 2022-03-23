<template>
  <div class="container">
    <h1>AnimePlex</h1>
    <div class="col-sm-5 row">
      <SearchComponent />
    </div>
    <div v-if="loading" class="row justify-content-center">
      <div class="spinner-grow" style="width: 120px; height: 120px;" role="status"></div>
    </div>
    <template v-else-if="!(anime == null) && anime.length <= 0">
      <img src="../assets/img/not_found.png" class="img-fluid">
    </template>
    <template v-else-if="searchView">
      <div v-for="view in anime" :key="view.name" class="row justify-content-center">
        <PreviewAnime :name="view.name" :image="view.image" :data="view" :urlExternal="urlExternal"/>
      </div>
    </template>
    <div v-else class="row justify-content-center">
      <DetailsAnime
        :name="payload.name" 
        :date="payload.daterelease" 
        :description="payload.description" 
        :image="payload.image" 
        :status="payload.status" 
        :studio="payload.studio" 
        :urlPage="payload.urlPage"
        :duration="payload.durationepisode"
        :totalEpisode="payload.episodetotal"
        :vote="payload.vote"
        :urlPageDownload="payload.urlPageDownload"
        :urlExternal="urlExternal"
      />
    </div>
  </div>
</template>

<script>
import PreviewAnime from "../components/previewAnime.vue"
import DetailsAnime from "../components/detailsAnime.vue";
import SearchComponent from "../components/searchComponent.vue";

export default {
    name: "IndexPage",
    components: { PreviewAnime, DetailsAnime, SearchComponent },
    data() {
        return {
            anime: null,
            searchView: true,
            urlExternal: false,
            payload : {},
            loading: false
        };
    },
    created() {
      //get api internal
      this.$nuxt.$on("search", (name) => {
          this.loading = true;
          this.$axios.get("https://localhost:44300/anime/names/" + name)
            .then(rs => {

              //reset
              this.anime = [];

              //put array
              rs.data.forEach(item => {
                this.anime.push(item);
              });
              
            console.log(rs);

            //init
            this.urlExternal = false;
            this.searchView = true;
          })
          .catch(error => {
            if(error.message.includes('404'))
              this.anime = []
          })
          .then(() => {
            this.loading = false;
          })
      });

      //get api external
      this.$nuxt.$on("searchExternal", (name) => {
          this.loading = true;
          this.$axios.get("https://localhost:44300/animesaturn/name/" + name)
              .then(rs => {

                //reset
                this.anime = [];

                //put array
                rs.data.forEach(item => {
                  this.anime.push(item);
                });
              console.log(rs);

              //init
              this.urlExternal = true;
              this.searchView = true;
          })
          .catch(error => {
            if(error.message.includes('404'))
              this.anime = []
          })
          .then(() => {
            this.loading = false;
          })
      });

      //view details anime
      this.$nuxt.$on("viewDetails", (name) => {
        this.searchView = false;
        this.anime.forEach(item => {
          if(item.name == name){
            this.payload = item
          }
        });
      })

      //close page detailsView
      this.$nuxt.$on("close", (name) => {
        this.searchView = true;
      })
      
      //close page detailsView
      this.$nuxt.$on("reloadViewDetails", (data) => {
        this.searchView = false;
        this.urlExternal = false;
        this.payload = data
      })
    }
}
</script>
