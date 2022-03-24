<template>
  <div class="container">

    <!-- head -->
    <div>
      <!-- title -->
      <div style="padding-bottom: 10px;">
        <img src="../assets/img/bar-anime.jpg" style="object-fit: cover;" height="220" width="100%">
      </div>
    </div>
    
    <!-- body -->
    <div style="background-color: white;">
      <!-- bar search -->
      <div class="col-sm-5">
        <SearchComponent />
      </div>

      <!-- animation loading pages -->
      <div v-if="loading" class="d-flex flex-row flex-wrap justify-content-center">
        <div v-for="number in 8" :key="number">
          <PreviewAnimeLoading />
        </div>
      </div>
      
      <!-- empty -->
      <div v-else-if="pages != null && pages.length <= 0" class="row justify-content-center">
        <img src="../assets/img/empty.png" class="img-fluid" style="width: 500px;">
      </div>

      <!-- search -->
      <div v-else-if="searchView">

        <!-- bar pages -->
        <nav aria-label="Page navigation example">
          <template v-if="numberPageTotal != -1">
            <ul class="pagination">
              <template v-if="numberPageCurrent > 0">
                <li class="page-item"><a @click="Previous()" class="page-link" href="#">Previous</a></li>
              </template>
              <div v-for="numberPage in (numberPageTotal + 1)" :key="numberPage">
                  <template v-if="(numberPage-1) == numberPageCurrent">
                    <li class="page-item active">
                        <a @click="ToPage(numberPage)" class="page-link " href="#">{{numberPage}}</a>
                    </li>
                  </template>
                  <template v-else>  
                    <li class="page-item">
                      <a @click="ToPage(numberPage)" class="page-link" href="#">{{numberPage}}</a>
                    </li>
                  </template>
              </div>
              <template v-if="numberPageCurrent < numberPageTotal">
                <li class="page-item"><a @click="Next()" class="page-link" href="#">Next</a></li>
              </template>
            </ul>
          </template>
        </nav>

        <!-- list previewAnime -->
        <div v-if="pages != null"  class="d-flex flex-row flex-wrap justify-content-center">
          <div v-for="view in pages[numberPageCurrent]" :key="view.name">
            <PreviewAnime :name="view.name" :image="view.image" :data="view" :urlExternal="urlExternal"/>
          </div>
        </div>
      </div>

      <!-- details anime -->
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
  </div>
</template>

<script>
import PreviewAnime from "../components/previewAnime.vue"
import DetailsAnime from "../components/detailsAnime.vue";
import SearchComponent from "../components/searchComponent.vue";
import PreviewAnimeLoading from "../components/previewAnimeLoading.vue";

export default {
    name: "IndexPage",
    components: { PreviewAnime, DetailsAnime, SearchComponent, PreviewAnimeLoading },
    data() {
        return {
            searchView: true,
            urlExternal: false,
            payload : {},
            loading: false,
            success: false,

            pages:null,
            numberPageTotal:-1,
            numberPageCurrent:0
        };
    },
    created() {
      //get api internal
      this.$nuxt.$on("search", (name) => {
          this.loading = true;
          this.$axios.get("https://localhost:44300/anime/names/" + name)
            .then(rs => {

              //reset
              this.pages = [];
              this.numberPageTotal = -1;
              this.numberPageCurrent = 0;

              //array to pages
              this.SetPages(rs.data);

              //init
              this.urlExternal = false;
              this.searchView = true;
          })
          .catch(error => {
            if(error.message.includes('404'))
              this.pages = [];
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
              this.pages = [];
              this.numberPageTotal = -1;
              this.numberPageCurrent = 0;

              //array to pages
              this.SetPages(rs.data);

              //init
              this.urlExternal = true;
              this.searchView = true;
          })
          .catch(error => {
            if(error.message.includes('404'))
              this.pages = [];
          })
          .then(() => {
            this.loading = false;
          })
      });

      //view details anime
      this.$nuxt.$on("viewDetails", (name) => {
        this.searchView = false;
        this.pages[this.numberPageCurrent].forEach(item => {
          if(item.name == name){
            this.payload = item
          }
        });
      })

      //close page detailsView
      this.$nuxt.$on("close", (urlExternal) => {
        if(urlExternal == null)
          this.urlExternal = true;
        this.searchView = true;
      })
      
      //close page detailsView
      this.$nuxt.$on("reloadViewDetails", (data) => {
        this.success = true;
        this.searchView = false;
        this.urlExternal = null;
        this.payload = data
      })
    },
    methods:{
      closeAlert(){
        this.success = false;
      },
      ToPage(number){
        this.numberPageCurrent = number - 1
      },
      Previous(){
        if(this.numberPageCurrent > 0)
          this.numberPageCurrent = this.numberPageCurrent - 1
      },
      Next(){
        if(this.numberPageCurrent < this.numberPageTotal)
          this.numberPageCurrent = this.numberPageCurrent + 1
      },
      SetPages(array){
        var temp = []
        //put array
        array.forEach(item => {
          temp.push(item);

          //push limit 10 item for pages
          if(temp.length == 20){
            this.pages.push(temp)
            temp = []
            this.numberPageTotal = this.numberPageTotal + 1
          }
        });

        //push remain anime
        if(temp.length > 0){
          this.pages.push(temp)
          this.numberPageTotal = this.numberPageTotal + 1
        }
      }
    }
}
</script>
