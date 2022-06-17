<template>
  <div>
    <!-- Problem img tag src 403-->
    <meta name="referrer" content="no-referrer" />
    
    <div style="position:absolute; margin: 10px;">
      <b-button v-b-toggle.sidebar-no-header style="background-color: #98ccd4; border:#98ccd4;"><i class="bi bi-caret-right"></i></b-button>
    </div>
    <b-sidebar id="sidebar-no-header" no-header shadow>
      <template #default="{ hide }">
        <div style="margin: 20px;">
          <!-- Space Disk -->
          <div>
            <SpaceDiskComponent />
          </div>
          <!-- Health Services -->
          <div>
            <HealthServicesComponent />
          </div>
          <b-button variant="primary" block @click="hide">Close</b-button>
        </div>
      </template>
    </b-sidebar>

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
        <!-- <template v-if="validateCertificate">
          <a class="btn btn-danger" :href="protocol+'://'+host+':'+port+'/check'" role="button">Validate API</a>
        </template> -->
        
        <!-- animation loading pages -->
        <div v-if="loading" class="d-flex flex-row flex-wrap justify-content-center"  style="padding-top: 60px;">
          <div v-for="number in 8" :key="number">
            <PreviewLoading />
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
                  <li class="page-item"><a @click="Previous()" class="page-link" style="margin: 0px 2px;" href="#"><i class="bi bi-arrow-left"></i></a></li>
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
                  <li class="page-item"><a @click="Next()" class="page-link" style="margin: 0px 2px;" href="#"><i class="bi bi-arrow-right"></i></a></li>
                </div>
              </ul>
            </template>
          </nav>

          <!-- list Preview -->
          <div v-if="pages != null"  class="d-flex flex-row flex-wrap justify-content-center">
            <div v-for="view in pages[numberPageCurrent]" :key="view.id">
              <Preview :name="view.name" :image="view.image" :urlExternal="urlExternal" :typeView="view.typeView" :positionCurrent="pages[numberPageCurrent].findIndex((e) => e.id == view.id)"/>
            </div>
          </div>
        </div>

        <!-- details anime -->
        <div v-else class="row justify-content-center">
          <template v-if="payload.anime !== undefined">
            <DetailsAnime
              :anime="payload.anime"
              :artist="payload.artist"
              :author="payload.author"
              :dataRelease="payload.dataRelease"
              :description="payload.description"
              :image="payload.image"
              :name="payload.name"
              :status="payload.status"
              :totalChapters="payload.totalChapters"
              :totalVolumes="payload.totalVolumes"
              :type="payload.type"
              :urlPage="payload.urlPage"
              :typeView ="typeView"
              :exists="payload.exists"
            />
          </template>
          <template v-else>
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
              :typeView ="typeView"
            />
          </template>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Preview from "../components/preview.vue"
import DetailsAnime from "../components/details.vue";
import SearchComponent from "../components/searchComponent.vue";
import PreviewLoading from "../components/previewLoading.vue";
import SpaceDiskComponent from "../components/spaceDiskComponent.vue";
import HealthServicesComponent from "../components/healthServicesComponent.vue";

export default {
    name: "IndexPage",
    components: { Preview, DetailsAnime, SearchComponent, PreviewLoading, SpaceDiskComponent, HealthServicesComponent },
    data() {
        return {
            searchView: true,
            urlExternal: false,
            typeView: null,
            payload : {},
            loading: false,
            success: false,

            pages:null,
            numberPageTotal:-1,
            numberPageCurrent:0,
            positionCurrent:0,
            
            host:this.$config.ipAPI,
            port:this.$config.portAPI,
            protocol:this.$config.protocolAPI,
            //validateCertificate:false
        };
    },
    created() {
      //get all api internal
      this.$nuxt.$on("searchall", () => {
          this.loading = true;
          this.$axios.get(`${this.protocol}://${this.host}:${this.port}/all`)
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
            //else
              //this.validateCertificate = true
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
            //else
              //this.validateCertificate = true
          })
          .then(() => {
            this.loading = false;
          })
      });

      //get api external anime
      this.$nuxt.$on("searchExternalAnime", (name) => {
          this.loading = true;
          this.$axios.get(`${this.protocol}://${this.host}:${this.port}/anime/list/name/${name}`)
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
            //else
              //this.validateCertificate = true
          })
          .then(() => {
            this.loading = false;
            this.typeView = "anime";
          })
      });

      //get api external manga
      this.$nuxt.$on("searchExternalManga", (name) => {
          this.loading = true;
          this.$axios.get(`${this.protocol}://${this.host}:${this.port}/manga/list/name/${name}`)
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
            //else
              //this.validateCertificate = true
          })
          .then(() => {
            this.loading = false;
            this.typeView = "manga";
          })
      });

      //view details anime
      this.$nuxt.$on("viewDetails", (positionCurrent, typeView) => {
        this.searchView = false;
        this.payload = this.pages[this.numberPageCurrent][positionCurrent];
        this.typeView = typeView;
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
        var id = 0;
        //put array
        array.forEach(item => {
          if(item.typeView !== undefined && item.typeView === "manga" || item.artist !== undefined)
            item.typeView = "manga"
          else
            item.typeView = "anime"
          
          item.id = id;
          temp.push(item);
          
          //push limit 20 item for pages
          if(temp.length == 20){
            this.pages.push(temp)
            temp = []
            this.numberPageTotal = this.numberPageTotal + 1
          }
          id++;
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

.card
{
  border: none;
  border-top-right-radius: 5px;
  border-top-left-radius: 5px;
  border-bottom-right-radius: 30px;
  border-bottom-left-radius: 30px;
  background-color: rgba(237, 237, 238, 0.95);
}
.card img
{
  border-bottom-right-radius: 25px;
  border-bottom-left-radius: 25px;
}

</style>
