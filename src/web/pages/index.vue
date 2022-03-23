<template>
  <div class="container">
    <h1>AnimePlex</h1>
    <div class="col-sm-5 row">
      <SearchComponent/>
    </div>
    <template v-if="searchView">
      <div v-for="view in anime" :key="view.name" class="row justify-content-center">
        <PreviewAnime :name="view.name" :image="view.image" :data="view"/>
      </div>
    </template>
    <div v-else class="row justify-content-center">
      <DetailsAnime 
        :name="details.name" 
        :date="details.daterelease" 
        :description="details.description" 
        :image="details.image" 
        :status="details.status" 
        :studio="details.studio" 
        :urlPage="details.urlPage"
        :duration="details.durationepisode"
        :totalEpisode="details.episodetotal"
        :vote="details.vote"
      />
    </div>
  </div>
</template>

<script>
import PreviewAnime from "../components/previewAnime.vue"
import DetailsAnime from "../components/detailsAnime.vue";
export default {
    name: "IndexPage",
    data() {
        return {
            anime: [],
            searchView: true,
            details : {}
        };
    },
    created() {
      //get api internal
      this.$nuxt.$on("search", (name) => {
          this.$axios.get("https://localhost:44300/anime/names/" + name)
              .then(rs => {

                //reset
                this.anime = [];

                //put array
                rs.data.forEach(item => {
                  this.anime.push(item);
                });
              console.log(rs);
          });
      });

      //view details anime
      this.$nuxt.$on("viewDetails", (name) => {
        this.searchView = false;
        this.anime.forEach(item => {
          if(item.name == name){
            this.details = item
          }
        });
      })
    },
    components: { PreviewAnime, DetailsAnime }
}
</script>
