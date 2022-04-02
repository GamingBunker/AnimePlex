<template>
  <div class="container">
    <!-- head -->
    <div>
      <!-- title -->
      <div style="padding-bottom: 10px;">
        <img src="../assets/img/bar-anime.jpg" style="object-fit: contain;" height="220" width="100%">
      </div>
    </div>
    
    <!-- body -->
    <div>
      <!-- bar search -->
      <div>
        <SearchComponent />
      </div>

      <!-- validate cert -->
      <template v-if="validateCertificate">
        <a class="btn btn-danger" :href="protocol+'://'+host+':'+port+'/check'" role="button">Validate API</a>
      </template>
      
      <!-- animation loading pages -->
      <div v-if="loading" class="d-flex flex-row flex-wrap justify-content-center"  style="padding-top: 60px;">
        <div v-for="number in 8" :key="number">
          <PreviewAnimeLoading />
        </div>
      </div>
      
      <!-- empty -->
      <div v-else-if="pages != null && pages.length <= 0">
        <div class="alert alert-danger" role="alert">
          <i class="bi bi-emoji-dizzy-fill"></i>
          <strong>Not found!</strong>
        </div>
      </div>

      <!-- search -->
      <div v-else-if="searchView">

        <!-- pagination -->
        <nav style="margin: 10px 0px;">
          <template v-if="numberPageTotal != -1">
            <ul class="pagination nav justify-content-center pagination-sm">
              <div>
                <li class="page-item"><a @click="Previous()" class="page-link" style="margin: 0px 2px;" href="#">Previous</a></li>
              </div>
              <div v-for="numberPage in (numberPageTotal + 1)" :key="numberPage">
                  <template v-if="(numberPage-1) == numberPageCurrent">
                    <li class="page-item active" style="margin: 0px 2px;">
                        <a @click="ToPage(numberPage)" class="page-link " href="#">{{numberPage}}</a>
                    </li>
                  </template>
                  <template v-else>  
                    <li class="page-item" style="margin: 0px 2px;">
                      <a @click="ToPage(numberPage)" class="page-link" href="#">{{numberPage}}</a>
                    </li>
                  </template>
              </div>
              <div>
                <li class="page-item"><a @click="Next()" class="page-link" style="margin: 0px 2px;" href="#">Next</a></li>
              </div>
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
          :date="new Date(Date.parse(payload.dateRelease)).getDay()+'-'+new Date(Date.parse(payload.dateRelease)).getMonth()+'-'+new Date(Date.parse(payload.dateRelease)).getFullYear()" 
          :description="payload.description" 
          :image="payload.image" 
          :status="payload.finish" 
          :studio="payload.studio" 
          :urlPage="payload.urlPage"
          :duration="payload.durationEpisode"
          :totalEpisode="payload.episodeTotal"
          :vote="payload.vote"
          :urlPageDownload="payload.urlPageDownload"
          :urlExternal="urlExternal"
          :exists="payload.exists"
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
            numberPageCurrent:0,
            
            host:this.$config.ipAPI,
            port:this.$config.portAPI,
            protocol:this.$config.protocolAPI,

            validateCertificate:false
        };
    },
    created() {
      //get all api internal
      this.$nuxt.$on("searchall", (name) => {
          this.loading = true;
          this.$axios.get(`${this.protocol}://${this.host}:${this.port}/anime`)
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
            else
              this.validateCertificate = true
          })
          .then(() => {
            this.loading = false;
          })
      });

      //get api internal
      this.$nuxt.$on("search", (name) => {
          this.loading = true;
          this.$axios.get(`${this.protocol}://${this.host}:${this.port}/anime/names/${name}`)
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
            else
              this.validateCertificate = true
          })
          .then(() => {
            this.loading = false;
          })
      });

      //get api external
      this.$nuxt.$on("searchExternal", (name) => {
          this.loading = true;
          this.$axios.get(`${this.protocol}://${this.host}:${this.port}/animesaturn/name/${name}`)
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
            else
              this.validateCertificate = true
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

<style>

body{
  background-image: url("../assets/img/background.jpg");
  background-size: contain;
  background-attachment: fixed;
}

</style>
